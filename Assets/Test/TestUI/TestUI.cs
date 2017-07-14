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

[InContext(typeof(DemoContext))]
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
        //b.Bind(v, m);
        //v.inputInputField.Bind(m.money);
        //v.txtResult.Bind(m.money);
        //m.money.Init();
        var inputEventProperty = v.inputInputField.GetEventText();
        var textEventProperty = v.txtText.GetEventText();
        inputEventProperty.Bind(textEventProperty);

    }

    public void OnTestButton()
    {
        v.txtText.text = v.goTestButton.name;
    }

}
