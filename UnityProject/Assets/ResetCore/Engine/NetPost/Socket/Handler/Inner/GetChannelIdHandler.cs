using UnityEngine;
using System.Collections;
using System;
using Protobuf.Data;
using ResetCore.Event;

namespace ResetCore.NetPost
{
    /// <summary>
    /// 获取频道Id
    /// </summary>
    public class GetChannelIdHandler : NetPackageHandler
    {
        protected override void Handle(Package package, Action act = null)
        {
            EventDispatcher.TriggerEventWithTag<string>(ServerEvent.GetChannelId, package.GetValue<StringData>().Value, ownerServer);
        }
    }
}

