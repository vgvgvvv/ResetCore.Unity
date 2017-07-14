using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TestCsharp5 {

		
	[Test]
	//测试可空对象的可计算性
	public void TestNullableVar ()
	{
		int? y = null;
		int? x = 100 + y;
		Debug.Log(x);
		Debug.Log(y);
		Assert.AreEqual(x, 100);
	}

	[Test]
	//测试Switch函数的表达式case
	public void TestCase ()
	{

		int i = 5;
		bool res = false;
		switch (i)
		{
			case 2+3:
				res = true;
				break;
		}
		Assert.True(res);
	}

	[Test]
	public void TestLazy()
	{
		Lazy<int> lazyInt = new Lazy<int>();
	}

	[Test]
	public void TestCall()
	{
		TraceMessage("test");
	}
		
	public void TraceMessage(string message,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0)
	{
		Debug.unityLogger.Log("message: " + message);
		Debug.unityLogger.Log("member name: " + memberName);
		Debug.unityLogger.Log("source file path: " + sourceFilePath);
		Debug.unityLogger.Log("source line number: " + sourceLineNumber);
	}
}
