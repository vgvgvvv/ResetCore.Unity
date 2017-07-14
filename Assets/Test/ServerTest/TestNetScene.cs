using UnityEngine;
using System.Collections;
using ResetCore.NetPost;
using ResetCore.Util;
using Protobuf.Data;

public class TestNetScene : NetScene<Transform3DData>
{

    GameObject cube;

    public override void OnAfterConnect(bool result)
    {
        base.OnAfterConnect(result);
        Object obj = Resources.Load("Cube");
        cube = GameObject.Instantiate(obj) as GameObject;
        cube.ResetTransform();
        NetTransform netTran = cube.GetComponent<NetTransform>();
        netTran.instanceId = 100;
    }

    public override void OnAfterDisconnect(bool result)
    {
        base.OnAfterDisconnect(result);
        Destroy(cube);
    }

    public override Transform3DData CreateSnapshotData()
    {
        Transform3DData data = new Transform3DData();
        data.LocalPosition = new Vector3DData();
        data.LocalPosition.X = transform.localPosition.x;
        data.LocalPosition.Y = transform.localPosition.y;
        data.LocalPosition.Z = transform.localPosition.z;

        data.LocalEulerAngle = new Vector3DData();
        data.LocalEulerAngle.X = transform.localEulerAngles.x;
        data.LocalEulerAngle.Y = transform.localEulerAngles.y;
        data.LocalEulerAngle.Z = transform.localEulerAngles.z;

        data.LocalScale = new Vector3DData();
        data.LocalScale.X = transform.localScale.x;
        data.LocalScale.Y = transform.localScale.y;
        data.LocalScale.Z = transform.localScale.z;
        
        return data;
    }

    public override void HandleSnapshot(Package pkg)
    {
        Debug.Log("处理快照");
        var trans = pkg.GetValue<Transform3DData>();
        NetTransform netTran = cube.GetComponent<NetTransform>();
        netTran.SetData(trans);
        cube.transform.localPosition = new Vector3(trans.LocalPosition.X, trans.LocalPosition.Y, trans.LocalPosition.Z);
    }

}
