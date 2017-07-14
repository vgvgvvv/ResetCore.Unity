using UnityEngine;
using System.Collections;


namespace ResetCore.UGUI
{
    public abstract class BaseTopUI : BaseUI
    {
        protected override void OnEnable()
        {
            uiRoot = UIManager.Instance.topRoot;
            transform.SetParent(uiRoot, false);
            transform.SetAsLastSibling();
        }


    }
}

