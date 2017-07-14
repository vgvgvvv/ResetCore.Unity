using UnityEngine;
using System.Collections;
using System;

namespace ResetCore.NetPost
{
    /// <summary>
    /// 所有场景快照通用Handler
    /// </summary>
    public class NetSceneSnapShotHandler : NetPackageHandler
    {
        protected override void Handle(Package package, Action act = null)
        {
            if (NetSceneManager.Instance.sceneConnected)
            {
                NetSceneManager.Instance.currentScene.HandleSnapshot(package);
            }
            if (act != null)
                act();
        }
    }
}
