using UnityEngine;
using System.Collections;
using ResetCore.NetPost;

public class TestNetUI : MonoBehaviour {

    BaseServer sever;

    void Awake()
    {
        sever = new BaseServer();
    }
	public void OnConnect()
    {
        NetSceneManager.Instance.StartScene(1, "DefaultScene");
    }

    public void OnDisconnect()
    {
        NetSceneManager.Instance.Disconnect();
    }
}
