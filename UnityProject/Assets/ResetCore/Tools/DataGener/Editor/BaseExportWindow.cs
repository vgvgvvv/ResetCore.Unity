using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ResetCore.Data
{
    public abstract class BaseExportWindow : EditorWindow
    {

        protected IDataReadable reader { get; set; }
        protected DataType currentExcelType = DataType.Normal;

        protected virtual void OnGUI()
        {
            ShowOpen();
            EditorGUILayout.Space();

            if (reader == null) return;

            ShowSetType();
            EditorGUILayout.Space();

            switch (currentExcelType)
            {
                case DataType.Normal:
                    {
                        ShowExportXml();
                        EditorGUILayout.Space();
                        //ShowExportProtobuf();
                        //EditorGUILayout.Space();
                        ShowExportObj();
                        EditorGUILayout.Space();
                        ShowExportJson();
                    }
                    break;
                case DataType.Pref:
                    {
                        ShowExportPrf();
                    }
                    break;
            }
        }

        /// <summary>
        /// 显示打开工具
        /// </summary>
        protected abstract void ShowOpen();
        /// <summary>
        /// 显示DataSet显示工具
        /// </summary>
        protected abstract void ShowSetType();
        /// <summary>
        /// 导出Xml
        /// </summary>
        protected void ShowExportXml()
        {
            GUIStyle headStyle = GUIHelper.MakeHeader();
            GUILayout.Label("导出Xml（支持大多数类型）", headStyle);
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("导出Xml", GUILayout.Width(100)))
            {
                new Source2Xml().GenData(reader);
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("导出XmlData.cs", GUILayout.Width(100)))
            {
                new Source2Xml().GenCS(reader);
                AssetDatabase.Refresh();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("全部导出", GUILayout.Width(100)))
            {
                new Source2Xml().GenData(reader);
                new Source2Xml().GenCS(reader);
            }
        }
        /// <summary>
        /// 导出Obj
        /// </summary>
        protected void ShowExportObj()
        {
            GUIStyle headStyle = GUIHelper.MakeHeader();
            GUILayout.Label("导出Obj(开发中)", headStyle);
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("导出Objdat", GUILayout.Width(100)))
            {
                new Source2ScrObj().GenData(reader);
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("导出ObjData.cs", GUILayout.Width(100)))
            {
                new Source2ScrObj().GenCS(reader);
                AssetDatabase.Refresh();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("全部导出", GUILayout.Width(100)))
            {
                new Source2ScrObj().GenCS(reader);
                new Source2ScrObj().GenData(reader);
            }
        }
        /// <summary>
        /// 导出Json
        /// </summary>
        protected void ShowExportJson()
        {
            GUIStyle headStyle = GUIHelper.MakeHeader();
            GUILayout.Label("导出Json（支持大多数类型）", headStyle);
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("导出Json", GUILayout.Width(100)))
            {
                new Source2Json().GenData(reader);
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("导出JsonData.cs", GUILayout.Width(100)))
            {
                new Source2Json().GenCS(reader);
                AssetDatabase.Refresh();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("全部导出", GUILayout.Width(100)))
            {
                new Source2Json().GenData(reader);
                new Source2Json().GenCS(reader);
            }
        }
        /// <summary>
        /// 导出Protobuf
        /// </summary>
        protected void ShowExportProtobuf()
        {
            GUIStyle headStyle = GUIHelper.MakeHeader();
            GUILayout.Label("导出Protobuf(仅支持C#内置类型)", headStyle);
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("导出Protobuf", GUILayout.Width(100)))
            {
                new Source2Protobuf().GenData(reader);
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("导出ProtoData.cs", GUILayout.Width(100)))
            {
                new Source2Protobuf().GenCS(reader);
                AssetDatabase.Refresh();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("全部导出", GUILayout.Width(100)))
            {
                new Source2Protobuf().GenCS(reader);
                new Source2Protobuf().GenData(reader);
            }
        }
        /// <summary>
        /// 导出Pref
        /// </summary>
        protected void ShowExportPrf()
        {
            GUIStyle headStyle = GUIHelper.MakeHeader();
            GUILayout.Label("Export PrefData", headStyle);
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export PrefData", GUILayout.Width(100)))
            {
                new Source2PrefData().GenData(reader);
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("Export PrefData.cs", GUILayout.Width(100)))
            {
                new Source2PrefData().GenCS(reader);
                AssetDatabase.Refresh();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Export ALL", GUILayout.Width(100)))
            {
                new Source2PrefData().GenData(reader);
                new Source2PrefData().GenCS(reader);
            }
        }
    }

}
