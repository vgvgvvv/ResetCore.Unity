using ResetCore.ReAssembly;
using UnityEngine;
using ResetCore.Event;
using ResetCore.Util;

public class TestComponent : MonoBehaviour {

    public Vector3 testProp;

    public float testInt;

    private void Start()
    {
        Debug.Log(testProp.ConverToString());
        Debug.Log(testInt.ConverToString());
    }
}
