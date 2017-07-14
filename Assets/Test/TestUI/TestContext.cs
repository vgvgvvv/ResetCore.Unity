using ResetCore.IOC;
using ResetCore.UGUI.Binder;
using ResetCore.UGUI.Model;
using ResetCore.UGUI.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestContext : Context<TestContext> {

	[Bean]
    public TestUIView GetView()
    {
        var v = new TestUIView();
        return v;
    }

    [Bean]
    public TestUIModel GetProxy()
    {
        var p = new TestUIModel();
        p.money.propValue = "asd";
        return p;
    }

    [Bean]
    public TestUIBinder GetBinder()
    {
        return new TestUIBinder();
    }

}
