using System.Collections;
using System.Collections.Generic;
using System.IO;
using ResetCore.Util;
using UnityEditor;
using UnityEngine;

namespace ResetCore.SAsset
{
    public class ABBuilder
    {
        
        /// <summary>
        /// 上下文
        /// </summary>
        protected ABExportContext context = new ABExportContext();

        /// <summary>
        /// 数据写入
        /// </summary>
        public ABDataWriter dataWriter { get; private set; }

        public ABBuilder(ABFormat writerType)
        {
            if (writerType == ABFormat.Bin)
            {
                this.dataWriter = new ABBinaryDataWriter();
            }
            else
            {
                this.dataWriter = new ABTextDataWriter();
            }
        }
        
        /// <summary>
        /// 构建Bundle
        /// </summary>
        /// <param name="fliter"></param>
        public void BuildAssetBundles(ABFliter fliter)
        {
            AddRootTarget(fliter);
            Analyze();
            Export(fliter.exportDirectory);
            SaveDep(fliter.exportDirectory);
            RemoveUnused();
            AssetDatabase.Refresh();
        }


        #region 私有函数
        
        
        /// <summary>
        /// 添加根目标
        /// </summary>
        /// <param name="fliter"></param>
        private void AddRootTarget(ABFliter fliter)
        {
            var files = fliter.GetFiles();
            files.ForEach((file) =>
            {
                var target = ABTarget.GetOrCreateTarget(context, file);
                if (target == null)
                {
                    Debug.LogError(file);
                    return;
                }
                target.exportType = ABExportType.Root;
            });
        }

        /// <summary>
        /// 分析依赖关系进行分包
        /// </summary>
        private void Analyze()
        {
            var allTarget = context.allTarget;
            float total = allTarget.Count;
            float count = 0;
            for (int i = 0; i < allTarget.Count; i++)
            {
                allTarget[i].Analyze();
                EditorUtility.DisplayProgressBar($"Analyze...({count}/{total})", allTarget[i].assetPath, ++count / total);
            }

            allTarget = context.allTarget;
            total = allTarget.Count;
            count = 0;
            for (int i = 0; i < allTarget.Count; i++)
            {
                allTarget[i].Merge();
                EditorUtility.DisplayProgressBar($"Merge...({count}/{total})", allTarget[i].assetPath, ++count / total);
            }

            allTarget = context.allTarget;
            total = allTarget.Count;
            count = 0;
            for (int i = 0; i < allTarget.Count; i++)
            {
                allTarget[i].BeforeExportProcess();
                EditorUtility.DisplayProgressBar($"BeforeExport...({count}/{total})", allTarget[i].assetPath, ++count / total);
            }
        }

        /// <summary>
        /// 进行最后的导出操作
        /// </summary>
        /// <param name="exportDir"></param>
        private void Export(string exportDir)
        {

            var allTarget = context.allTarget;
            //标记所有 asset bundle name，如果需要单独打包则标出assetbundle名
            for (int i = 0; i < allTarget.Count; i++)
            {
                ABTarget target = allTarget[i];
                AssetImporter importer = AssetImporter.GetAtPath(target.assetPath);
                if (!importer) continue;
                importer.assetBundleName = target.needSelfExport ? target.bundleName : null;
            }

            var bundleSavePath = ABExportConst.PlatformBundleSavePath + "/" + exportDir;
            PathEx.MakeDirectoryExist(bundleSavePath);
            AssetDatabase.Refresh();

            //开始打包
            BuildPipeline.BuildAssetBundles(bundleSavePath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

            //清除所有 asset bundle name
            for (int i = 0; i < allTarget.Count; i++)
            {
                ABTarget target = allTarget[i];
                if (!target.needSelfExport) continue;
                AssetImporter importer = AssetImporter.GetAtPath(target.assetPath);
                if (!importer) continue;
                importer.assetBundleName = null;
            }
        }

        /// <summary>
        /// 保存依赖文件
        /// </summary>
        /// <param name="fliter"></param>
        private void SaveDep(string exportDirectory)
        {
            var all = context.allTarget;
            var path = PathEx.Combine(ABExportConst.PlatformBundleSavePath, exportDirectory,
                ABPathResolver.DEPEND_FILE_NAME);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            List<ABTarget> exportList = new List<ABTarget>();
            for (int i = 0; i < all.Count; i++)
            {
                ABTarget target = all[i];
                if (target.needSelfExport)
                {
                    exportList.Add(target);
                }
            }
            dataWriter.Save(path, exportList.ToArray());
        }

        /// <summary>
        /// 删除未使用的AB，可能是上次打包出来的，而这一次没生成的
        /// </summary>
        private void RemoveUnused()
        {
            var all = context.allTarget;
            HashSet<string> usedSet = new HashSet<string>();
            for (int i = 0; i < all.Count; i++)
            {
                ABTarget target = all[i];
                if (target.needSelfExport)
                    usedSet.Add(target.bundleName);
            }

            DirectoryInfo di = new DirectoryInfo(ABExportConst.PlatformBundleSavePath); ;

            FileInfo[] abFiles = di.GetFiles("*.ab");
            for (int i = 0; i < abFiles.Length; i++)
            {
                FileInfo fi = abFiles[i];
                if (usedSet.Add(fi.Name))
                {
                    Debug.Log("Remove unused AB : " + fi.Name);

                    fi.Delete();
                    File.Delete(fi.FullName + ".manifest");
                }
            }
        }
        
        
        #endregion 私有函数

    }


}
