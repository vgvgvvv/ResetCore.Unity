using UnityEngine;
using ResetCore.Event;

namespace ResetCore.Util
{
    public class GameInitManager : MonoBehaviour
    {

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            EventDispatcher.TriggerEvent(InnerEvents.GameEvents.OnGameInit);
        }
    }

}
