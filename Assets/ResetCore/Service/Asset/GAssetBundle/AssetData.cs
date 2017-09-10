using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.GAsset
{
    public enum DownloadAssetBundlePriority
    {
        None = 0,
        Hight,
        Medium,
        Low
    }

    public enum DownloadAssetPriority
    {
        None = 0,
        Hight,
        Medium,
        Low
    }

    public delegate void OnAssetLoadComplete(string assetBundleName, string assetName, UnityEngine.Object asset);
    public delegate void OnAssetBundleLoadComplete(string assetBundleName, bool succeed);

    public enum AssetLoadType
    {
        LoadAsset,
        LoadAssetBundle,
        LoadFile
    }

    public enum CreateAssetBundleType
    {
        SyncCreate,
        AsyncCreateFromFile,
        AsyncCreateFromMemory,
        AsyncCreateFromWWW
    }

    public class AssetData
    {
        
        public string fileName { get; private set; }
        
        public string fileFullPath { get; private set; }

        public float process { get; set; }

        public string assetName { get; private set; }

        public System.Type assetType { get; private set; }

        public string message { get; private set; }

        public CreateAssetBundleType bundle;

        
    }

}
