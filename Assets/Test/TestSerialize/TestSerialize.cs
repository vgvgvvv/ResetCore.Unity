using System.Collections;
using System.Collections.Generic;
using System.Xml;
using ResetCore.Data;
using ResetCore.Util;
using UnityEngine;

public class TestSerialize : MonoBehaviour {

	// Use this for initialization
	void Start () {
        XmlDocument xDoc = new XmlDocument();
	    var Root = xDoc.CreateElement("Root");
        xDoc.AppendChild(Root);
        XmlUtil.WriteAny(Root, "TestArray", new Dictionary<int, List<int>>()
        {
            { 1, new List<int>(){1, 2, 3}},
            { 2, new List<int>(){1, 2, 3}},
            { 3, new List<int>(){1, 2, 3}},
        });
	    var res = XmlUtil.ReadAny(Root, "TestArray", typeof(Dictionary<int, List<int>>), null);
        Debug.Log(res.ConverToString());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
