using UnityEngine;
using System.Collections;
using ResetCore.UGUI;
using UnityEngine.UI;
using ResetCore.Event;
using ResetCore.UGUI.View;
using ResetCore.IOC;
using ResetCore.UGUI.Model;
using System.Xml.Linq;
using ResetCore.UGUI.Binder;

[InCodeContext(typeof(DemoCodeContext))]
public class TestUI : BaseNormalUI {

    [Inject]
    TestUIView v;

    [Inject]
    TestUIModel m;

    [Inject]
    TestUIBinder b;

    protected override void Awake()
    {
        base.Awake();
        v.Init(this);
        v.txtMoney.Bind(m.money);
        v.inputInputField.Bind(m.money);
    }

    public void OnTestButton()
    {
    }

}
