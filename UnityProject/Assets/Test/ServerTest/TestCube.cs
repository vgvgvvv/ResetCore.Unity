using UnityEngine;
using System.Collections;
using ResetCore.Event;
using ResetCore.NetPost;
using Protobuf.Data;
using System;

public class TestCube : MonoBehaviour {

    public NetTransform tran;

    void Awake()
    {
        tran = GetComponent<NetTransform>();
    }

    // Use this for initialization
    void Start ()
    {
        
	}

    // Update is called once per frame
    void Update ()
    {
        Control();
    }

    private void Control()
    {
        if (Input.GetKey(KeyCode.A))
        {
            //transform.position = transform.position + new Vector3(-0.1f, 0, 0);
            Vector3 newPos = transform.position + new Vector3(-0.1f, 0, 0);
            tran.SetLocalPosition(newPos);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //transform.position = transform.position + new Vector3(0.1f, 0, 0);
            Vector3 newPos = transform.position + new Vector3(0.1f, 0, 0);
            tran.SetLocalPosition(newPos);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            //transform.position = transform.position + new Vector3(0, 0.1f, 0);

            Vector3 newPos = transform.position + new Vector3(0, 0.1f, 0);
            tran.SetLocalPosition(newPos);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //transform.position = transform.position + new Vector3(0, -0.1f, 0);

            Vector3 newPos = transform.position + new Vector3(0, -0.1f, 0);
            tran.SetLocalPosition(newPos);
        }
    }

}
