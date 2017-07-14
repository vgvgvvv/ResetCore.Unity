using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using ResetCore.Data.GameDatas.Xml;
using System;

namespace ResetCore.Data
{
    public class Source2PrefData : ISource2
    {
        public DataType dataType
        {
            get
            {
                return DataType.Pref;
            }
        }

        public void GenData(IDataReadable reader, string outputPath = null)
        {
            
            IDataReadable exReader = reader;
            XDocument xDoc = new XDocument();
            XElement root = new XElement("Root");
            xDoc.Add(root);

            List<string> name = exReader.GetMemberNames();
            List<string> value = exReader.GetColume(2);
            for (int i = 0; i < name.Count; i++)
            {
                XElement item = new XElement(name[i], value[i]);
                root.Add(item);
            }

            if(outputPath == null)
            {
                outputPath = PathConfig.GetLocalGameDataPath(PathConfig.DataType.Pref)
                    + Path.GetFileNameWithoutExtension(reader.currentDataTypeName) + PrefData.m_fileExtention;
            }
            if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            }

            xDoc.Save(outputPath);
            AssetDatabase.Refresh();
        }

        public void GenCS(IDataReadable reader)
        {
            string className = reader.currentDataTypeName;
            PrefDataClassGener.CreateNewClass(className, reader.fieldDict, reader.attributeDict);
        }

    }

}
