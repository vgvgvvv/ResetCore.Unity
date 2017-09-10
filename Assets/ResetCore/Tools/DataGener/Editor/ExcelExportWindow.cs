using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using ResetCore.Excel;
using System;
using System.Collections.Generic;

namespace ResetCore.Data
{
    public class ExcelExportWindow : BaseExportWindow
    {

        //显示窗口的函数
        [MenuItem("Tools/GameData/Excel Exporter")]
        static void ShowMainWindow()
        {
            ExcelExportWindow window =
                EditorWindow.GetWindow(typeof(ExcelExportWindow), true, "Excel Exporter") as ExcelExportWindow;
            window.Show();
        }


        string excelFilePath = "";
        string fileName = "";
        string[] sheetsNames = { "" };
        int currentSheetIndex = 0;
        string currentSheetName = "";


        protected override void ShowOpen()
        {
            GUIStyle headStyle = GUIHelper.MakeHeader();
            GUILayout.Label("Select File", headStyle);
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Label("File:", GUILayout.Width(50));

            string path = string.Empty;
            if (string.IsNullOrEmpty(excelFilePath))
                path = Application.dataPath;
            else
                path = excelFilePath;

            excelFilePath = GUILayout.TextField(path, GUILayout.Width(250));
            if (GUILayout.Button("...", GUILayout.Width(20)))
            {
                string folder = Path.GetDirectoryName(path);
#if UNITY_EDITOR_WIN
                path = EditorUtility.OpenFilePanel("Open Excel file", folder, "excel files;*.xls;*.xlsx");
#else
            path = EditorUtility.OpenFilePanel("Open Excel file", folder, "xls");
#endif

                if (path.Length != 0)
                {
                    fileName = Path.GetFileName(path);

                    // the path should be relative not absolute one to make it work on any platform.
                    int index = path.IndexOf("Assets");
                    if (index >= 0)
                    {
                        // set relative path
                        excelFilePath = path.Substring(index);

                        //
                        reader = new ExcelReader(path);

                        // pass absolute path
                        sheetsNames = reader.GetSheetNames();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error",
                            @"Wrong folder is selected.
                        Set a folder under the 'Assets' folder! \n
                        The excel file should be anywhere under  the 'Assets' folder", "OK");
                        return;
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("Spreadsheet File: " + fileName);

            EditorGUILayout.Space();

            if (reader == null) return;

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Worksheet: ", GUILayout.Width(100));
                currentSheetIndex = EditorGUILayout.Popup(currentSheetIndex, sheetsNames, GUILayout.Width(100));
                currentExcelType = (DataType)EditorGUILayout.EnumPopup(currentExcelType, GUILayout.Width(100));
                if (sheetsNames != null)
                {
                    currentSheetName = sheetsNames[currentSheetIndex];

                    // reopen the excel file e.g) new worksheet is added so need to reopen.
                    reader = new ExcelReader(path, currentSheetName, currentExcelType);
                    sheetsNames = reader.GetSheetNames();

                    // one of worksheet was removed, so reset the selected worksheet index
                    // to prevent the index out of range error.
                    if (sheetsNames.Length <= currentSheetIndex)
                    {
                        currentSheetIndex = 0;
                        string message = "Worksheet was changed. Check the 'Worksheet' and 'Update' it again if it is necessary.";
                        EditorUtility.DisplayDialog("Info", message, "OK");
                    }
                }
            }



        }

        protected override void ShowSetType()
        {
            GUIStyle headStyle = GUIHelper.MakeHeader();
            GUILayout.Label("设置类型种类", headStyle);
            EditorGUILayout.Space();

            //表头部
            const int MEMBER_WIDTH = 100;
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("成员", GUILayout.MinWidth(MEMBER_WIDTH));
            GUILayout.FlexibleSpace();
            string[] names = { "类型", "注释" };
            int[] widths = { 100, 100 };
            for (int i = 0; i < names.Length; i++)
            {
                GUILayout.Label(new GUIContent(names[i]), GUILayout.Width(widths[i]));
            }
            GUILayout.EndHorizontal();//EditorStyles.toolbar

            EditorGUILayout.BeginVertical("box");

            Dictionary<string, Type> fieldDict = reader.fieldDict;

            List<string> comment = reader.GetComment();


            int index = 0;
            //表内容
            foreach (string fieldName in fieldDict.Keys)
            {
                GUILayout.BeginHorizontal();

                // member field label
                EditorGUILayout.LabelField(fieldName, GUILayout.MinWidth(MEMBER_WIDTH));
                GUILayout.FlexibleSpace();

                // type enum popup
                //header.type = (CellType)EditorGUILayout.EnumPopup(header.type, GUILayout.Width(100));
                GUILayout.Label(fieldDict[fieldName].Name, GUILayout.Width(100));
                //excelReader.fieldDict[field.Key] = Type.GetType(GUILayout.TextField(field.Value.Name, GUILayout.Width(100)));
                GUILayout.Space(20);

                // array toggle
                GUILayout.Label(comment[index], GUILayout.Width(100));

                GUILayout.Space(10);
                GUILayout.EndHorizontal();

                index++;

            }
            EditorGUILayout.EndVertical();
        }
    }
}
