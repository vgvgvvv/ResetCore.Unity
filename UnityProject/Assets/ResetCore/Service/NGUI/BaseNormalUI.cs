using UnityEngine;
using System.Collections;

namespace ResetCore.NGUI
{
    public class BaseNormalUI : BaseUI
    {
        private static readonly int startDepth = 0;
        private static int currentHighestDepth = startDepth;

        protected override void OnEnable()
        {
            base.OnEnable();
            uiRoot = UIManager.Instance.normalRoot;
            transform.SetParent(uiRoot, false);
            currentHighestDepth += 1;
            panel.depth = currentHighestDepth;
            
        }
        
    }

}
