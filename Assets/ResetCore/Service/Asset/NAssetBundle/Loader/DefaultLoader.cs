using System;
using System.Collections;
using System.Collections.Generic;
using ResetCore.Asset;
using UnityEngine;

namespace ResetCore.NAsset
{
    public class DefaultLoader : IBundleLoader
    {
        public AudioClip GetAudio(string bundleName, string resName)
        {
            return AssetLoader.GetAudio(bundleName, resName);
        }

        public AssetBundle GetBundle(string bundleName)
        {
            return AssetLoader.GetBundle(bundleName);
        }

        public GameObject GetGameObject(string bundleName, string resName)
        {
            return AssetLoader.GetGameObject(bundleName, resName);
        }

        public Material GetMaterial(string bundleName, string resName)
        {
            return AssetLoader.GetMaterial(bundleName, resName);
        }

        public Shader GetShader(string bundleName, string resName)
        {
            return AssetLoader.GetShader(bundleName, resName);
        }

        public Sprite GetSprite(string bundleName, string resName, string fatherResName = null)
        {
            return AssetLoader.GetSprite(bundleName, resName, fatherResName);
        }

        public TextAsset GetText(string bundleName, string resName)
        {
            return AssetLoader.GetText(bundleName, resName);
        }

        public Texture GetTexture(string bundleName, string resName)
        {
            return AssetLoader.GetTexture(bundleName, resName);
        }

        public bool HasLoaded(string bundleName)
        {
            return AssetLoader.HasLoaded(bundleName);
        }

        public void LoadBundleAsyc(string bundleName, ThreadPriority priority = ThreadPriority.High, Action<float> progressAct = null, Action<AssetBundle> callBack = null)
        {
            AssetLoader.LoadBundleAsyc(bundleName, priority, progressAct, callBack);
        }

        public AssetBundle LoadBundleSync(string bundleName)
        {
            return AssetLoader.LoadBundleSync(bundleName);
        }

        public void UnloadBundle(string name, bool flag = true)
        {
            AssetLoader.UnloadBundle(name, flag);
        }
    }


}
