using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using I18N.West;
using static UnityEngine.Mathf;

public class TestCsharp6 {

	[Test]
	//测试字符串中加入表达式
	public void TestCsharp6SimplePasses() {
		var Name = "Jack";
		var results = $"Hello {Name}";
		Assert.AreEqual(results,  "Hello Jack");
	}

	[Test]
	//测试可空操作
	public void TestNullOperation()
	{
		User user = null;
		user?.SayHello();
	}

	[Test]
	//测试Nameof
	public void TestNameOf()
	{
		Assert.AreEqual("User", nameof(User));
	}

	[Test]
	public static string Sayhello() => "Hello!~";

	public int age { get; set; } = 100;

	[Test]
	//测试初始化
	public void TestInit()
	{
		Assert.AreEqual(100, age);
	}

	public int readOnlyAge { get; } = 20;
	
	[Test]
	//测试初始化
	public void TestReadonly()
	{
		Assert.AreEqual(20, readOnlyAge);
	}

	[Test]
	//测试静态导入
	public void TestStaticUsing()
	{
		Assert.AreEqual(FloorToInt(9.8f), 9);
	}

	public void TestIndexInit()
	{
		var names = new Dictionary<int, string>()
		{
			[1] = "asd"
		};
	}

}

public class User
{
	public void SayHello()
	{
		Debug.Log("Hello");
	}
}
