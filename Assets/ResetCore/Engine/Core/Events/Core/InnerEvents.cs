using UnityEngine;
using System.Collections;

namespace ResetCore.Event
{
    public static class InnerEvents
    {

        public class UGUIEvents
        {
            public static readonly string OnLocalize = "UGUIEvents.OnLocalize";
        }

        public class GameEvents
        {
            public static readonly string OnGameInit = "GameEvents.OnGameInit";
        }
    }

}
