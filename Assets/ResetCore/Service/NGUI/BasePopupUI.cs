using UnityEngine;
using System.Collections;

namespace ResetCore.NGUI
{
    public class BasePopupUI : BaseUI
    {
        private static readonly int startDepth = 1000;
        private static int currentHighestDepth = startDepth;

        protected override void OnEnable()
        {
            base.OnEnable();
            uiRoot = UIManager.Instance.popupRoot;
            transform.SetParent(uiRoot, false);
            currentHighestDepth += 1;
            panel.depth = currentHighestDepth;
        }
    }
}

