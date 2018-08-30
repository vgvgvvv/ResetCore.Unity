using System.Collections;
using System.Collections.Generic;
using System.Xml;
using ResetCore.Data;
using ResetCore.Util;
using UnityEngine;
using System.IO;

public class TestSerialize : MonoBehaviour {

    class TestClass
    {
        public string testString;
        public List<int> testList;
        public Dictionary<int, string> testDict;

        public List<TestChild> TestChildren;
    }

    class TestChild
    {
        public string testString;
        public List<int> testList;
    }

	// Use this for initialization
	void Start () {
        XmlDocument xDoc = new XmlDocument();
	    var Root = xDoc.CreateElement("Root");
        xDoc.AppendChild(Root);

        var testClass = new TestClass();
	    testClass.testString = "asdasdas";
        testClass.testList = new List<int>()
        {
            1, 2, 3
        };
	    testClass.testDict = new Dictionary<int, string>()
	    {
	        {1, "asdasd"},
	        {2, "asdcqwe"},
	    };

	    testClass.TestChildren = new List<TestChild>()
	    {
	        new TestChild() {testString = "asdasd", testList = new List<int>() {5, 3, 123}},
	        new TestChild() {testString = "asdasd", testList = new List<int>() {5, 3, 123}},
	        new TestChild() {testString = "asdasd", testList = new List<int>() {5, 3, 123}},
            new TestChild() {testString = "asdasd", testList = new List<int>() {5, 3, 123}}
        };

        ReXmlSerializer.WriteAny(Root, "TestClass", testClass);
        Debug.Log(xDoc.InnerXml);

	    var res = ReXmlSerializer.ReadAny(Root, "TestClass", typeof(TestClass), null);
	    Debug.Log(res);

    }

    // Update is called once per frame
    void Update () {
		
	}
}
