using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace ResetCore.Util
{
    public class LoadSceneManager : Singleton<LoadSceneManager>
    {
        public bool isLoading { get; private set; }

        public override void Init()
        {
            isLoading = false;
        }

       
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="loadedAct">加载行为回调</param>
        /// <param name="progressAct">加载进度回调</param>
        public void LoadScene(string sceneName, System.Action<bool> loadedAct = null, System.Action<float> progressAct = null)
        {
            if (isLoading == false)
            {
                CoroutineTaskManager.Instance.AddTask(
                new CoroutineTaskManager.CoroutineTask("LoadScene" + sceneName, DoLoadScene(sceneName), loadedAct));
                CoroutineTaskManager.Instance.AddTask(
                    new CoroutineTaskManager.CoroutineTask("LoadSceneProgress" + sceneName, DoLoadSceneProgress(progressAct)));
            }
            else
            {
                Debug.unityLogger.LogError("加载场景错误", "正在加载其他场景无法加载新场景");
            }
        }

        private AsyncOperation operation;
        private IEnumerator DoLoadScene(string sceneName)
        {
            isLoading = true;
            yield return operation = SceneManager.LoadSceneAsync(sceneName);
            isLoading = false;
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneId">场景编号</param>
        /// <param name="loadedAct">加载行为回调</param>
        /// <param name="progressAct">加载进度回调</param>
        public void LoadScene(int sceneId, System.Action<bool> loadedAct = null, System.Action<float> progressAct = null)
        {
            if (isLoading == false)
            {
                CoroutineTaskManager.Instance.AddTask(
                new CoroutineTaskManager.CoroutineTask("LoadScene" + sceneId, DoLoadScene(sceneId), loadedAct));
                CoroutineTaskManager.Instance.AddTask(
                    new CoroutineTaskManager.CoroutineTask("LoadSceneProgress" + sceneId, DoLoadSceneProgress(progressAct), loadedAct));
            }
            else
            {
                Debug.unityLogger.LogError("加载场景错误", "正在加载其他场景无法加载新场景");
            }
        }

        private IEnumerator DoLoadScene(int sceneId)
        {
            isLoading = true;
            yield return operation = SceneManager.LoadSceneAsync(sceneId);
            isLoading = false;
        }

        private IEnumerator DoLoadSceneProgress(System.Action<float> progressAct)
        {
            while (!operation.isDone)
            {
                if (progressAct != null)
                {
                    progressAct(operation.progress);
                }
                yield return null;
            }
        }  
	
	}

}

