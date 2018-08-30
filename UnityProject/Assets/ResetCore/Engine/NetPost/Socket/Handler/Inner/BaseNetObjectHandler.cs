using UnityEngine;
using System.Collections;
using System;
using ResetCore.Util;
using ResetCore.Event;

namespace ResetCore.NetPost
{
    public class BaseNetObjectHandler : NetPackageHandler
    {

        protected override void Handle(Package package, Action act = null)
        {
            var id = EnumEx.GetValue<HandlerConst.RequestId>(package.eventId);
            EventDispatcher.TriggerEvent<Package>(NetSceneEvent.GetNetBehaviorEventName(id), package);
        }
    }

}
