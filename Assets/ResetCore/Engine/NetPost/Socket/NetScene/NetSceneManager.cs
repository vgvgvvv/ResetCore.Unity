using UnityEngine;
using System.Collections;
using ResetCore.Util;
using System.Collections.Generic;
using Protobuf.Data;
using ResetCore.Event;
using System;

namespace ResetCore.NetPost
{
    public sealed class NetSceneManager : Singleton<NetSceneManager>
    {
        /// <summary>
        /// 场景是否成功连接
        /// </summary>
        private bool _sceneConnected = false;
        public bool sceneConnected
        {
            get
            {
                return _sceneConnected;
            }
            private set
            {
                _sceneConnected = value;
                if(_sceneConnected == true)
                {
                    EventDispatcher.TriggerEvent(NetSceneEvent.NetSceneReady);
                }
                else
                {
                    EventDispatcher.TriggerEvent(NetSceneEvent.NetSceneDisconnect);
                }
            }
        }

        /// <summary>
        /// 当前服务器
        /// </summary>
        public BaseServer currentServer { get; private set; }

        /// <summary>
        /// 当前场景Id
        /// </summary>
        public int currentSceneId { get; private set; }

        /// <summary>
        /// 当前场景
        /// </summary>
        public NetScene currentScene { get; private set; }

        /// <summary>
        /// 客户端创建的Net
        /// </summary>
        private Dictionary<int, NetBehavior> clientNetBehaviorDict = new Dictionary<int, NetPost.NetBehavior>();  

        /// <summary>
        /// 开启场景
        /// </summary>
        /// <param name="sceneId"></param>
        public void StartScene(int sceneId, string sceneType, System.Action onSuccess = null, System.Action onError = null)
        {
            if (sceneConnected)
            {
                Debug.unityLogger.LogError("NetPost", "在连接前请断开连接，当前连接Id为" + currentSceneId);
                return;
            }
            currentScene = GameObject.FindObjectOfType<NetScene>();
            if (currentScene == null)
            {
                Debug.unityLogger.LogError("NetPost", "未找到网络场景对象" + currentSceneId);
                return;
            }
            currentScene.OnBeforeConnect();

            currentSceneId = sceneId;
            if(currentServer == null)
            {
                currentServer = new BaseServer();
            }
            currentServer.Connect(ServerConst.ServerAddress, ServerConst.TcpRemotePort, ServerConst.UdpRemotePort, ServerConst.UdpLocalPort, true);

            CoroutineTaskManager.Instance.WaitSecondTodo(() =>
            {
                RequestSceneData reqData = new RequestSceneData();
                reqData.Id = sceneId;
                reqData.SceneType = sceneType;
                currentServer.Request(HandlerConst.RequestId.RequsetSceneHandler, -1, reqData, SendType.TCP, (reqSceneRes) =>
                {
                    bool result = reqSceneRes.GetValue<BoolData>().Value;
                    if (result == true)
                    {
                        Debug.unityLogger.Log("请求场景成功并且注册至频道");
                        EventDispatcher.AddEventListener<NetBehavior>(NetSceneEvent.NetBehaviorAddToScene, AddNetBehavior);
                        EventDispatcher.AddEventListener<NetBehavior>(NetSceneEvent.NetBehaviorRemoveFromScene, RemoveNetBehavior);
                        sceneConnected = true;

                        if(onSuccess != null)
                            onSuccess();

                    }
                    else
                    {
                        Debug.unityLogger.LogError("NetPost", "请求场景失败");
                        if (onError != null)
                            onError();
                    }
                    currentScene.OnAfterConnect(result);
                }, () =>
                {
                    Debug.unityLogger.LogError("NetPost", "请求场景超时");
                    if (onError != null)
                        onError();
                });
            }, 0.5f);

        }

        /// <summary>
        /// 断开场景
        /// </summary>
        public void Disconnect(System.Action onDisconnect = null)
        {
            if(sceneConnected == false)
            {
                Debug.unityLogger.LogError("NetPost", "当前不存在连接");
                return;
            }
            currentScene.OnBeforeDisconnect();

            sceneConnected = false;
            Int32Data sceneIdData = new Int32Data();
            sceneIdData.Value = currentSceneId;

            ActionQueue destroyQueue = new ActionQueue();
            //添加删除物体的行为
            foreach (var kvp in clientNetBehaviorDict)
            {
                Debug.Log(kvp.Value.gameObject.name);
                destroyQueue.AddAction((act)=> { kvp.Value.RequestDestroy(act); });
            }
            //添加最终断开场景的行为
            destroyQueue.AddAction((act) =>
            {
                currentServer.Request(HandlerConst.RequestId.DisconnectSceneHandler, -1, sceneIdData, SendType.TCP, (pkg) =>
                {
                    bool result = pkg.GetValue<BoolData>().Value;
                    if (result == true)
                    {
                        Debug.unityLogger.Log("成功断开场景");
                    }
                    else
                    {
                        Debug.unityLogger.Log("断开场景失败");
                    }
                    currentScene.OnAfterDisconnect(result);
                    act();
                }, () =>
                {
                    Debug.unityLogger.LogError("NetPost", "断开场景超时");
                });
            });
            destroyQueue.AddAction(OnDestroy);

            if(onDisconnect != null)
                destroyQueue.AddAction(onDisconnect);
        }

        private void AddNetBehavior(NetBehavior behavior)
        {
            MonoActionPool.AddAction(() =>
            {
                clientNetBehaviorDict.Add(behavior.instanceId, behavior);
            });
        }

        private void RemoveNetBehavior(NetBehavior behavior)
        {
            MonoActionPool.AddAction(() =>
            {
                clientNetBehaviorDict.Remove(behavior.instanceId);
            });
        }

        void OnDestroy()
        {
            EventDispatcher.RemoveEventListener<NetBehavior>(NetSceneEvent.NetBehaviorAddToScene, AddNetBehavior);
            EventDispatcher.RemoveEventListener<NetBehavior>(NetSceneEvent.NetBehaviorRemoveFromScene, RemoveNetBehavior);
            currentServer.Disconnect();
        }

        /// <summary>
        /// 向场景服务端发送消息
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="value"></param>
        /// <param name="sendType"></param>
        public void SendData<T>(HandlerConst.RequestId eventId, T value, SendType sendType = SendType.TCP)
        {
            currentServer.Send<T>(eventId, currentSceneId, value, sendType);
        }

        /// <summary>
        /// 向服务端发送请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventId"></param>
        /// <param name="channelId"></param>
        /// <param name="value"></param>
        /// <param name="sendType"></param>
        /// <param name="callBack"></param>
        /// <param name="timeoutAct"></param>
        /// <param name="timeout"></param>
        public void Request<T>(HandlerConst.RequestId eventId, T value, 
            Action<Package> callBack, SendType sendType = SendType.TCP, Action timeoutAct = null, float timeout = 2)
        {
            currentServer.Request<T>(eventId, currentSceneId, value, sendType, callBack, timeoutAct, timeout);
        }
    }
}
