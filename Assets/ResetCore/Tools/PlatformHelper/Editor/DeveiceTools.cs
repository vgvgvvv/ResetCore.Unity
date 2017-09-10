using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ResetCore.PlatformHelper
{
    public class DeveiceTools
    {
        [MenuItem("Tools/PlatformHelper/CreateDeviceManager", false, -1)]
        public static void CreateDeviceManager()
        {
            GameObject go = new GameObject();
            go.name = "DeviceManager";
            go.AddComponent<DeviceManager>();
        }
    }
}
