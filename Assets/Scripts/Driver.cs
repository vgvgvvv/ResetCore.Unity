using ResetCore.Util;
using System.Collections.Generic;
using ResetCore.NetPost;
using UnityEngine;
using ResetCore.Event;

//using ResetCore.Data.GameDatas;

public class Driver : MonoSingleton<Driver> {

   
    void Awake()
    {
        AddTask addTask = new AddTask();
        var first = new ConstValue(100);
        var second = new ConstValue(200);
        addTask.AddDependence("first", first);
        addTask.AddDependence("second", second);
        addTask.StartTask();
        Debug.LogError(addTask.Result);
    }

    void Update()
    {
        
    }
    
}

public class AddTask : Task<int>
{
    protected override object DoProcess(BetterDictionary<string, object> results)
    {
        int first = (int)results["first"];
        int second = (int)results["second"];
        return first + second;
    }
}

public class ConstValue : Task<int>
{
    private readonly int _value;

    public ConstValue(int value)
    {
        this._value = value;
    }

    protected override object DoProcess(BetterDictionary<string, object> results)
    {
        return _value;
    }
}
