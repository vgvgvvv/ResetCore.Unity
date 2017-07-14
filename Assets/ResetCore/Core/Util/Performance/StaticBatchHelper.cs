using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResetCore.Util
{
    public class StaticBatchHelper : MonoBehaviour
    {

        private void Awake()
        {
            StaticBatchingUtility.Combine(gameObject);
        }
    }

}
