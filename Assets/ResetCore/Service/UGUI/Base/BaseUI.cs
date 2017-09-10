using UnityEngine;
using System.Collections;
using ResetCore.IOC;
using System.Xml.Linq;

namespace ResetCore.UGUI
{
    public abstract class ShowUIArg { }

    public abstract class BaseUI : IOCMonoBehavior
    {

        public Transform uiRoot { get; protected set; }

        [SerializeField]
        private UIConst.UIName _uiName;
        public UIConst.UIName uiName
        {
            get { return _uiName; }
            set { _uiName = value; }
        }

        protected override void Awake() { base.Awake(); }

        protected virtual void OnEnable() { }

        protected virtual void Start() { }

        public virtual void Init(ShowUIArg arg)
        {
            if (arg == null) return;
        }

        public virtual void UpdateUI(string funcName, Hashtable arg) { }

        protected virtual void Update() { }

        protected virtual void OnDisable() { }

        public virtual void Show(System.Action afterAct = null)
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide(System.Action afterAct = null)
        {
            gameObject.SetActive(false);
        }

    }
}

