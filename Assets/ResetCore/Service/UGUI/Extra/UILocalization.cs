using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ResetCore.Event;
using ResetCore.Data;
using ResetCore.Util;
using ResetCore.NAsset;

namespace ResetCore.UGUI
{
    [AddComponentMenu("UI/Extra/UILocalization")]
    public class UILocalization : MonoBehaviour
    {
        
        private bool started = false;

        public string key;

        private string textToShow;
        public string val
        {
            set
            {
#if DATA_GENER

                key = value;
                textToShow = LanguageManager.GetWord(key);
                Text txt = GetComponent<Text>();
                Image image = GetComponent<Image>();
                if(txt != null)
                {
                    txt.text = textToShow;
                }
                if (image != null)
                {
                   //命名要求为： 包名-sprite名
                    if (string.IsNullOrEmpty(textToShow)) return;
                    Sprite spr = AssetLoader.GetSpriteByR(textToShow);
                    if(spr != null)
                    {
                        image.sprite = spr;
                    }
                }
#else
                key = value;
#endif
            }
        }

        private void OnLocalize()
        {
            val = key;
        }

        void Awake()
        {
            EventDispatcher.AddEventListener(InnerEvents.UGUIEvents.OnLocalize, OnLocalize);
        }

        void OnDestroy()
        {
            EventDispatcher.RemoveEventListener(InnerEvents.UGUIEvents.OnLocalize, OnLocalize);
        }

        void Start()
        {
            started = true;
            OnLocalize();
        }

        void OnEnable()
        {
            if (started)
            {
                OnLocalize();
            }
        }
    }
}
