using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResetCore.Util;
using System;

public class TestPass : BasePass<List<string>, string>
{
    public override string HandlePass(List<string> input)
    {
        string temp = "";
        foreach (var i in input)
        {
            temp += i;
        }
        return temp;
    }

}

public class TestIntPass : BasePass<string, int>
{
    public override int HandlePass(string input)
    {
        return int.Parse(input);
    }

}
