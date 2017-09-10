using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace ResetCore.PlatformHelper
{
    /// <summary>
    /// 安卓Menifest文件处理器
    /// </summary>
    public class AndroidManifestBuilder
    {
        /// <summary>
        /// 合并权限
        /// </summary>
        /// <param name="bas"></param>
        /// <param name="add"></param>
        public static void MergePermission(AndroidManifest bas, AndroidManifest add)
        {
            if (add.PermissionNodes.Count == 0)
                return;

            //XmlNode position = bas.LastPermissionNode != null ? bas.LastPermissionNode : null;
            XmlNode pre = AddComment(bas.ManifestNode, "***********************************来自" + add.Name + "的权限开始***********************************", bas.PermissionEndNode, false);

            XmlNode last = pre;

            foreach (XmlNode n in add.PermissionNodes)
            {
                XmlNode nd = bas.XML.ImportNode(n, true);

                if (bas.ContainsPermission(nd.OuterXml))
                {
                    string s = "【重复】" + nd.OuterXml;
                    s = s.Replace(" xmlns:android=\"http://schemas.android.com/apk/res/android\"", "");
                    nd = AddComment(bas.ManifestNode, s, last);
                }

                else
                    bas.ManifestNode.InsertAfter(nd, last);

                last = nd;
            }

            AddComment(bas.ManifestNode, "***********************************来自" + add.Name + "的权限结束***********************************", last);
        }

        /// <summary>
        /// 合并SDK
        /// </summary>
        /// <param name="bas"></param>
        /// <param name="add"></param>
        public static void MergeSDK(AndroidManifest bas, AndroidManifest add)
        {
            if (add.SDKNode == null)
                return;

            if (bas.SDKNode == null)
            {
                XmlNode node = bas.XML.ImportNode(add.SDKNode, true);
                bas.ManifestNode.InsertAfter(node, null);
            }
            else
            {
                XmlNode node = bas.XML.ImportNode(add.SDKNode, true);
                bas.ManifestNode.InsertAfter(node, bas.SDKNode.PreviousSibling);
                bas.ManifestNode.RemoveChild(bas.SDKNode);
            }
        }

        /// <summary>
        /// 合并特性
        /// </summary>
        /// <param name="bas"></param>
        /// <param name="add"></param>
        public static void MergeFeature(AndroidManifest bas, AndroidManifest add)
        {
            if (add.FeatureNodes.Count == 0)
                return;

            XmlNode pre = AddComment(bas.ManifestNode, "***********************************来自" + add.Name + "的特性开始***********************************", bas.FeatureEndNode, false);

            XmlNode last = pre;

            foreach (XmlNode n in add.FeatureNodes)
            {
                XmlNode nd = bas.XML.ImportNode(n, true);

                if (bas.ContainsFeature(nd.OuterXml))
                {
                    string s = "【重复】" + nd.OuterXml;
                    s = s.Replace(" xmlns:android=\"http://schemas.android.com/apk/res/android\"", "");
                    nd = AddComment(bas.ManifestNode, s, last);
                }

                else
                    bas.ManifestNode.InsertAfter(nd, last);

                last = nd;
            }

            AddComment(bas.ManifestNode, "***********************************来自" + add.Name + "的特性结束***********************************", last);
        }

        /// <summary>
        /// 合并屏幕
        /// </summary>
        /// <param name="bas"></param>
        /// <param name="add"></param>
        public static void MergeScreen(AndroidManifest bas, AndroidManifest add)
        {
            if (add.ScreenNode == null)
                return;

            if (bas.ScreenNode == null)
            {
                XmlNode node = bas.XML.ImportNode(add.ScreenNode, true);
                bas.ManifestNode.InsertAfter(node, null);
            }
            else
            {
                XmlNode node = bas.XML.ImportNode(add.ScreenNode, true);
                bas.ManifestNode.InsertAfter(node, bas.ScreenNode.PreviousSibling);
                bas.ManifestNode.RemoveChild(bas.ScreenNode);
            }
        }

        /// <summary>
        /// 合并Application
        /// </summary>
        /// <param name="bas"></param>
        /// <param name="add"></param>
        public static void MergeApplication(AndroidManifest bas, AndroidManifest add)
        {
            if (add.ApplicationNode == null)
                return;

            //合并Application的属性
            foreach (XmlAttribute attr in add.ApplicationNode.Attributes)
            {
                bool exist = false;
                foreach (XmlAttribute bttr in bas.ApplicationNode.Attributes)
                {
                    if (bttr.Name.ToLower() == attr.Name.ToLower() && bttr.NamespaceURI == attr.NamespaceURI)
                    {
                        exist = true;
                        bttr.Value = attr.Value;
                        break;
                    }
                }

                if (!exist)
                {
                    XmlAttribute at = bas.XML.CreateAttribute(attr.Name, attr.NamespaceURI);
                    at.Value = attr.Value;
                    bas.ApplicationNode.Attributes.Append(at);
                }

            }

            if (bas.ApplicationNode == null)
            {
                XmlNode nd = bas.XML.ImportNode(add.ApplicationNode, true);
                bas.ManifestNode.InsertAfter(nd, null);
            }
            else if (add.ApplicationChildren != null && add.ApplicationChildren.Count > 0)
            {
                AddComment(bas.ApplicationNode, "***********************************来自" + add.Name + "的界面开始***********************************");
                foreach (XmlNode nn in add.ApplicationChildren)
                {
                    XmlNode nd = bas.XML.ImportNode(nn, true);
                    bas.ApplicationNode.AppendChild(nd);
                }
                AddComment(bas.ApplicationNode, "***********************************来自" + add.Name + "的界面结束***********************************");
            }
        }

        /// <summary>
        /// 合并
        /// </summary>
        /// <param name="baseFile"></param>
        /// <param name="addFile"></param>
        public static void MergeAndroidMenifest(string baseFile, string addFile)
        {
            AndroidManifest b = new AndroidManifest(baseFile);
            AndroidManifest a = new AndroidManifest(addFile);

            IList<XmlNode> nodes = b.UnknownNodes;
            MergePermission(b, a);
            MergeSDK(b, a);
            MergeFeature(b, a);
            MergeScreen(b, a);
            MergeApplication(b, a);

            b.XML.Save("merge.xml");
        }

        /// <summary>
        /// 合并文件
        /// </summary>
        /// <param name="b"></param>
        /// <param name="addFile"></param>
        public static void MergeAndroidMenifest(AndroidManifest b, string addFile)
        {
            AndroidManifest a = new AndroidManifest(addFile);

            if (!a.IsValid)
            {
                UnityEngine.Debug.LogError("file " + addFile + " format error ");
                return;
            }


            MergePermission(b, a);
            MergeSDK(b, a);
            MergeFeature(b, a);
            MergeScreen(b, a);
            MergeApplication(b, a);
        }

        /// <summary>
        /// 添加注释
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public static XmlNode AddComment(XmlNode parent, string name, XmlNode position = null, bool after = true)
        {
            XmlComment com = parent.OwnerDocument.CreateComment(name);

            if (position == null)
                parent.AppendChild(com);
            else
            {
                if (after)
                    parent.InsertAfter(com, position);
                else
                    parent.InsertBefore(com, position);
            }

            return com;
        }

        /// <summary>
        /// 参数替换
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="name">参数名</param>
        /// <param name="val">参数值</param>
        public static void ReplaceParam(string file, string name, string val)
        {
            if (!File.Exists(file))
                return;

            string content = File.ReadAllText(file);
            content = content.Replace("{" + name + "}", val);

            File.Delete(file);
            File.WriteAllText(file, content, Encoding.UTF8);
        }
    }


}
