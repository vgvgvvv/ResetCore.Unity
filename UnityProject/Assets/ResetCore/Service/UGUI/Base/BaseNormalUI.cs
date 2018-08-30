using UnityEngine;
using System.Collections;

namespace ResetCore.UGUI
{
    public abstract class BaseNormalUI : BaseUI
    {

        protected override void OnEnable()
        {
            uiRoot = UIManager.Instance.normalRoot;
            transform.SetParent(uiRoot, false);
            transform.SetAsLastSibling();
        }

    }
}


