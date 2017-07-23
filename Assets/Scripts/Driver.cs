using ResetCore.Util;
using System.Collections.Generic;
using ResetCore.NetPost;
using UnityEngine;
using ResetCore.Event;
using ResetCore.UGradle;
using UnityEngine.UI;

//using ResetCore.Data.GameDatas;

public class Driver : MonoSingleton<Driver>
{
    public GameObject layout;
    public Slider slider;

    void Awake()
    {
        WorkFlow workFlow = new WorkFlow("Test", "用于测试工作流");

        AddTask task3 = new AddTask("add3", "增加数字", 80);
        workFlow.AddTask(
            new AddTask("add1", "增加数字", 50)
                .DependsOn(
                    new AddTask("add2", "增加数字", 70)
                        .DependsOn(task3))
                .DependsOn(
                    new AddTask("add4", "增加数字", 90)
                        .DependsOn(task3))
        );


        workFlow.Start();
    }

    
}

public class AddTask : Task
{
    public int currentNumber { get; private set; }
    public int target { get; private set; }

    public AddTask(string name, string description, int target) : base(name, description, TaskActionType.Asyn)
    {
        this.target = target;
        this.currentNumber = 0;
        AddAync(AddNumber());
    }

    IEnumerator<float> AddNumber()
    {
        Debug.unityLogger.Log(name + "开始！");
        Driver.Instance.slider.gameObject.SetActive(true);
        var slider = GameObject.Instantiate(Driver.Instance.slider.gameObject).GetComponent<Slider>();
        var text = slider.transform.FindObjectInChild("Text").GetComponent<Text>();
        slider.transform.SetParent(Driver.Instance.layout.transform);
        Driver.Instance.slider.gameObject.SetActive(false);
        while (currentNumber < target)
        {
            slider.value = (float) currentNumber / (float) target;
            text.text = "任务：" + name + " 进度:" + currentNumber;
            currentNumber++;
            yield return 0.1f;
        }
        GameObject.Destroy(slider.gameObject);    
    }
}