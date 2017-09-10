using UnityEngine;
using System.Collections;
using ResetCore.Util;

public class TestScriptableObj {

    public int testValue = 1;
    public string testStr = "asdasd";

    [ToString]
    public static string ConvertToString(TestScriptableObj obj)
    {
        return obj.testValue + ":" + obj.testStr;
    }

    [FromString]
    public static TestScriptableObj ConvertFromString(string str)
    {
        TestScriptableObj obj = new TestScriptableObj();
        string[] strs = str.Split(':');
        obj.testValue = strs[0].GetValue<int>();
        obj.testStr = strs[1];
        return obj;
    }
}
