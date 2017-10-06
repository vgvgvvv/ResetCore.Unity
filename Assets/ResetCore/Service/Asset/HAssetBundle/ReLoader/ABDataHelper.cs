using System;
using System.Collections.Generic;
using System.IO;
using ResetCore.Debugger;
using ResetCore.Util;

namespace ResetCore.SAsset
{
    /// <summary>
    /// 文本文件格式说明
    /// *固定一行字符串ABDT
    /// 循环 { AssetBundleData
    ///     *名字(string)
    ///     *短名字(string)
    ///     *Hash值(string)
    ///     *类型(AssetBundleExportType)
    ///     *依赖文件个数M(int)
    ///     循环 M {
    ///         *依赖的AB文件名(string)
    ///     }
    /// }
    /// </summary>
    public class ABDataHelper
    {
        protected Dictionary<uint, ABData> _infoMap = new Dictionary<uint, ABData>();

        /// <summary>
        /// 获取AssetBundle数据
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public ABData GetABData(uint fullName)
        {
            if (fullName != 0 && _infoMap.ContainsKey(fullName))
            {
                return _infoMap[fullName];
            }
            return null;
        }

        public void Init()
        {
            string localPath = ABPathResolver.GetBundleSourceFile(ABPathResolver.DEPEND_FILE_NAME, false);
            string cachePath = string.Format("{0}/{1}", ABPathResolver.BundleCacheDir, ABPathResolver.DEPEND_FILE_NAME);

            ReDebug.Log(ReLogType.System, "ABDataHelper", "######## " + localPath);

            if (File.Exists(localPath))
            {
                readFromPath(localPath);
            }
            else if (File.Exists(cachePath))
            {
                readFromPath(cachePath);
            }
        }

        #region read file
        /// <summary>
        /// 读取ABData数据
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="useBin"></param>
        public void Read(FileStream fs, bool useBin)
        {
            if (useBin)
            {
                readBin(fs);
            }
            else
            {
                readText(fs);
            }
        }

        /// <summary>
        /// 从路径中读取
        /// </summary>
        /// <param name="filePath"></param>
        public void readFromPath(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (fs.Length > 4)
                {
                    BinaryReader br = new BinaryReader(fs);
                    if (br.ReadChar() == 'A' && br.ReadChar() == 'B' && br.ReadChar() == 'D')
                    {
                        if (br.ReadChar() == 'T')
                        {
                            fs.Position = 0;
                            Read(fs, false);
                        }
                        else
                        {
                            fs.Position = 0;
                            Read(fs, true);
                        }
                    }
                    br.Close();
                }
                fs.Close();
                ReDebug.Log(ReLogType.System, "ABDataHelper", string.Format("Init dependency info success, file=", filePath));
            }
        }

        #region 私有函数

        private void readBin(Stream fs)
        {
            if (fs.Length < 4) return;

            BinaryReader sr = new BinaryReader(fs);
            char[] fileHeadChars = sr.ReadChars(4);
            //读取文件头判断文件类型，ABDB 意思即 Asset-Bundle-Data-Binary
            if (fileHeadChars[0] != 'A' || fileHeadChars[1] != 'B' || fileHeadChars[2] != 'D' || fileHeadChars[3] != 'B')
            {
                return;
            }

            int namesCount = sr.ReadInt32();
            uint[] names = new uint[namesCount];
            for (int i = 0; i < namesCount; i++)
            {
                names[i] = uint.Parse(sr.ReadString().Replace(".ab", ""));
            }

            while (true)
            {
                if (fs.Position == fs.Length)
                {
                    break;
                }
                string debugName = sr.ReadString();
                uint name = names[sr.ReadInt32()];
                string hash = sr.ReadString();
                int typeData = sr.ReadInt32();
                int depsCount = sr.ReadInt32();

                uint[] deps = new uint[depsCount];
                for (int i = 0; i < depsCount; i++)
                {
                    deps[i] = names[sr.ReadInt32()];
                }

                ABData info = new ABData();
                info.hash = hash;
                info.fullName = name;
                info.debugName = debugName;
                info.dependencies = deps;
                info.compositeType = (ABExportType)typeData;
                _infoMap[name] = info;
            }
            sr.Close();
        }

