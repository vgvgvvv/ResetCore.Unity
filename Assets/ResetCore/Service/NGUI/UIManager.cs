using UnityEngine;
using System.Collections;
using ResetCore.Util;
using System.Collections.Generic;
using ResetCore.NAsset;

namespace ResetCore.NGUI
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField]
        private Camera _camera;
        public Camera canvasCamera { get { return _camera; } }
        [SerializeField]
        private Transform _normalRoot;
        public Transform normalRoot { get { return _normalRoot; } }
        [SerializeField]
        private Transform _popupRoot;
        public Transform popupRoot { get { return _popupRoot; } }
        [SerializeField]
        private Transform _topRoot;
        public Transform topRoot { get { return _topRoot; } }

        private Dictionary<UIConst.UIName, BaseUI> uiDic = new Dictionary<UIConst.UIName, BaseUI>();

        public void ShowUI(UIConst.UIName name, System.Action afterAct = null, ShowUIArg arg = null)
        {
            if (uiDic.ContainsKey(name))
            {
                if (uiDic[name] == null)
                {
                    BaseUI newUI = GameObject.Instantiate(
                        AssetLoader.GetGameObject(UIConst.UIPrefabBundleDic[name],
                        UIConst.UIPrefabNameDic[name])).GetComponent<BaseUI>();
                    uiDic[name] = newUI;
                }
                uiDic[name].gameObject.SetActive(true);
                uiDic[name].transform.SetAsLastSibling();
                uiDic[name].Init(arg);
            }
            else
            {
                BaseUI newUI = GameObject.Instantiate(
                        AssetLoader.GetGameObject(UIConst.UIPrefabBundleDic[name],
                        UIConst.UIPrefabNameDic[name])).GetComponent<BaseUI>();
                newUI.Init(arg);
                uiDic.Add(name, newUI);
            }

            if (afterAct != null)
            {
                afterAct();
            }

        }

        public void HideUI(UIConst.UIName name, System.Action afterAct = null)
        {
            if (uiDic.ContainsKey(name))
            {
                uiDic[name].gameObject.SetActive(false);
                if (afterAct != null)
                {
                    afterAct();
                }
            }
        }

        public BaseUI GetUI(UIConst.UIName name)
        {
            if (uiDic.ContainsKey(name))
            {
                if (uiDic[name] != null)
                {
                    return uiDic[name].gameObject.GetComponent<BaseUI>();
                }
                else
                {
                    uiDic.Remove(name);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void CleanUI()
        {
            normalRoot.transform.DeleteAllChild();

            popupRoot.transform.DeleteAllChild();

            topRoot.transform.DeleteAllChild();

            uiDic.Clear();
        }
    }
}

