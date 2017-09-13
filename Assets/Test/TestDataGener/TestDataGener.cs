using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResetCore.Data.GameDatas.Json;
using ResetCore.Util;

public class TestDataGener : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    BuffData.dataMap.ForEach((k, v) =>
	    {
	        Debug.Log(v.BuffName);
	        Debug.Log(v.BuffTime);
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
