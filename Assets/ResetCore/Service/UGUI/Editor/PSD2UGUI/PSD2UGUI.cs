//using UnityEngine;
//using System.Collections;
//using UnityEditor;
//using System.Xml.Linq;
//using ResetCore.Util;
//using System.Collections.Generic;
//using System;
//using System.IO;
//using UnityEngine.UI;

//namespace ResetCore.UGUI
//{
//    public class PSD2UGUI
//    {
//        enum LayerType
//        {
//            Null, Image, Button, Text, Root
//        }
//        private static Dictionary<LayerType, string> layerTypePrefix = new Dictionary<LayerType, string>()
//        {
//            {LayerType.Image, "img:"},
//            {LayerType.Button, "btn:"},
//            {LayerType.Text, "txt:"},
//            {LayerType.Root, "root:"},
//        };

//        private static string psdFilePath = "";
//        private static string uiName = "";
//        public static void PsdFile2UGUI()
//        {
//            psdFilePath = EditorUtility.OpenFolderPanel("选择导出的Psd文件", psdFilePath, "");
//            uiName = FileEx.GetDirectoryName(psdFilePath);
//            XDocument xDoc = XDocument.Load(PathEx.Combine(psdFilePath, "config.xml"));
//        }

//        private static void CreateUGUI(XDocument xDoc)
//        {
//            var root = xDoc.Root.Element("Layers").Element("Layer");
//            HandleLayer(root);
//        }
//        private static bool hasRoot = false;
//        private static void HandleLayer(XElement layerEle, Transform parent)
//        {
//            switch (GetLayerTypeByName(layerEle.Name.LocalName))
//            {
//                case LayerType.Null:
//                    {
//                        var layers = layerEle.Elements("Layer");
//                        foreach (var layer in layers)
//                        {
//                            HandleLayer(layer, parent);
//                        }
//                    }
//                    break;
//                case LayerType.Image:
//                    {
//                        HandleImage(layerEle);
//                    }
//                    break;
//                case LayerType.Button:
//                    {
//                        HandleButton(layerEle);
//                    }
//                    break;
//                case LayerType.Text:
//                    {
//                        HandleText(layerEle);
//                    }
//                    break;
//                case LayerType.Root:
//                    {
//                        HandleRoot(layerEle);
//                    }
//                    break;
//                default:
//                    {
//                        Debug.logger.LogError("未知错误", "不存在键值");
//                    }
//                    break;
//            }
//        }

//        private static void HandleImage(XElement layer, Transform parent)
//        {

//        }

//        private static void HandleButton(XElement layer)
//        {

//        }

//        private static void HandleText(XElement layer)
//        {

//        }

//        private static void HandleRoot(XElement layer)
//        {
//            hasRoot = true;
//            GameObject root = new GameObject(uiName, typeof(Image));
//        }


//        private static LayerType GetLayerTypeByName(string name)
//        {
//            foreach(KeyValuePair<LayerType, string> kvp in layerTypePrefix)
//            {
//                if (name.StartsWith(kvp.Value))
//                {
//                    return kvp.Key;
//                }
//            }
//            return LayerType.Null;
//        }
//    }
//}
