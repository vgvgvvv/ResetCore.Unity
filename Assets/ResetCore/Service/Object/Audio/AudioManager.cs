using UnityEngine;
using UnityEngine.Audio;
using ResetCore.Util;
using System.Collections.Generic;
using ResetCore.NAsset;
#if DATA_GENER
using ResetCore.Data;
#endif

namespace ResetCore.ResObject
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [SerializeField]
        private bool isLocalization = false;

        public bool localization { get { return isLocalization; } }

        [SerializeField]
        private List<AudioMixerGroup> groupList;

        public Dictionary<string, AudioMixerGroup> groupDictionary
        {
            get;
            private set;
        }

        [SerializeField]
        private AudioMixerGroup BGMGroup;

        private Transform BGMPool;
        private Transform SEPool;


        public override void Init()
        {
            base.Init();

            groupDictionary = new Dictionary<string, AudioMixerGroup>();
            if (groupList != null)
            {
                foreach (AudioMixerGroup group in groupList)
                {
                    groupDictionary.Add(group.name, group);
                }
            }

            GameObject bgmPool = new GameObject("BGMPool");
            BGMPool = bgmPool.transform;
            BGMPool.SetParent(transform);
            GameObject sePool = new GameObject("SEPool");
            SEPool = sePool.transform;
            SEPool.SetParent(transform);
            DontDestroyOnLoad(gameObject);
        }
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clipName">音频资源名(带后缀</param>
        public void PlayBGM(string clipBundle, string clipName)
        {
            GameObject BGMObject = null;
            BGMPool.DoToAllChildren((tran) =>
            {
                AudioSource source = tran.GetComponent<AudioSource>();
                if (source.clip.name == clipName && !source.isPlaying)
                {
                    BGMObject = source.gameObject;
                    PlayObject(BGMObject, clipBundle, clipName, 1, -1, null, true, true);
                }
                if (source.isPlaying)
                {
                    source.volume = 0;
                }
            });
            if (BGMObject == null)
            {
                BGMObject = new GameObject(clipName);
                BGMObject.transform.SetParent(BGMPool);
                PlayObject(BGMObject.gameObject, clipBundle, clipName, 1, -1, BGMGroup, true, true);
            }
        }

        /// <summary>
        /// 播放局部音效
        /// </summary>
        /// <param name="go">绑定的物体</param>
        /// <param name="clipName">音频资源名（带后缀</param>
        /// <param name="mixerGroup"></param>
        /// <param name="isLoop"></param>
        /// <param name="playOnAwake"></param>
        /// <param name="fadeIn"></param>
        public AudioSource PlayObjectSE(GameObject go, string clipBundle, string clipName, float volume = 1, float time = -1, string mixerGroup = "")
        {
            AudioMixerGroup group = groupDictionary.ContainsKey(mixerGroup) ? groupDictionary[mixerGroup] : null;
            AudioSource source = PlayObject(go, clipBundle, clipName, volume, time, group);

            return source;
        }

        /// <summary>
        /// 播放3D局部音效
        /// </summary>
        /// <param name="go">绑定的物体</param>
        /// <param name="clipName">音频资源名</param>
        /// <param name="volume">音量</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="time">持续时间</param>
        /// <param name="mixerGroup">混合器组</param>
        /// <returns></returns>
        public AudioSource Play3DObjectSE(GameObject go, string clipBundle, string clipName, float volume = 1, float maxDistance = 500, float time = -1, string mixerGroup = "")
        {
            AudioMixerGroup group = groupDictionary.ContainsKey(mixerGroup) ? groupDictionary[mixerGroup] : null;
            AudioSource source = PlayObject(go, clipBundle, clipName, volume, time, group);
            source.spatialBlend = 1;
            source.maxDistance = maxDistance;
            return source;
        }


        /// <summary>
        /// 播放全局音效
        /// </summary>
        /// <param name="clipName">音频资源名（带后缀</param>
        /// <param name="mixerGroup">混音器名</param>
        public AudioSource PlayGlobalSE(string clipBundle, string clipName, float volume = 1, float time = -1, string mixerGroup = "")
        {
            AudioMixerGroup group = groupDictionary.ContainsKey(mixerGroup) ? groupDictionary[mixerGroup] : null;
            AudioSource source = PlayObject(FindOrCreateSEClipObject(clipName, SEPool), clipBundle, clipName, volume, time, group, false, false);
            return source;
        }


        private AudioSource PlayObject(GameObject go, string clipBundle, string clipName, float volume, float time, AudioMixerGroup mixerGroup = null, bool isLoop = false, bool fadeIn = false)
        {
            AudioSource audioSource = go.GetOrCreateComponent<AudioSource>();
#if DATA_GENER
            if (isLocalization)
            {
                if (LanguageManager.ContainKey(clipName))
                {
                    clipName = LanguageManager.GetWord(clipName);
                }
            }
#endif
            audioSource.clip = AssetLoader.GetAudio(clipBundle, clipName);
            audioSource.outputAudioMixerGroup = mixerGroup;
            audioSource.loop = isLoop;
            audioSource.volume = volume;
            audioSource.Play();
            if (time > 0)
            {
                CoroutineTaskManager.Instance.WaitSecondTodo(() =>
                {
                    if (audioSource == null) return;
                    audioSource.Stop();
                }, time);
            }
            return audioSource;
        }

        private GameObject FindOrCreateSEClipObject(string clipName, Transform pool)
        {
            List<AudioSource> fitSource = new List<AudioSource>();
            pool.DoToAllChildren((tran) =>
            {
                AudioSource source = tran.GetComponent<AudioSource>();
                if (source.clip.name == clipName && !source.isPlaying)
                {
                    fitSource.Add(source);
                }
            });
            if (fitSource.Count > 0)
            {
                return fitSource[0].gameObject;
            }
            else
            {
                GameObject newSource = new GameObject(clipName);
                newSource.transform.SetParent(pool);
                return newSource;
            }
        }
    }

}
