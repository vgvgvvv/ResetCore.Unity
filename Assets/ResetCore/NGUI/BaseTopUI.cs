using UnityEngine;
using System.Collections;

namespace ResetCore.NGUI
{
    public class BaseTopUI : BaseUI
    {
        private static readonly int startDepth = 2000;
        private static int currentHighestDepth = startDepth;

        protected override void OnEnable()
        {
            base.OnEnable();
            uiRoot = UIManager.Instance.topRoot;
            transform.SetParent(uiRoot, false);
            currentHighestDepth += 1;
            panel.depth = currentHighestDepth;
        }
    }

}
