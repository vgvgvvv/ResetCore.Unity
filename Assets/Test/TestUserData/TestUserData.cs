using ResetCore.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[LoadableClass("Test")]
public class TestUserData : LoadableObject {

    private int _testInt;

    [Loadable("testInt", 10)]
    public int testInt
    {
        get
        {
            return _testInt;
        }
        set
        {
            _testInt = value;
            Save();
        }
    }

}
