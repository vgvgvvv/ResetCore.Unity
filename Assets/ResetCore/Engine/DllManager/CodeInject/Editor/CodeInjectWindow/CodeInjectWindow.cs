using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ResetCore.Util;
using ResetCore.Asset;
using System.Xml.Linq;

namespace ResetCore.ReAssembly
{
    public class CodeInjectWindow : EditorWindow
    {
        private static CodeInjectWindow window;

        [MenuItem("Tools/DllManager/CodeInjector ")]
        public static void OpenEditorWindow()
        {
            window = EditorWindow.GetWindow<CodeInjectWindow>(true, "代码注入设置");
            window.Show();
        }

        private enum InjectPlatForm
        {
            Common,
            Android,
            IOS,
            StandAlone
        }

        //当前显示的平台信息
        private InjectPlatForm currentPlatform = InjectPlatForm.Common;
        void OnGUI()
        {
            ShowSelectPlatform();
            GUILayout.Space(20);
            ShowInjectInfo(currentPlatform);
            GUILayout.Space(20);
            ToolBag();
        }

        //展示选择平台
        private void ShowSelectPlatform()
        {
            EditorGUILayout.LabelField("Select Platform", GUIHelper.MakeHeader());
            EditorGUILayout.LabelField("Current Platform: " + currentPlatform.ToString(), GUIHelper.MakeHeader());
            GUILayout.BeginHorizontal();
            foreach (InjectPlatForm platform in System.Enum.GetValues(typeof(InjectPlatForm)))
            {
                if (GUILayout.Button(platform.ToString(), GUILayout.Width(150)))
                {
                    currentPlatform = platform;
                }
            }
            GUILayout.EndHorizontal();
        }
        //展示注入信息
        private void ShowInjectInfo(InjectPlatForm currentPlatform)
        {
            EditorGUILayout.LabelField("Platform Info", GUIHelper.MakeHeader());
            var infoXmlTextAsset = EditorResources.GetAsset<TextAsset>("InjectDll", "CodeInject");
            XDocument xDoc = XDocument.Parse(infoXmlTextAsset.text);
            var infoRoot = xDoc.Root.Element(currentPlatform.ToString());
            
            foreach(var infoElement in infoRoot.Elements())
            {
                string attrbutes = infoElement.Element("injectAttr").Value;
                if(!string.IsNullOrEmpty(attrbutes))
                {
                    var dllEles = new List<XElement>(infoElement.Elements("dll"));
                    foreach (var dllEle in dllEles)
                    {
                        EditorGUILayout.LabelField("Effect Dll：" + dllEle.Name.LocalName);
                    }
                    EditorGUILayout.LabelField("To Inject：" + attrbutes);
                }
            }
        }
        //应用注入
        private void ToolBag()
        {
            EditorGUILayout.LabelField("Tool Bag");
            if (GUILayout.Button("Test Inject", GUILayout.Width(150)))
            {
            }
        }

    }
}
