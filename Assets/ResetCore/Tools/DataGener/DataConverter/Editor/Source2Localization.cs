using System.Xml.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;
using ResetCore.Excel;

namespace ResetCore.Data
{
    public class Source2Localization
    {
        public static void ExportExcelFile()
        {
            IDataReadable reader = new ExcelReader(PathConfig.LanguageDataExcelPath);
            GenData(reader);
        }

        public static void ExportMySQLFile()
        {

        }

        private static void GenData(IDataReadable rder)
        {
            IDataReadable reader = rder; 

            XDocument xDoc = new XDocument();
            XElement root = new XElement("Root");
            xDoc.Add(root);

            //List<string> commentLine = reader.GetLine(0, 1);
            List<string> keyLine = reader.GetColume(1, 1);

            int num = 2;
            Array languageType = Enum.GetValues(typeof(LanguageConst.LanguageType));
            for (int n = 0; n < languageType.Length; n++)
            {
                XElement languageEle = new XElement("item");
                root.Add(languageEle);

                List<string> valueLine = reader.GetColume(num, 1, keyLine.Count);

                for (int i = 0; i < keyLine.Count; i++)
                {
                    if (valueLine.Count > i)
                    {
                        languageEle.Add(new XElement(keyLine[i], valueLine[i]));
                    }
                    else
                    {
                        languageEle.Add(new XElement(keyLine[i], valueLine[i]));
                    }
                }

                num++;
            }

            string outputPath = PathConfig.LanguageDataPath;
            if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            }

            xDoc.Save(outputPath);

            AssetDatabase.Refresh();
        }

    }

}
