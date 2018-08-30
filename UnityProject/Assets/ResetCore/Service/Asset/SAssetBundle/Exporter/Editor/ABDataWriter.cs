using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ResetCore.SAsset
{
    public abstract class ABDataWriter
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="path"></param>
        /// <param name="targets"></param>
        public void Save(string path, ABTarget[] targets)
        {
            FileStream fs = new FileStream(path, FileMode.CreateNew);
            Save(fs, targets);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="targets"></param>
        public abstract void Save(Stream stream, ABTarget[] targets);
    }

    /// <summary>
    /// 依赖关系文本数据写入
    /// </summary>
    public class ABTextDataWriter : ABDataWriter
    {
        public override void Save(Stream stream, ABTarget[] targets)
        {
            StreamWriter sw = new StreamWriter(stream);
            //写入文件头判断文件类型用，ABDT 意思即 Asset-Bundle-Data-Text
            sw.WriteLine("ABDT");

            for (int i = 0; i < targets.Length; i++)
            {
                ABTarget target = targets[i];
                HashSet<ABTarget> deps = target.GetAllDependencies();

                sw.WriteLine(target.assetPath); // debug name
                sw.WriteLine(target.bundleName); // bundle name
                sw.WriteLine(target.fileHash); // file hash
                sw.WriteLine((int)target.compositeType); // export type

                sw.WriteLine(deps.Count);
                foreach (ABTarget item in deps)
                {
                    sw.WriteLine(item.bundleName);
                }
                sw.WriteLine("<------------->");
            }
            sw.Close();
        }
    }

    /// <summary>
    /// 依赖关系二进制数据写入
    /// </summary>
    public class ABBinaryDataWriter : ABDataWriter
    {
        public override void Save(Stream stream, ABTarget[] targets)
        {
            BinaryWriter sw = new BinaryWriter(stream);
            //写入文件头判断文件类型用，ABDB 意思即 Asset-Bundle-Data-Binary
            sw.Write(new char[] { 'A', 'B', 'D', 'B' });

            List<string> bundleNames = new List<string>();

            for (int i = 0; i < targets.Length; i++)
            {
                ABTarget target = targets[i];
                bundleNames.Add(target.bundleName);
            }

            //写入文件名池
            sw.Write(bundleNames.Count);
            for (int i = 0; i < bundleNames.Count; i++)
            {
                sw.Write(bundleNames[i]);
            }

            //写入详细信息
            for (int i = 0; i < targets.Length; i++)
            {
                ABTarget target = targets[i];
                HashSet<ABTarget> deps = target.GetAllDependencies();

                sw.Write(target.assetPath); //debug name
                sw.Write(bundleNames.IndexOf(target.bundleName)); //bundle name
                sw.Write(target.fileHash); //file hash
                sw.Write((int)target.compositeType); //export type

                sw.Write(deps.Count);
                foreach (ABTarget item in deps)
                {
                    sw.Write(bundleNames.IndexOf(item.bundleName));
                }
            }
            sw.Close();
        }
    }
}
