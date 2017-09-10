using UnityEngine;
using System.Collections;
using Protobuf.Data;
using System;

namespace ResetCore.NetPost
{
    public class NetTransform : NetBehavior<Transform3DData>
    {
        public override HandlerConst.RequestId handlerId
        {
            get
            {
                return HandlerConst.RequestId.NetTransform;
            }
        }

        /// <summary>
        /// 获取局部坐标
        /// </summary>
        public Vector3 localPosition
        {
            get
            {
                return new Vector3(behaviorData.LocalPosition.X, behaviorData.LocalPosition.Y, behaviorData.LocalPosition.Z);
            }
        }

        /// <summary>
        /// 获取局部欧拉角
        /// </summary>
        public Vector3 localEulerAngle
        {
            get
            {
                return new Vector3(behaviorData.LocalEulerAngle.X, behaviorData.LocalEulerAngle.Y, behaviorData.LocalEulerAngle.Z);
            }
        }

        /// <summary>
        /// 获取局部缩放
        /// </summary>
        public Vector3 localScale
        {
            get
            {
                return new Vector3(behaviorData.LocalScale.X, behaviorData.LocalScale.Y, behaviorData.LocalScale.Z);
            }
        }

        public override void Awake()
        {
            base.Awake();
            behaviorData = new Transform3DData();
            behaviorData.InstanceId = instanceId;
            behaviorData.LocalPosition = new Vector3DData();
            behaviorData.LocalPosition.X = gameObject.transform.localPosition.x;
            behaviorData.LocalPosition.Y = gameObject.transform.localPosition.y;
            behaviorData.LocalPosition.Z = gameObject.transform.localPosition.z;

            behaviorData.LocalEulerAngle = new Vector3DData();
            behaviorData.LocalEulerAngle.X = gameObject.transform.localEulerAngles.x;
            behaviorData.LocalEulerAngle.Y = gameObject.transform.localEulerAngles.y;
            behaviorData.LocalEulerAngle.Z = gameObject.transform.localEulerAngles.z;

            behaviorData.LocalScale = new Vector3DData();
            behaviorData.LocalScale.X = gameObject.transform.localScale.x;
            behaviorData.LocalScale.Y = gameObject.transform.localScale.y;
            behaviorData.LocalScale.Z = gameObject.transform.localScale.z;
        }

        public override void OnNetUpdate(Package serverPkg)
        {
            base.OnNetUpdate(serverPkg);
            Transform3DData serverData = serverPkg.GetValue<Transform3DData>();

            if(serverData.InstanceId != this.instanceId)
            {
                Debug.Log(serverData.InstanceId + " " + this.instanceId);
                return;
            }

            gameObject.transform.localPosition = new Vector3(serverData.LocalPosition.X, serverData.LocalPosition.Y, serverData.LocalPosition.Z);
            gameObject.transform.localScale = new Vector3(serverData.LocalScale.X, serverData.LocalScale.Y, serverData.LocalScale.Z);
            gameObject.transform.localEulerAngles = new Vector3(serverData.LocalEulerAngle.X, serverData.LocalEulerAngle.Y, serverData.LocalEulerAngle.Z);
        }
        
        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="pos"></param>
        public void SetLocalPosition(Vector3 pos)
        {
            Transform3DData data = new Transform3DData();
            data.InstanceId = instanceId;
            data.LocalPosition = new Vector3DData();
            data.LocalPosition.X = pos.x;
            data.LocalPosition.Y = pos.y;
            data.LocalPosition.Z = pos.z;

            data.LocalEulerAngle = new Vector3DData();
            data.LocalEulerAngle.X = gameObject.transform.localEulerAngles.x;
            data.LocalEulerAngle.Y = gameObject.transform.localEulerAngles.y;
            data.LocalEulerAngle.Z = gameObject.transform.localEulerAngles.z;

            data.LocalScale = new Vector3DData();
            data.LocalScale.X = gameObject.transform.localScale.x;
            data.LocalScale.Y = gameObject.transform.localScale.y;
            data.LocalScale.Z = gameObject.transform.localScale.z;

            SetData(data);
        }

        /// <summary>
        /// 设置旋转
        /// </summary>
        /// <param name="eulerAngle"></param>
        public void SetLocalEulerAngle(Vector3 eulerAngle)
        {
            Transform3DData data = new Transform3DData();
            data.InstanceId = instanceId;
            data.LocalPosition = new Vector3DData();
            data.LocalPosition.X = gameObject.transform.localPosition.x;
            data.LocalPosition.Y = gameObject.transform.localPosition.y;
            data.LocalPosition.Z = gameObject.transform.localPosition.z;

            data.LocalEulerAngle = new Vector3DData();
            data.LocalEulerAngle.X = eulerAngle.x;
            data.LocalEulerAngle.Y = eulerAngle.y;
            data.LocalEulerAngle.Z = eulerAngle.z;

            data.LocalScale = new Vector3DData();
            data.LocalScale.X = gameObject.transform.localScale.x;
            data.LocalScale.Y = gameObject.transform.localScale.y;
            data.LocalScale.Z = gameObject.transform.localScale.z;

            SetData(data);
        }

        /// <summary>
        /// 设置尺寸
        /// </summary>
        /// <param name="eulerAngle"></param>
        public void SetLocalScale(Vector3 scale)
        {
            Transform3DData data = new Transform3DData();
            data.InstanceId = instanceId;
            data.LocalPosition = new Vector3DData();
            data.LocalPosition.X = gameObject.transform.localPosition.x;
            data.LocalPosition.Y = gameObject.transform.localPosition.y;
            data.LocalPosition.Z = gameObject.transform.localPosition.z;

            data.LocalEulerAngle = new Vector3DData();
            data.LocalEulerAngle.X = gameObject.transform.localEulerAngles.x;
            data.LocalEulerAngle.Y = gameObject.transform.localEulerAngles.y;
            data.LocalEulerAngle.Z = gameObject.transform.localEulerAngles.z;

            data.LocalScale = new Vector3DData();
            data.LocalScale.X = scale.x;
            data.LocalScale.Y = scale.y;
            data.LocalScale.Z = scale.z;

            SetData(data);
        }

    }

}
