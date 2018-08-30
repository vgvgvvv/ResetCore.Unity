using UnityEngine;
using System.Collections;

namespace ResetCore.Event
{
    public class MonoEventCleanUp : MonoBehaviour
    {
        void OnDestroy()
        {
            MonoEventDispatcher.RemoveMonoController(gameObject);
        }
    }

}
