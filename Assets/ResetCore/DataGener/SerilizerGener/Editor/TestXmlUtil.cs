using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Xml;
using ResetCore.Data;

public class TestXmlUtil {

	[Test]
	public void TestWriteAny() {
		// Use the Assert class to test conditions.
        XmlDocument xDoc = new XmlDocument();
        XmlUtil.WriteAny(xDoc.DocumentElement, "Root", new int[]{1,1,1});
        Debug.Log("asdasd");
	}

}