        private void readText(Stream fs)
        {
            StreamReader sr = new StreamReader(fs);
            char[] fileHeadChars = new char[6];
            sr.Read(fileHeadChars, 0, fileHeadChars.Length);
            //读取文件头判断文件类型，ABDT 意思即 Asset-Bundle-Data-Text
            if (fileHeadChars[0] != 'A' || fileHeadChars[1] != 'B' || fileHeadChars[2] != 'D' || fileHeadChars[3] != 'T')
            {
                return;
            }

            while (true)
            {
                string debugName = sr.ReadLine();
                if (string.IsNullOrEmpty(debugName))
                {
                    break;
                }
                uint name = uint.Parse(sr.ReadLine().Replace(".ab", ""));
                string hash = sr.ReadLine();
                int typeData = Convert.ToInt32(sr.ReadLine());
                int depsCount = Convert.ToInt32(sr.ReadLine());

                uint[] deps = new uint[depsCount];
                for (int i = 0; i < depsCount; i++)
                {
                    deps[i] = uint.Parse(sr.ReadLine().Replace(".ab", ""));
                }
                sr.ReadLine(); // skip <------------->

                ABData info = new ABData();
                info.debugName = debugName;
                info.hash = hash;
                info.fullName = name;
                info.dependencies = deps;
                info.compositeType = (ABExportType)typeData;
                _infoMap[name] = info;
            }
            sr.Close();
        }

        #endregion

        #endregion

        #region write file
        /// <summary>
        /// 写入ABData
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="useBin"></param>
        public void Write(FileStream fs, bool useBin)
        {
            if (useBin)
            {
                writeBin(fs);
            }
            else
            {
                writeText(fs);
            }
        }

        #region 私有函数

        private void writeBin(Stream fs)
        {
            BinaryWriter sw = new BinaryWriter(fs);
            //写入文件头判断文件类型用，ABDB 意思即 Asset-Bundle-Data-Binary
            sw.Write(new char[] { 'A', 'B', 'D', 'B' });

            List<string> bundleNames = new List<string>();
            List<ABData> abDataList = new List<ABData>();
            abDataList.AddRange(_infoMap.Values);
            for (int i = 0; i < abDataList.Count; i++)
            {
                bundleNames.Add(abDataList[i].fullName + ".ab");
            }

            //写入文件名池
            sw.Write(bundleNames.Count);
            for (int i = 0; i < bundleNames.Count; i++)
            {
                sw.Write(bundleNames[i]);
            }

            //写入详细信息
            for (int i = 0; i < abDataList.Count; i++)
            {
                ABData data = abDataList[i];

                sw.Write(data.debugName); //debug name
                sw.Write(bundleNames.IndexOf(data.fullName + ".ab")); //bundle name
                sw.Write(data.hash); //file hash
                sw.Write((int)data.compositeType); //export type

                sw.Write(data.dependencies.Length);
                foreach (uint name in data.dependencies)
                {
                    sw.Write(bundleNames.IndexOf(name + ".ab"));
                }
            }
            sw.Close();
        }
        private void writeText(Stream fs)
        {
            StreamWriter sw = new StreamWriter(fs);
            //写入文件头判断文件类型用，ABDT 意思即 Asset-Bundle-Data-Text
            sw.WriteLine("ABDT");

            foreach (KeyValuePair<uint, ABData> pair in _infoMap)
            {
                ABData data = pair.Value;
                if (data == null)
                {
                    continue;
                }
                sw.WriteLine(data.debugName); // debug name
                sw.WriteLine(data.fullName + ".ab"); // bundle name
                sw.WriteLine(data.hash); // file hash
                sw.WriteLine((int)data.compositeType); // export type

                if (data.dependencies == null)
                {
                    UnityEngine.Debug.LogError("##### " + data.debugName);
                }
                sw.WriteLine(data.dependencies.Length);
                foreach (uint name in data.dependencies)
                {
                    sw.WriteLine(name + ".ab");
                }
                sw.WriteLine("<------------->");
            }
            sw.Close();
        }
        #endregion

        #endregion
    }
}
