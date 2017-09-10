using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Asset
{
    /// <summary>
    /// 通用Loader接口
    /// </summary>
    public interface IBundleLoader
    {
        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="priority"></param>
        /// <param name="progressAct"></param>
        /// <param name="callBack"></param>
        void LoadBundleAsyc(string bundleName,
            ThreadPriority priority = ThreadPriority.High,
            Action<float> progressAct = null, Action<AssetBundle> callBack = null);

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        AssetBundle LoadBundleSync(string bundleName);

        /// <summary>
        /// 是否已经加载
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        bool HasLoaded(string bundleName);

        /// <summary>
        /// 获取Bundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        AssetBundle GetBundle(string bundleName);

        /// <summary>
        /// 卸载Bundle
        /// </summary>
        /// <param name="name"></param>
        /// <param name="flag"></param>
        void UnloadBundle(string name, bool flag = true);

        /// <summary>
        /// 获取文本
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        TextAsset GetText(string bundleName, string resName);

        /// <summary>
        /// 获取贴图
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        Texture GetTexture(string bundleName, string resName);

        /// <summary>
        /// 获取Sprite
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="resName"></param>
        /// <param name="fatherResName"></param>
        /// <returns></returns>
        Sprite GetSprite(string bundleName, string resName, string fatherResName = null);

        /// <summary>
        /// 获取声音
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        AudioClip GetAudio(string bundleName, string resName);

        /// <summary>
        /// 获取材质
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        Material GetMaterial(string bundleName, string resName);

        /// <summary>
        /// 获取Shader
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        Shader GetShader(string bundleName, string resName);

        /// <summary>
        /// 获取GameObject
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        GameObject GetGameObject(string bundleName, string resName);
    }
}
