using System.Collections;
using System.Collections.Generic;
using ResetCore.GameSystem;
using UnityEngine;

public class TestTimePlayer : TimePlayer
{
    private string name;

    public TestTimePlayer(string name)
    {
        this.name = name;
    }

    protected override void OnStart()
    {
        Debug.Log(name + " Start");
    }

    protected override void OnUpdate()
    {
        Debug.Log(name + " pastTime:" + pastTime);
    }

    protected override void OnEnd()
    {
        Debug.Log(name + " End");
    }
}
