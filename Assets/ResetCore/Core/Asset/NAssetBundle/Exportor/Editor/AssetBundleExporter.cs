using ResetCore.CodeDom;
using ResetCore.Util;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ResetCore.NAsset
{
    internal class AssetBundleExporter
    {
        /// <summary>
        /// 输出目标
        /// </summary>
        public BuildTarget currentTarget { get; set; }
        private readonly BuildTarget defTarget = BuildTarget.Android;
        /// <summary>
        /// 选项
        /// </summary>
        public BuildAssetBundleOptions options { get; set; }
        public readonly BuildAssetBundleOptions defOptions = BuildAssetBundleOptions.ChunkBasedCompression;
        /// <summary>
        /// 输出路径
        /// </summary>
        public string outputPath { get; set; }

        /// <summary>
        /// 输出路径
        /// </summary>
        public string overrideOutputPath { get; set; }

        public List<string> ignoreType = new List<string>()
        {
            ".cs", ".dll", ".manifest", ".asset", ".mdb"
        };

        public List<string> error { get; set; }
    
        public AssetBundleExporter()
        {
            outputPath = NAssetPaths.defOutputBundlePath;
            currentTarget = defTarget;
            options = BuildAssetBundleOptions.ChunkBasedCompression | 
                BuildAssetBundleOptions.DeterministicAssetBundle;
            error = new List<string>();
        }

        public void BuildAssetBundle()
        {
            RefreshAssetBundleDict();
            string path = outputPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            BuildPipeline.BuildAssetBundles(path,
                options
                , currentTarget);
            GenRList();
            GenBundleList();
            AssetDatabase.Refresh();
            GenResourcesFolderList();
            GenStreamingFolderList();
        }

        //每个bundle中包含的所有资源
        public Dictionary<string, List<string>> assetBundleDict { get; private set; }
        /// <summary>
        /// 生成包中资源
        /// </summary>
        public void RefreshAssetBundleDict()
        {
            assetBundleDict = new Dictionary<string, List<string>>();
            var allAssetPaths = AssetDatabase.GetAllAssetPaths();
            
            foreach (var assetPath in allAssetPaths)
            {
                if (IsIgnore(assetPath))
                    continue;

                var ai = AssetImporter.GetAtPath(assetPath);
                string abName = ai.assetBundleName;
                if (string.IsNullOrEmpty(abName))
                    abName = "none";

                if (!assetBundleDict.ContainsKey(abName))
                {
                    assetBundleDict.Add(abName, new List<string>());
                }
                if (!assetBundleDict[abName].Contains(assetPath))
                {
                    assetBundleDict[abName].Add(assetPath);
                    var deps = AssetDatabase.GetDependencies(assetPath);
                    foreach(var dep in deps)
                    {
                        var depai = AssetImporter.GetAtPath(dep);
                        if (string.IsNullOrEmpty(depai.assetBundleName) &&
                            !assetBundleDict[abName].Contains(dep) && !IsIgnore(dep))
                        {
                            assetBundleDict[abName].Add(dep + "(依赖未标记的资源)");
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 是否被忽略
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        private bool IsIgnore(string assetPath)
        {
            if (!assetPath.StartsWith("Assets")
                || string.IsNullOrEmpty(Path.GetExtension(assetPath)))
                return true;
            foreach(var end in ignoreType)
            {
                if (assetPath.EndsWith(end))
                {
                    return true;
                }
            }
            return false;
            
        }

        /// <summary>
        /// 生成资源列表类
        /// </summary>
        private void GenRList()
        {
            CodeGener gener = new CodeGener("ResetCore.NAsset", "R");
            foreach(var kvp in assetBundleDict)
            {
                if (kvp.Key == "none")
                    continue;
                foreach (var path in kvp.Value)
                {
                    string name = kvp.Key + "_" + FileEx.GetFileNameWithoutExtention(path);
                    string defName = name.Replace(" ", "_").Replace("/", "_");
                    string content = kvp.Key + "###" + FileEx.GetFileNameWithoutExtention(path);
                    gener.AddMemberField(typeof(string), defName, (field) =>
                    {
                        CodeVariableReferenceExpression fieldExpression = new CodeVariableReferenceExpression("\"" + content + "\"");
                        field.InitExpression = fieldExpression;
                    }, MemberAttributes.Static | MemberAttributes.Public);
                }
            }
            gener.GenCSharp(Path.Combine(Application.dataPath, "AssetBundle/Loader/temp"));
        }

        /// <summary>
        /// 生成Bundle列表类
        /// </summary>
        private void GenBundleList()
        {
            CodeGener gener = new CodeGener("ResetCore.NAsset", "Bundles");
            foreach (var kvp in assetBundleDict)
            {
                string name = kvp.Key;
                if (name == "none")
                    continue;
                string defName = name.Replace(" ", "_").Replace("/", "_");
                gener.AddMemberField(typeof(string), defName, (field) =>
                {
                    CodeVariableReferenceExpression fieldExpression = new CodeVariableReferenceExpression("\"" + name + "\"");
                    field.InitExpression = fieldExpression;
                }, MemberAttributes.Static | MemberAttributes.Public);
            }
            gener.GenCSharp(Path.Combine(Application.dataPath, "AssetBundle/Loader/temp"));
        }

        /// <summary>
        /// 生成资源列表
        /// </summary>
        public void GenResourcesFolderList()
        {
            PathEx.MakeDirectoryExist(NAssetPaths.resourcesListPath);
            string resourcesPath = PathEx.ConvertAssetPathToAbstractPath(PathConfig.assetResourcePath);
            DirectoryInfo dirInfo = new DirectoryInfo(resourcesPath);
            var files = dirInfo.GetFiles("*", SearchOption.AllDirectories);

            List<string> nameList = new List<string>();
            foreach (var file in files)
            {
                if (file.FullName.EndsWith(".meta"))
                    continue;
                nameList.Add(PathEx.MakePathStandard(file.FullName).Replace(resourcesPath, ""));
            }
            FileEx.SaveText(nameList.ConverToString(), NAssetPaths.resourcesListPath);
            AssetDatabase.Refresh();
        }
        /// <summary>
        /// 生成资源列表
        /// </summary>
        public void GenStreamingFolderList()
        {
            PathEx.MakeDirectoryExist(NAssetPaths.resourcesListPath);
            DirectoryInfo dirInfo = new DirectoryInfo(Application.streamingAssetsPath);
            var files = dirInfo.GetFiles("*", SearchOption.AllDirectories);

            List<string> nameList = new List<string>();
            foreach (var file in files)
            {
                if (file.FullName.EndsWith(".meta"))
                    continue;
                nameList.Add(PathEx.MakePathStandard(file.FullName).Replace(Application.streamingAssetsPath + "/", ""));
            }
            FileEx.SaveText(nameList.ConverToString(), NAssetPaths.streamingListPath);
            AssetDatabase.Refresh();
        }

    }

}
