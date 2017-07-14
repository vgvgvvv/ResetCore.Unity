using ResetCore.Util;
using System.Collections.Generic;
using ResetCore.NetPost;
using UnityEngine;
using ResetCore.Event;

//using ResetCore.Data.GameDatas;

public class Driver : MonoSingleton<Driver> {

    //Pipeline<List<string>, int> pipeLine = new Pipeline<List<string>, int>();
    List<string> list = new List<string>();
    void Awake()
    {
        //int res = pipeLine.AddPass(new TestPass())
        //    .AddPass(new TestIntPass())
        //    .SyncProcess(new List<string> { "1", "222" });
        //Debug.LogError(res);
        for(int i = 0; i < 100000; i++)
        {
            list.Add(i.ToString());
        }
    }

    void Update()
    {
        var e = list.GetEnumerator();
        while (e.MoveNext())
        {
            
        }
    }
    
}
