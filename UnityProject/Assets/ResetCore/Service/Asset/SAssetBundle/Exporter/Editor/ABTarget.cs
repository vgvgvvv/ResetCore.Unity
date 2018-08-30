using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ResetCore.Util;
using UnityEditor;
using UnityEngine;

namespace ResetCore.SAsset
{


    public class ABTarget
    {
        /// <summary>
        /// 目标Object
        /// </summary>
        public UnityEngine.Object asset { get; private set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public FileInfo fileInfo { get; private set; }
        /// <summary>
        /// 相对于Assets文件夹的目录
        /// </summary>
        public string assetPath { get; private set; }
        /// <summary>
        /// 文件的md5值
        /// </summary>
        public string fileHash { get; private set; }
        /// <summary>
        /// 导出类型
        /// </summary>
        public ABExportType exportType { get; set; } = ABExportType.Asset;
        /// <summary>
        /// 保存地址
        /// </summary>
        public string bundleSavePath { get; private set; }
        /// <summary>
        /// bundleName
        /// </summary>
        public string bundleName { get; private set; }

        /// <summary>
        /// 上下文
        /// </summary>
        public ABExportContext context { get; private set; }

        /// <summary>
        /// 我要依赖的项，例如我是材质，这个就是贴图
        /// </summary>
        private HashSet<ABTarget> dependencies = new HashSet<ABTarget>();
        /// <summary>
        /// 依赖我的项，例如我是材质，这个就是模型
        /// </summary>
        private HashSet<ABTarget> reverseDependencies = new HashSet<ABTarget>();

        private bool isBeforeExportProcessed = false;//已经进行过预处理
        private bool isAnalyzed = false; // 是否已分析过依赖

        /// <summary>
        /// 导出类型
        /// </summary>
        public ABExportType compositeType
        {
            get
            {
                ABExportType type = exportType;
                if (type == ABExportType.Root && reverseDependencies.Count > 0)
                {
                    type |= ABExportType.Asset;
                }
                return type;
            }
        }

        /// <summary>
        /// 是不是自身的原因需要导出如指定的类型prefab等，有些情况下是因为依赖树原因需要单独导出
        /// </summary>
        public bool needSelfExport
        {
            get
            {
                bool v = exportType == ABExportType.Root || exportType == ABExportType.Standalone;
                return v;
            }
        }


        private ABTarget(UnityEngine.Object obj, FileInfo file, string assetPath)
        {
            this.asset = obj;
            this.fileInfo = file;
            this.assetPath = assetPath;
            this.bundleName = ConvertToABName() + ".ab";
            this.bundleSavePath = Path.Combine(ABExportConst.PlatformBundleSavePath, bundleName);
            using (FileStream fs = new FileStream(assetPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fileHash = HashUtil.Get(fs);
                fs.Close();
            }
        }

        /// <summary>
        /// 从上下文中获取或创建Target
        /// </summary>
        /// <param name="context"></param>
        /// <param name="file"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ABTarget GetOrCreateTarget(ABExportContext context, FileInfo file, System.Type t = null)
        {
            ABTarget target = null;
            string fullPath = file.FullName;
            int index = fullPath.IndexOf("Assets");
            if (index < 0)
                return null;

            //通过assetPath寻找
            string assetPath = fullPath.Substring(index);
            if (context.TryGetTarget(assetPath, out target))
            {
                return target;
            }

            UnityEngine.Object obj = t != null ? AssetDatabase.LoadAssetAtPath(assetPath, t) : AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (obj == null)
                return null;

            //通过InstanceId寻找
            int instID = obj.GetInstanceID();
            if (context.TryGetTarget(instID, out target))
            {
                return target;
            }

            //创建新的Target
            target = new ABTarget(obj, file, assetPath);
            target.context = context;

            context.AddTarget(target);

            return target;
        }

        /// <summary>
        /// 将Target从上下文中移除
        /// </summary>
        /// <param name="target"></param>
        public static void RemoveTarget(ABTarget target)
        {
            target.context.RemoveTarget(target);
        }

        #region 打包预处理

        /// <summary>
        /// 分析关系依赖
        /// </summary>
        public void Analyze()
        {
            if (isAnalyzed) return;
            isAnalyzed = true;

            //得到依赖
            UnityEngine.Object[] deps = EditorUtility.CollectDependencies(new UnityEngine.Object[] { asset });

            var depList = from dep in deps
                          let path = AssetDatabase.GetAssetPath(dep)
                          where !(dep is MonoScript || path.StartsWith("Resources"))
                          select AssetDatabase.GetAssetPath(dep);

            string[] paths = depList.Distinct().ToArray();

            paths.Foreach<string>((path) =>
            {
                if (!File.Exists(path))
                    return;

                FileInfo fileInfo = new FileInfo(path);
                ABTarget target = GetOrCreateTarget(context, fileInfo);
                if (target == null)
                    return;

                AddDepend(target);
                target.Analyze();
            });
        }

        /// <summary>
        /// 合并依赖
        /// </summary>
        public void Merge()
        {
            if (reverseDependencies.Count <= 1)
                return;
            var children = new List<ABTarget>(reverseDependencies);
            reverseDependencies.Foreach(RemoveDepend);
            children.ForEach(AddReverseDepend);
        }

        /// <summary>
        /// 确定是否需要独立打包
        /// </summary>
        public void BeforeExportProcess()
        {
            if (isBeforeExportProcessed) return;
            isBeforeExportProcessed = true;

            reverseDependencies.Foreach((target) => { target.BeforeExportProcess(); });

            if (exportType == ABExportType.Asset)
            {
                if (GetRootNumber().Count > 1)
                {
                    exportType = ABExportType.Standalone;
                }
            }
        }

        #endregion 打包预处理

        #region 依赖相关操作

        /// <summary>
        /// 获取所有依赖
        /// </summary>
        /// <returns></returns>
        public HashSet<ABTarget> GetAllDependencies()
        {
            var result = new HashSet<ABTarget>();
            GetDependecies(result);
            return result;
        }

        private void GetDependecies(HashSet<ABTarget> targets)
        {
            var ie = dependencies.GetEnumerator();
            while (ie.MoveNext())
            {
                ABTarget target = ie.Current;
                if (target.needSelfExport)
                {
                    targets.Add(target);
                }
                else
                {
                    target.GetDependecies(targets);
                }
            }
        }

        /// <summary>
        /// 添加本对象的依赖
        /// </summary>
        /// <param name="target"></param>
        private void AddDepend(ABTarget target)
        {
            if (target == this || this.ContainsDepend(target))
            {
                return;
            }
            if (target.ContainsDepend(this))
            {
                Debug.LogError("出现循环依赖：" + target.bundleName + " 与 " + bundleName);
                return;
            }
            dependencies.Add(target);
            target.reverseDependencies.Add(this);

            //遍历依赖树依赖我的对象不需要再依赖该目标
            IEnumerable<ABTarget> cols = dependencies;
            cols = new ABTarget[] { target };
            foreach (var at in cols)
            {
                var e = reverseDependencies.GetEnumerator();
                while (e.MoveNext())
                {
                    ABTarget dc = e.Current;
                    if (dc == null) continue;
                    dc.RemoveDepend(at);
                }
                e.Dispose();
            }
        }

        /// <summary>
        /// 向依赖本对象的依赖列表中添加项
        /// </summary>
        /// <param name="target"></param>
        private void AddReverseDepend(ABTarget target)
        {
            target.AddDepend(this);
        }

        /// <summary>
        /// 是否已经依赖（遍历依赖树）
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool ContainsDepend(ABTarget target)
        {
            if (dependencies.Contains(target))
            {
                return true;
            }
            var e = dependencies.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.ContainsDepend(target))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 移除本对象的依赖项
        /// </summary>
        /// <param name="target"></param>
        private void RemoveDepend(ABTarget target)
        {
            dependencies.Remove(target);
            target.reverseDependencies.Remove(this);
        }

        /// <summary>
        /// 从依赖本对象的依赖列表中移除
        /// </summary>
        /// <param name="target"></param>
        private void RemoveReverseDepend(ABTarget target)
        {
            target.RemoveDepend(this);
        }

        /// <summary>
        /// 获取反向依赖的根数目
        /// </summary>
        /// <returns></returns>
        private HashSet<ABTarget> GetRootNumber()
        {
            var rootSet = new HashSet<ABTarget>();
            GetRootNumber(rootSet);
            return rootSet;
        }

        private void GetRootNumber(HashSet<ABTarget> rootSet)
        {
            switch (exportType)
            {
                case ABExportType.Standalone:
                case ABExportType.Root:
                    rootSet.Add(this);
                    break;
                default:
                    foreach (ABTarget target in reverseDependencies)
                    {
                        target.GetRootNumber(rootSet);
                    }
                    break;
            }
        }

        #endregion 依赖相关操作

        #region 私有函数

        /// <summary>
        /// 获得AssetBundle名字
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public string ConvertToABName()
        {
            string bn = assetPath
                .Replace(ABExportConst.AssetPath, "")
                .Replace('\\', '.')
                .Replace('/', '.')
                .Replace(" ", "_")
                .ToLower();
            return bn;
        }

        #endregion 私有函数
    }
}

