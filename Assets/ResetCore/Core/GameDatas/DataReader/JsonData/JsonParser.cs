using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using ResetCore.Data;

namespace ResetCore.Json
{
    public class JsonParser
    {
        //加载Json
        public static bool LoadIntMap(string fileName,
            out Dictionary<int, Dictionary<string, string>> dicFromXml, string rootPath = null)
        {
            dicFromXml = new Dictionary<int, Dictionary<string, string>>();

           
         
            JsonData data = JsonMapper.ToObject(DataUtil.LoadFile(fileName, rootPath));
            List<Dictionary<string, string>> strList = JsonMapper.ToObject<List<Dictionary<string, string>>>(data[fileName].ToJson());
            for(int i = 0; i < strList.Count; i++)
            {
                dicFromXml.Add(i + 1, strList[i]);
            }
            
            return true;
        }
    }

}
