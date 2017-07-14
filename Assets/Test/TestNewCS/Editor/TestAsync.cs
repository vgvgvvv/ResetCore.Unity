using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

public class TestAsync
{

	static async void Async2()
	{
		await Task.Run(() => { Thread.Sleep(500); Debug.unityLogger.Log("bbb"); });
		Debug.unityLogger.Log("ccc");
	}
	
	[Test]
	public void NewEditModeTestSimplePasses()
	{
		Async2();
		Debug.unityLogger.Log("aaa");
		// Use the Assert class to test conditions.
	}
	
}
