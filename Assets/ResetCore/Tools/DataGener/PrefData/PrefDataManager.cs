using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ResetCore.Util;

namespace ResetCore.Data.GameDatas.Xml
{
    public class PrefDataManager
    {

        /// <summary>
        /// 得到所有可用的预置文件
        /// </summary>
        /// <returns></returns>
        public static List<string> GetPrefDataFileNames()
        {
            string prefRoot = PathConfig.GetLocalGameDataPath(PathConfig.DataType.Pref);
            PathEx.MakeDirectoryExist(prefRoot);
            string[] fileNames = Directory.GetFiles(prefRoot);
            List<string> results = new List<string>();

            foreach (string fileName in fileNames)
            {
                if (Path.GetExtension(fileName) == PrefData.m_fileExtention)
                {
                    results.Add(Path.GetFileName(fileName));
                }
            }

            return results;
        }
    }
}
