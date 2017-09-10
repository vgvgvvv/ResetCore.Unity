using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ResetCore.NGUI
{
    public abstract class ShowUIArg{}

    public abstract class BaseUI : MonoBehaviour
    {

        public Transform uiRoot { get; protected set; }
        [SerializeField]
        private UIConst.UIName _uiName;
        public UIConst.UIName uiName { get { return _uiName; } }

        private UIPanel _panel;
        protected UIPanel panel
        {
            get
            {
                if (_panel == null)
                {
                    _panel = GetComponent<UIPanel>();
                }
                return _panel;
            }
        }

        protected virtual void Awake() 
        {
            
        }

        protected virtual void OnEnable() 
        {
            
        }

        protected virtual void Start() { }

        public virtual void Init(ShowUIArg arg)
        {
            if (arg == null) return;
        }
        protected virtual void Update() { }

        protected virtual void OnDisable() { }

        protected void Hide(System.Action afterAct = null)
        {
            UIManager.Instance.HideUI(uiName, afterAct);
        }
    }
}

