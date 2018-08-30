using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResetCore.Util;

public class TestCoroutineDriver : MonoBehaviour {

    private void Awake()
    {
        //ReCoroutineManager.AddCoroutine(TestCoroutine());

        ReCoroutineTaskManager.Instance.AddTask("TestTask", TestWaitCor(), (bo) =>
        {
            //回调
        }, this);


    }

    IEnumerator<float> TestCoroutine()
    {
        int sum = 0;
        for (int i = 0; i < int.MaxValue; i++)
        {
            sum++;
            yield return 0;
        }
    }

    IEnumerator<float> TestWaitCor()
    {
        Debug.Log("BeginWait");
        WWW www = new WWW("http://121.196.216.106:4040/Escape/ServerAddress.txt");
        yield return ReCoroutine.WaitWWW(www);
        Debug.Log(www.text);
        Debug.Log("EndWait");
    }

    IEnumerator TestCoroutine2()
    {
        int sum = 0;
        for (int i = 0; i < int.MaxValue; i++)
        {
            sum++;
            yield return CoroutineConst.GetWaitForSeconds(1);
            yield return CoroutineConst.waitForEndOfFrame;
            yield return CoroutineConst.waitForFixedUpdate;
        }
    }

    IEnumerator<float> TestCoroutine3()
    {
        int sum = 0;
        for (int i = 0; i < int.MaxValue; i++)
        {
            sum++;
            yield return 0;
        }
    }
}
