using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.IO;
using ResetCore.Data;
using System;
using ResetCore.Excel;

namespace ResetCore.Data
{
    public class ExcelExportInMenu
    {
        /// <summary>
        /// 导出选中的Excel为Xml
        /// </summary>
        [MenuItem("Assets/DataHelper/Excel/Xml/Export Selected Excel")]
        public static void ExportAllSelectedExcelToXml()
        {
            ExportAllSelectExcel(new Source2Xml());
        }

        [MenuItem("Assets/DataHelper/Excel/Json/Export Selected Excel")]
        public static void ExportAllSelectedExcelToJson()
        {
            ExportAllSelectExcel(new Source2Json());
        }

        /// <summary>
        /// 导出选中的Excel为首选项
        /// </summary>
        [MenuItem("Assets/DataHelper/Excel/PrefData/Export Selected Excel")]
        public static void ExportAllSelectedExcelToPrefData()
        {
            ExportAllSelectExcel(new Source2PrefData());
        }

        /// <summary>
        /// 导出选中的Excel为Protobuf（暂时不可用）
        /// </summary>
        [MenuItem("Assets/DataHelper/Protobuf/Export Selected Excel(暂时不可用)")]
        public static void ExportAllSelectedExcelToProtobuf()
        {
            ExportAllSelectExcel(new Source2Protobuf());
        }

        /// <summary>
        ///  导出选中的Excel为Scriptable Object（暂时不可用）
        /// </summary>
        [MenuItem("Assets/DataHelper/Scriptable Object/Export Selected Excel（暂时不可用）")]
        public static void ExportAllSelectedExcelToScriptableObject()
        {
            ExportAllSelectExcel(new Source2ScrObj());
        }

        private static void ExportAllSelectExcel(ISource2 source2)
        {
            ExportData((item, sheetName) =>
            {
                ExcelReader reader = new ExcelReader(item, sheetName, source2.dataType);
                source2.GenData(reader);
                source2.GenCS(reader);
            });
        }

        /// <summary>
        /// 导出所有本地化数据
        /// </summary>
        [MenuItem("Assets/DataHelper/Excel/Language/Export All Localization File")]
        [MenuItem("Tools/GameData/Language/Export All Localization File By Excel")]
        static void ExportLanguageFile()
        {
            Source2Localization.ExportExcelFile();
        }

        /// <summary>
        /// 打开本地化Excel
        /// </summary>
        [MenuItem("Tools/GameData/Language/Open Localization Excel")]
        static void OpenLanguageExcel()
        {
            EditorUtility.OpenWithDefaultApp(PathConfig.LanguageDataExcelPath);
        }

        private static void ExportData(Action<string, string> genAction)
        {
            var selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            var paths = (from s in selection
                         let path = AssetDatabase.GetAssetPath(s)
                         where (path.EndsWith(".xlsx") || path.EndsWith(".xls"))
                         select path).ToArray();

            int num = 1;
            foreach (string item in paths)
            {
                IDataReadable reader = new ExcelReader(item);
                foreach (string sheetName in reader.GetSheetNames())
                {
                    EditorUtility.DisplayProgressBar
                        ("Exporting Excel", "Current: " + num + "/" + paths.Length + " File: " + Path.GetFileName(item) +
                        " Sheet: " + sheetName, (float)num / (float)paths.Length);

                    genAction(item, sheetName);

                }
                num++;
            }
            EditorUtility.ClearProgressBar();
            Debug.unityLogger.Log("Finished");
        }
        
    }

}
