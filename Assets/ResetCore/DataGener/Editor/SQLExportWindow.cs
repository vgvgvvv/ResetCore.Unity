#if MYSQL
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using ResetCore.MySQL;

namespace ResetCore.Data
{
    public class SQLExportWindow : BaseExportWindow
    {

        //显示窗口的函数
        [MenuItem("Tools/GameData/MySQL Exporter")]
        static void ShowMainWindow()
        {
            SQLExportWindow window =
                EditorWindow.GetWindow(typeof(SQLExportWindow), true, "MySQL Exporter") as SQLExportWindow;
            window.Show();
        }

        string url = "127.0.0.1";
        string id = "root";
        string pwd = string.Empty;
        string port = "3306";
        //string database = string.Empty;

        int currentDatabaseIndex = 0;
        string currentDatabseName = string.Empty;

        int currentSheetIndex = 0;
        string currentSheetName = string.Empty;

        protected override void ShowOpen()
        {
            GUIStyle headStyle = GUIHelper.MakeHeader();
            GUILayout.Label("Input SQL", headStyle);
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Url:", GUILayout.Width(100));
            url = GUILayout.TextField(url, GUILayout.Width(250));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Id:", GUILayout.Width(100));
            id = GUILayout.TextField(id, GUILayout.Width(250));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("PassWord:", GUILayout.Width(100));
            pwd = GUILayout.TextField(pwd, GUILayout.Width(250));
            GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal();
            //GUILayout.Label("Databse:", GUILayout.Width(100));
            //database = GUILayout.TextField(database, GUILayout.Width(250));
            //GUILayout.EndHorizontal();

            if (GUILayout.Button("Connect", GUILayout.Width(150)))
            {
                try
                {
                    reader = new SQLReader(string.Empty, string.Empty, id, pwd, url, port);
                }
                catch (Exception e)
                {
                    reader = null;
                    Debug.LogException(e);
                }
            }

            if (reader != null)
            {
                string[] databaseNames = MySQLManager.GetAllDatabaseName().GetColume(0).ToArray();
                EditorGUILayout.LabelField("Database: ", GUILayout.Width(100));
                currentDatabaseIndex = EditorGUILayout.Popup(currentDatabaseIndex, databaseNames, GUILayout.Width(100));
                if (databaseNames != null)
                {
                    currentDatabseName = databaseNames[currentDatabaseIndex];
                    reader = new SQLReader(currentDatabseName, "", id, pwd, url, port);
                    //databaseNames = MySQLManager.GetAllDatabaseName().GetColume(0).ToArray();
                }

                string[] tableNames = MySQLManager.GetAllTableName().GetColume(0).ToArray();
                EditorGUILayout.LabelField("Worksheet: ", GUILayout.Width(100));
                currentSheetIndex = EditorGUILayout.Popup(currentSheetIndex, tableNames, GUILayout.Width(100));
                if (tableNames != null)
                {
                    currentSheetName = tableNames[currentSheetIndex];
                    reader = new SQLReader((reader as SQLReader).database, currentSheetName, id, pwd, url, port);
                    //tableNames = reader.GetSheetNames();
                }

                EditorGUILayout.LabelField("DataType: ", GUILayout.Width(100));
                currentExcelType = (DataType)EditorGUILayout.EnumPopup(currentExcelType, GUILayout.Width(100));
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
#endif