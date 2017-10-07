using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ResetCore.SAsset
{
    public class ABFliter
    {
        /// <summary>
        /// 导出过滤器
        /// </summary>
        public class FileFliter
        {

            public string path { get; private set; }
            public string[] partterns { get; private set; }

            public FileFliter(string path, params string[] partterns)
            {
                this.path = path;
                this.partterns = partterns;
            }
        }
        /// <summary>
        /// 导出目录
        /// </summary>
        public string exportDirectory { get; private set; }
        
        /// <summary>
        /// 过滤器集合
        /// </summary>
        public List<FileFliter> fliters { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="exportDirectory"></param>
        /// <param name="fliters"></param>
        public ABFliter(string exportDirectory, List<FileFliter> fliters)
        {
            this.exportDirectory = exportDirectory;
            this.fliters = fliters;
        }

        /// <summary>
        /// 获得与过滤器相符的文件
        /// </summary>
        /// <param name="ignoreMeta"></param>
        /// <returns></returns>
        public List<FileInfo> GetFiles(bool ignoreMeta = true)
        {
            List<FileInfo> result = new List<FileInfo>();

            fliters.ForEach((fliter) =>
            {
                var currentFliter = fliter;
                if (!Directory.Exists(currentFliter.path))
                    return;

                var directory = new DirectoryInfo(currentFliter.path);
                var partterns = currentFliter.partterns == null || currentFliter.partterns.Length == 0
                    ? new string[] { "*.*" }
                    : currentFliter.partterns;


                partterns.Foreach<string>((partternIndex, parttern) =>
                {
                    FileInfo[] files = directory.GetFiles(parttern.Trim(), SearchOption.AllDirectories);
                    files.Foreach<FileInfo>((file) =>
                    {
                        if (file == null || 
                            ignoreMeta && file.Extension == ".meta" ||
                            result.Contains(file))
                            return;

                        result.Add(file);

                    });
                });

            });

            return result;
        }

    }
}
