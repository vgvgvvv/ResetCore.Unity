using ResetCore.Util;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace ResetCore.NAsset
{
    public class NAssetPaths
    {
        public static string defBundleFolderName = "AssetBundles";
        public static readonly string defOutputBundlePath = 
            PathEx.Combine(
                "Assets/StreamingAssets"
                , "AssetBundles");

        public static readonly string defDownloadBundlePath = PathEx.Combine(Application.persistentDataPath, "AssetBundles");

        //Resources资源表
        public static string resourcesListPath
        {
            get
            {
                return PathEx.ConvertAssetPathToAbstractPath(
                    Path.Combine(PathConfig.assetResourcePath, "ResourcesList.txt"));
            }
        }
        //流媒体资源表
        public static string streamingListPath
        {
            get
            {
                return PathEx.ConvertAssetPathToAbstractPath(
                    Path.Combine(PathConfig.assetResourcePath, "StreamingList.txt"));
            }
        }
            
    }
}
