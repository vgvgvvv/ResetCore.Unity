using UnityEngine;
using System.Collections;
using System;
using ResetCore.Util;
using ResetCore.Event;
using Protobuf.Data;

namespace ResetCore.NetPost
{
    public class TestHandler : NetPackageHandler
    {
        protected override void Handle(Package package, Action act = null)
        {
            Vector3DData vec = package.GetValue<Vector3DData>();
            Debug.LogError(vec.X + " " + vec.Y + " " + vec.Z);
            EventDispatcher.TriggerEvent<Vector3DData>("TestHandler", vec);
            if (act != null)
            {
                act();
            }
        }
    }
}
