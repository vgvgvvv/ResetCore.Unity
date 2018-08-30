using ResetCore.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ResetCore.NAsset
{
    public class AssetLoader
    {
        /// <summary>
        /// 已经加载的Bundle
        /// </summary>
        private static readonly Dictionary<string, AssetBundle> loadedBundle = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// 未加载的Bundle
        /// </summary>
        private static readonly Dictionary<string, BundleResources> bundleResources = new Dictionary<string, BundleResources>();

        /// <summary>
        /// 异步加载Bundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="priority"></param>
        /// <param name="progressAct"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static IEnumerator LoadBundleAsyc(string bundleName, 
            ThreadPriority priority = ThreadPriority.High, 
            Action<float> progressAct = null, Action<AssetBundle> callBack = null)
        {
            if (HasLoaded(bundleName))
            {
                if(callBack != null)
                    callBack(GetBundle(bundleName));
                yield break;
            }

            yield return DownloadAsyc(bundleName, priority, progressAct, (bundle)=> {
                loadedBundle.Add(bundleName, bundle);
                loadedBundle[bundleName].name = bundleName;
                bundleResources.Add(bundleName, new BundleResources(bundle));
                if (callBack != null)
                    callBack(bundle);
                Debug.Log("loaded " + bundleName);
            });

        }

        /// <summary>
        /// 同步加载Bundle
        /// </summary>
        /// <param name="bundleName"></param>
        public static AssetBundle LoadBundleSync(string bundleName)
        {
            if (HasLoaded(bundleName))
            {
                return GetBundle(bundleName);
            }
            var bundle = DownloadSync(bundleName);
            loadedBundle.Add(bundleName, bundle);
            loadedBundle[bundleName].name = bundleName;
            bundleResources.Add(bundleName, new BundleResources(bundle));
            Debug.Log("loaded " + bundleName);
            return bundle;
        }

        private static IEnumerator DownloadAsyc(string bundleName, ThreadPriority priority, Action<float> progressAct, Action<AssetBundle> callback)
        {
            string path = PathEx.Combine(NAssetPaths.defBundleFolderName, bundleName);
            string downloadPath = "";

            bool isPersistentFileExist = FileManager.IsPersistentFileExist(path);
            bool isStreamFileExist = FileManager.IsStreamFileExist(path);
            bool isResourcesFileExist = FileManager.IsResourceFileExist(path);

            //寻找顺序：沙盒目录、Resources、StreamingAssets
            if(isPersistentFileExist)
            {
                downloadPath = FileManager.PersistentFileWWWPath(path);
                yield return PersistentDownloadAsync(downloadPath, priority, progressAct, callback);
            }
            else if (isResourcesFileExist)
            {
                yield return ResourcesDownloadAsync(path, priority, progressAct, callback);
            }
            else if (isStreamFileExist)
            {
                downloadPath = FileManager.StreamFileWWWPath(path);
                yield return StreamingDownloadAsync(downloadPath, priority, progressAct, callback);
            }
            else
            {
                Debug.LogError("未找到文件 " + bundleName);
            }
        }

        private static AssetBundle DownloadSync(string bundleName)
        {
            string path = PathEx.Combine(NAssetPaths.defBundleFolderName, bundleName);
            string downloadPath = "";

            bool isPersistentFileExist = FileManager.IsPersistentFileExist(path);
            bool isStreamFileExist = FileManager.IsStreamFileExist(path);
            bool isResourcesFileExist = FileManager.IsResourceFileExist(path);
            //寻找顺序：沙盒目录、Resources、StreamingAssets
            if (isPersistentFileExist)
            {
                downloadPath = FileManager.PersistentFileWWWPath(path);
                return PersistentDownloadSync(downloadPath);
            }
            else if (isResourcesFileExist)
            {
                return ResourcesDownloadSync(path);
            }
            else if (isStreamFileExist)
            {
                downloadPath = FileManager.StreamFileWWWPath(path);
                return StreamingDownloadSync(downloadPath);
            }
            else
            {
                Debug.LogError("未找到文件 " + bundleName);
                return null;
            }
        }

        /// <summary>
        /// 使用沙盒下载
        /// </summary>
        /// <param name="downloadPath"></param>
        /// <param name="priority"></param>
        /// <param name="progressAct"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator PersistentDownloadAsync(string downloadPath, 
            ThreadPriority priority, 
            Action<float> progressAct, 
            Action<AssetBundle> callback)
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(downloadPath);
            request.priority = (int)priority;

            while (!request.isDone)
            {
                if (progressAct != null)
                    progressAct(request.progress);

                yield return null;
            }

            if (request.assetBundle == null)
            {
                Debug.LogError(downloadPath + "加载失败");
                yield break;
            }

            if (callback != null)
                callback(request.assetBundle);

        }

        /// <summary>
        /// 同步加载沙盒目录
        /// </summary>
        /// <param name="downloadPath"></param>
        /// <returns></returns>
        public static AssetBundle PersistentDownloadSync(string downloadPath)
        {
            return AssetBundle.LoadFromFile(downloadPath);
        }

        /// <summary>
        /// 使用Resources下载
        /// </summary>
        /// <param name="downloadPath"></param>
        /// <param name="priority"></param>
        /// <param name="progressAct"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator ResourcesDownloadAsync(string downloadPath,
            ThreadPriority priority,
            Action<float> progressAct,
            Action<AssetBundle> callback)
        {
            var request = Resources.LoadAsync<TextAsset>(downloadPath);
            request.priority = (int)priority;

            while (!request.isDone)
            {
                if (progressAct != null)
                    progressAct(request.progress);

                yield return null;
            }

            var asset = request.asset as TextAsset;
            if (asset == null)
            {
                Debug.LogError("加载失败");
                yield break;
            }

            var adRequest = AssetBundle.LoadFromMemoryAsync(asset.bytes);
            adRequest.priority = (int)priority;

            while (!adRequest.isDone)
            {
                if (progressAct != null)
                    progressAct(request.progress);

                yield return null;
            }

            if (adRequest.assetBundle == null)
            {
                Debug.LogError("加载失败");
                yield break;
            }

            if (callback != null)
                callback(adRequest.assetBundle);

            Resources.UnloadAsset(asset);
        }

        /// <summary>
        /// 同步从Resource中加载
        /// </summary>
        /// <param name="downloadPath"></param>
        /// <returns></returns>
        public static AssetBundle ResourcesDownloadSync(string downloadPath)
        {
            var textAsset = Resources.Load<TextAsset>(downloadPath);
            if (textAsset == null)
            {
                Debug.LogError(downloadPath + " 从Resources中加载失败");
                return null;
            }
            var asset = AssetBundle.LoadFromMemory(textAsset.bytes);
            Resources.UnloadAsset(textAsset);
            return asset;
        }

        /// <summary>
        /// 使用流媒体下载
        /// </summary>
        /// <param name="downloadPath"></param>
        /// <param name="priority"></param>
        /// <param name="progressAct"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator StreamingDownloadAsync(string downloadPath,
            ThreadPriority priority,
            Action<float> progressAct,
            Action<AssetBundle> callback)
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(downloadPath);
            request.priority = (int)priority;

            while (!request.isDone)
            {
                if (progressAct != null)
                    progressAct(request.progress);

                yield return null;
            }

            if (request.assetBundle == null)
            {
                Debug.LogError(downloadPath + "加载失败");
                yield break;
            }

            if (callback != null)
                callback(request.assetBundle);
        }

        /// <summary>
        /// 流媒体同步下载
        /// </summary>
        /// <param name="downloadPath"></param>
        /// <returns></returns>
        public static AssetBundle StreamingDownloadSync(string downloadPath)
        {
            return AssetBundle.LoadFromFile(downloadPath);
        }

        /// <summary>
        /// 是否已经被加载
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>    
        public static bool HasLoaded(string name)
        {
            if (loadedBundle.ContainsKey(name))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取Bundle
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AssetBundle GetBundle(string name)
        {
            if(HasLoaded(name))
                return loadedBundle[name];
            return LoadBundleSync(name);
        }


        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="name"></param>
        /// <param name="flag"></param>
        public static void UnloadBundle(string name, bool flag = true)
        {
            AssetBundle bundle = GetBundle(name);

            loadedBundle.Remove(name);

            if(bundle == null)
            {
                Debug.LogWarning("未找到Bundle");
                return;
            }

            var bundleResource = bundleResources[name];
            if(bundleResource != null)
            {
                bundleResource.Reset();
                bundleResources.Remove(name);
            }

            bundle.Unload(flag);
        }

        /// <summary>
        /// 获取文本
        /// </summary>
        /// <param name="bundleName">资源包路径</param>
        /// <param name="resName">文本名</param>
        /// <returns>文本对象</returns>
        public static TextAsset GetText(string bundleName, string resName)
        {
#if UNITY_EDITOR
            var res = GetEditorAsset<TextAsset>(resName, bundleName);
            if (res != null)
                return res;
#endif
            BundleResources resource = bundleResources[bundleName] as BundleResources;
            return resource != null ? resource.GetText(resName) : null;
        }
        public static TextAsset GetTextByR(string R)
        {
            var Rl = R.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            return GetText(Rl[0], Rl[1]);
        }

        /// <summary>
        /// 获取Texture
        /// </summary>
        /// <param name="bundleName">资源包路径</param>
        /// <param name="resName">文本名</param>
        /// <returns>Texture</returns>
        public static Texture GetTexture(string bundleName, string resName)
        {
#if UNITY_EDITOR
            var res = GetEditorAsset<Texture>(resName, bundleName);
            if (res != null)
                return res;
#endif
            BundleResources resource = bundleResources[bundleName] as BundleResources;
            return resource != null ? resource.GetTexture(resName) : null;
        }
        public static Texture GetTextureByR(string R)
        {
            var Rl = R.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            return GetTexture(Rl[0], Rl[1]);
        }

        /// <summary>
        /// 获取Sprite
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="resName"></param>
        /// <param name="fatherResName"></param>
        /// <returns></returns>
        public static Sprite GetSprite(string bundleName, string resName, string fatherResName = null)
        {
#if UNITY_EDITOR
            if (fatherResName != null)
            {
                var res = GetAllEditorAsset<Sprite>(fatherResName, bundleName, resName);
                if (res != null)
                    return res;
            }
            else
            {
                var res = GetEditorAsset<Sprite>(resName, bundleName);
                if (res != null)
                    return res;
            }
#endif
            BundleResources resource = bundleResources[bundleName] as BundleResources;
            return resource != null ? resource.GetSprite(resName, fatherResName) : null;
        }

        public static Sprite GetSpriteByR(string R)
        {
            var Rl = R.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            return GetSprite(Rl[0], Rl[1]);
        }

        /// <summary>
        /// 获取音频
        /// </summary>
        /// <param name="bundleName">资源包路径</param>
        /// <param name="resName">音频名</param>
        /// <returns>音频对象</returns>
        public static AudioClip GetAudio(string bundleName, string resName)
        {
#if UNITY_EDITOR
            var res = GetEditorAsset<AudioClip>(resName, bundleName);
            if (res != null)
                return res;
#endif
            BundleResources resource = bundleResources[bundleName] as BundleResources;
            return resource != null ? resource.GetAudio(resName) : null;
        }
        public static AudioClip GetAudioByR(string R)
        {
            var Rl = R.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            return GetAudio(Rl[0], Rl[1]);
        }

        /// <summary>
        /// 获取材质
        /// </summary>
        /// <param name="bundleName">资源包路径</param>
        /// <param name="resName">材质名</param>
        /// <returns>材质对象</returns>
        public static Material GetMaterial(string bundleName, string resName)
        {
#if UNITY_EDITOR
            var res = GetEditorAsset<Material>(resName, bundleName);
            if (res != null)
                return res;
#endif
            BundleResources resource = bundleResources[bundleName] as BundleResources;
            return resource != null ? resource.GetMaterial(resName) : null;
        }
        public static Material GetMaterialByR(string R)
        {
            var Rl = R.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            return GetMaterial(Rl[0], Rl[1]);
        }

        /// <summary>
        /// 获取Shader
        /// </summary>
        /// <param name="bundleName">资源包路径</param>
        /// <param name="resName">Shader名</param>
        /// <returns>Shader对象</returns>
        public static Shader GetShader(string bundleName, string resName)
        {
#if UNITY_EDITOR
            var res = GetEditorAsset<Shader>(resName, bundleName);
            if (res != null)
                return res;
#endif
            BundleResources resource = bundleResources[bundleName] as BundleResources;
            return resource != null ? resource.GetShader(resName) : null;
        }

        public static Shader GetShaderByR(string R)
        {
            var Rl = R.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            return GetShader(Rl[0], Rl[1]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public static GameObject GetGameObject(string bundleName, string resName)
        {
#if UNITY_EDITOR
            var res = GetEditorAsset<GameObject>(resName, bundleName);
            if (res != null)
                return res;
#endif
            BundleResources resource = bundleResources[bundleName] as BundleResources;
            return resource != null ? resource.GetGameObject(resName) : null;
        }
        /// <summary>
        /// 通过R来获取GameObject
        /// </summary>
        /// <param name="R"></param>
        /// <returns></returns>
        public static GameObject GetGameObjectByR(string R)
        {
            var Rl = R.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            return GetGameObject(Rl[0], Rl[1]);
        }

        public static ScriptableObject GetScriptableObject(string bundleName, string resName)
        {
#if UNITY_EDITOR
            var res = GetEditorAsset<ScriptableObject>(resName, bundleName);
            if (res != null)
                return res;
#endif
            BundleResources resource = bundleResources[bundleName] as BundleResources;
            return resource != null ? resource.GetScriptableObject(resName) : null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <param name="fileNameFilter">文件名过滤器</param>
        /// <param name="pathFilter">路径过滤器</param>
        /// <returns></returns>
        public static List<string> GetAssetPaths(string fileNameFilter)
        {
            string[] guids = AssetDatabase.FindAssets(fileNameFilter);
            List<string> paths = new List<string>();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == fileNameFilter)
                    paths.Add(path);
            }
            return paths;
        }


        /// <summary>
        /// 获取资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="fileNameFilter">文件名过滤器</param>
        /// <param name="pathFilter">路径过滤器</param>
        /// <returns></returns>
        public static T GetEditorAsset<T>(string fileNameFilter, string assetBundleName) where T : UnityEngine.Object
        {
            List<string> fullPath = GetAssetPaths(fileNameFilter);

            if (fullPath.Count == 0)
                return null;

            if (fullPath.Count == 1)
            {
                return AssetDatabase.LoadAssetAtPath(fullPath[0], typeof(T)) as T;
            }

            string abName = assetBundleName;
            foreach (string path in fullPath)
            {
                AssetImporter ai = AssetImporter.GetAtPath(path);
                if (ai.assetBundleName == abName)
                {
                    return AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                }
            }
            foreach (string path in fullPath)
            {
                Debug.LogError("有重名资源" + path + "以及其他相同名字的资源， 请重命名或者确定其Bundle包");
            }
            return AssetDatabase.LoadAssetAtPath(fullPath[0], typeof(T)) as T;
        }

        /// <summary>
        /// 获取所有的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileNameFilter"></param>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        public static T GetAllEditorAsset<T>(string fileNameFilter, string assetBundleName, string childName) where T : UnityEngine.Object
        {
            List<string> fullPath = GetAssetPaths(fileNameFilter);

            if (fullPath.Count == 0)
            {
                return null;
            }


            if (fullPath.Count == 1)
            {
                return AssetDatabase.LoadAllAssetsAtPath(fullPath[0]) as T;
            }

            string abName = assetBundleName.Replace('\\', '/').Substring(assetBundleName.LastIndexOf('/') + 1);
            foreach (string path in fullPath)
            {
                AssetImporter ai = AssetImporter.GetAtPath(path);
                if (ai.assetBundleName == abName)
                {
                    var reses = AssetDatabase.LoadAllAssetsAtPath(path);
                    foreach (var r in reses)
                    {
                        if (r.name == childName)
                        {
                            return r as T;
                        }
                    }
                }
            }
            foreach (string path in fullPath)
            {
                Debug.LogError("有重名资源" + path + "以及其他相同名字的资源， 请重命名或者确定其Bundle包");
            }
            var res = AssetDatabase.LoadAllAssetsAtPath(fullPath[0]) as T;
            return res;
        }
#endif

    }

}
