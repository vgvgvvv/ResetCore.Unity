using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace ResetCore.PlatformHelper
{
    public class AndroidManifest
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;

        /// <summary>
        /// XML对象
        /// </summary>
        public XmlDocument XML = new XmlDocument();

        /// <summary>
        /// 格式是否正确
        /// </summary>
        private bool valid = false;

        /// <summary>
        /// 格式是否正确
        /// </summary>
        public bool IsValid
        {
            get { return valid; }
        }

        /// <summary>
        /// Menifest节点
        /// </summary>
        public XmlNode ManifestNode
        {
            get { return findNode(XML, "manifest"); }
        }

        /// <summary>
        /// 获取Application节点
        /// </summary>
        public XmlNode ApplicationNode
        {
            get { return ManifestNode != null ? findNode(ManifestNode, "application") : null; }
        }

        /// <summary>
        /// 获取Application子节点
        /// </summary>
        public IList<XmlNode> ApplicationChildren
        {
            get { return ApplicationNode != null ? allChildren(ApplicationNode) : null; }
        }

        /// <summary>
        /// 获取所有权限节点
        /// </summary>
        public IList<XmlNode> PermissionNodes
        {
            get { return ManifestNode != null ? findIncludeNodes(ManifestNode, new List<string>() { "uses-permission", "permission" }) : null; }
        }

        /// <summary>
        /// 获取最后一个权限节点
        /// </summary>
        public XmlNode LastPermissionNode
        {
            get { return PermissionNodes != null && PermissionNodes.Count > 0 ? PermissionNodes[PermissionNodes.Count - 1] : null; }
        }

        /// <summary>
        /// 权限结束节点
        /// </summary>
        public XmlNode PermissionEndNode
        {
            get { return findComment(ManifestNode, "权限结束"); }
        }

        /// <summary>
        /// SDK节点
        /// </summary>
        public XmlNode SDKNode
        {
            get { return ManifestNode != null ? findNode(ManifestNode, "uses-sdk", false) : null; }
        }

        /// <summary>
        /// 支持屏幕尺寸节点
        /// </summary>
        public XmlNode ScreenNode
        {
            get { return ManifestNode != null ? findNode(ManifestNode, "supports-screens", false) : null; }
        }

        /// <summary>
        /// 特性节点
        /// </summary>
        public IList<XmlNode> FeatureNodes
        {
            get { return ManifestNode != null ? findNodes(ManifestNode, "uses-feature") : null; }
        }

        public XmlNode LastFeatureNode
        {
            get { return FeatureNodes != null && FeatureNodes.Count > 0 ? FeatureNodes[FeatureNodes.Count - 1] : null; }
        }

        public XmlNode FeatureEndNode
        {
            get { return findComment(ManifestNode, "特性结束"); }
        }

        /// <summary>
        /// 其他未知节点
        /// </summary>
        public IList<XmlNode> UnknownNodes
        {
            get
            {
                IList<string> names = new List<string>() {
                "application", "uses-permission" , "permission", "uses-sdk", "supports-screens", "uses-feature"};

                return findExcludeNodes(ManifestNode, names);
            }
        }

        public AndroidManifest(string file)
        {
            FileInfo info = new FileInfo(file);
            if (!info.Exists)
                return;

            Name = info.Name;
            loadXml(info.FullName);
        }

        public bool ContainsPermission(string permission)
        {
            foreach (XmlNode node in PermissionNodes)
            {
                if (node.OuterXml.ToLower() == permission.ToLower())
                    return true;
            }

            return false;
        }

        public bool ContainsFeature(string feature)
        {
            foreach (XmlNode node in FeatureNodes)
            {
                if (node.OuterXml.ToLower() == feature.ToLower())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 加载xml文件
        /// </summary>
        /// <param name="file">文件名</param>
        private void loadXml(string file)
        {
            try
            {
                XML = new XmlDocument();
                XML.Load(file);
            }
            catch (Exception exp)
            {
                UnityEngine.Debug.LogError("parse xml exception : " + exp.Message);
                valid = false;
                return;
            }

            if (ManifestNode == null && ApplicationNode == null)
                valid = false;
            else
                valid = true;
        }

        /// <summary>
        /// 寻找单个节点
        /// </summary>
        /// <param name="root">xml节点对象</param>
        /// <param name="name">节点名</param>
        /// <param name="asc">是否按顺序查找</param>
        /// <returns>单个节点</returns>
        private XmlNode findNode(XmlNode root, string name, bool asc = true)
        {
            List<XmlNode> nodes = findIncludeNodes(root, new List<string> { name });

            if (nodes.Count == 0)
                return null;

            if (asc)
                return nodes[0];
            else
                return nodes[nodes.Count - 1];
        }

        /// <summary>
        /// 寻找注释节点
        /// </summary>
        /// <param name="root">根节点</param>
        /// <param name="name">注释内容</param>
        /// <returns>注释节点</returns>
        private XmlNode findComment(XmlNode root, string name)
        {
            if (root == null)
                return null;

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node is XmlComment && node.Value != null && node.Value.ToLower().Trim() == name.ToLower())
                    return node;
            }

            return null;
        }

        /// <summary>
        /// 寻找节点列表
        /// </summary>
        /// <param name="root">xml节点对象</param>
        /// <param name="name">节点名</param>
        /// <returns>节点列表</returns>
        private List<XmlNode> findNodes(XmlNode root, string name)
        {
            return findIncludeNodes(root, new List<string> { name });
        }

        /// <summary>
        /// 寻找所有名称匹配的节点
        /// </summary>
        /// <param name="root">根节点</param>
        /// <param name="names">名称</param>
        /// <returns>节点列表</returns>
        private List<XmlNode> findIncludeNodes(XmlNode root, IList<string> names)
        {
            List<XmlNode> nodes = new List<XmlNode>();

            foreach (XmlNode node in root.ChildNodes)
            {
                string n = node.Name.ToLower();

                if (names.Contains(n))
                    nodes.Add(node);
            }

            return nodes;
        }

        /// <summary>
        /// 寻找所有名称不匹配的节点
        /// </summary>
        /// <param name="root">根节点</param>
        /// <param name="names">名称列表</param>
        /// <returns>节点列表</returns>
        private List<XmlNode> findExcludeNodes(XmlNode root, IList<string> names)
        {
            List<XmlNode> nodes = new List<XmlNode>();

            foreach (XmlNode node in root.ChildNodes)
            {
                string n = node.Name.ToLower();

                if (!names.Contains(n))
                    nodes.Add(node);
            }

            return nodes;
        }

        /// <summary>
        /// 获取全部子节点
        /// </summary>
        /// <param name="root">xml节点对象</param>
        /// <returns>节点列表</returns>
        private List<XmlNode> allChildren(XmlNode root)
        {
            List<XmlNode> nodes = new List<XmlNode>();

            foreach (XmlNode node in root.ChildNodes)
                nodes.Add(node);

            return nodes;
        }
    }


}
