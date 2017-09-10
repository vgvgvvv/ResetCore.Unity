using UnityEngine;
using System.Collections;
using System;
using ResetCore.Event;
using ResetCore.Util;
using System.Collections.Generic;
using Protobuf.Data;

namespace ResetCore.NetPost
{
    public enum SendType
    {
        TCP,
        UDP
    }

    public static class ServerEvent
    {
        public static readonly string TcpOnCloseSocket = "ServerEvent.TcpOnCloseSocket";
        public static readonly string TcpOnConnect = "ServerEvent.TcpOnConnect";
        public static readonly string TcpOnError = "ServerEvent.TcpOnError";
        public static readonly string TcpOnListen = "ServerEvent.TcpOnListen";
        public static readonly string TcpOnReceive = "ServerEvent.TcpOnReceive";
        public static readonly string TcpOnSend = "ServerEvent.TcpOnSend";

        public static readonly string UdpOnBind = "ServerEvent.UdpOnBind";
        public static readonly string UdpOnError = "ServerEvent.UdpOnError";
        public static readonly string UdpOnListen = "ServerEvent.UdpOnListen";
        public static readonly string UdpOnReceive = "ServerEvent.UdpOnReceive";

        //针对回调的请求
        public static string GetResponseEvent(int requestId)
        {
            return "Response" + requestId;
        }

        //获取到服务器Id
        public static readonly string GetChannelId = "ServerEvent.GetChannelId";
    }

    public class BaseServer
    {
        //Tcp套接字
        private TcpSocket tcpSocket = new TcpSocket();
        //Udp套接字
        private UdpSocket udpSocket = new UdpSocket();

        //Tcp包接收器
        private PackageReciver tcpReciver;
        //Udp包接收器
        private PackageReciver udpReciver;

        //tcp同步锁
        private object tcpLockObject = new object();
        //udp同步锁
        private object udpLockObject = new object();

        //行为队列
        private ActionQueue handleQueue = new ActionQueue();

        //是否已经连接
        private bool _isConnect = false;
        public bool isConnect
        {
            get { return _isConnect; }
            set {  _isConnect = value; }
        }

        //频道Id
        public string channelId { get; private set; }

        public List<int> currentChannel { get; private set; }

        //请求回调池
        public static Dictionary<int, Action<Package>> responseHandlerPool = new Dictionary<int, Action<Package>>();

        public BaseServer()
        {
            
            isConnect = false;

            tcpReciver = new PackageReciver(this);
            udpReciver = new PackageReciver(this);

            tcpSocket.onCloseSocket += new TcpSocketCloseSocketDelegate(TcpOnCloseSocket);
            tcpSocket.onConnect += new TcpSocketConnectDelegate(TcpOnConnect);
            tcpSocket.onError += new TcpSocketErrorDelegate(TcpOnError);
            tcpSocket.onListen += new TcpSocketListenDelegate(TcpOnListen);
            tcpSocket.onReceive += new TcpSocketReceiveDelegate(TcpOnReceive);
            tcpSocket.onSend += new TcpSocketSendDelegate(TcpOnSend);

            udpSocket.onBind += new UdpSocketBindDelegate(UdpOnBind);
            udpSocket.onError += new UdpSocketErrorDelegate(UdpOnError);
            udpSocket.onListen += new UdpSocketListenDelegate(UdpOnListen);
            udpSocket.onReceive += new UdpSocketReceiveDelegate(UdpOnReceive);

            currentChannel = new List<int>();

            

            EventDispatcher.AddEventListener<string>(ServerEvent.GetChannelId, GetChannelId, this);
        }

        //设置频道Id
        private void GetChannelId(string channelId)
        {
            this.channelId = channelId;
            Debug.Log("频道id为" + channelId);
            EventDispatcher.RemoveEventListener<string>(ServerEvent.GetChannelId, GetChannelId, this);
        }

        #region 服务器公开行为
        /// <summary>
        /// 服务器连接
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="remoteTcpPort"></param>
        /// <param name="remoteUdpPort"></param>
        /// <param name="localUdpPort"></param>
        /// <param name="autoRebind"></param>
        public bool Connect(string remoteAddress, int remoteTcpPort
            , int remoteUdpPort, int localUdpPort, bool autoRebind = true, Action afterAct = null)
        {
            isConnect = true;
            bool udpBindSuccess = 
                udpSocket.BindRemoteEndPoint(remoteAddress, remoteUdpPort, localUdpPort, autoRebind);
            bool udpBeginReceive = udpSocket.BeginReceive();

            if(!udpBindSuccess || !udpBeginReceive)
            {
                Debug.LogError("Udp连接失败！");
                return false;
            }

            bool tcpConnect = tcpSocket.Connect(remoteAddress, remoteTcpPort);
            if (!tcpConnect)
            {
                Debug.LogError("Tcp连接失败！");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 服务器发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventId">事件Id</param>
        /// <param name="channelId">用户组Id</param>
        /// <param name="value">发送值</param>
        /// <param name="sendType">发送类型</param>
        public void Send<T>(int eventId, int channelId, T value, SendType sendType = SendType.TCP)
        {
            if(isConnect == false)
            {
                Debug.unityLogger.LogError("NetPost", "服务器未连接");
                return;
            }
            Package pkg = Package.MakePakage<T>(eventId, channelId, value, sendType);
            if (sendType == SendType.TCP)
            {
                tcpSocket.Send(pkg.totalData);
            }
            else
            {
                udpSocket.Send(pkg.totalData, pkg.totalLength);
            }
        }

        /// <summary>
        /// 服务器发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventId">事件Id</param>
        /// <param name="channelId">用户组Id</param>
        /// <param name="value">发送值</param>
        /// <param name="sendType">发送类型</param>
        public void Send<T>(HandlerConst.RequestId eventId, int channelId, T value, SendType sendType = SendType.TCP)
        {
            Send<T>((int)eventId, channelId, value, sendType);
        }

        /// <summary>
        /// 服务器断开连接
        /// </summary>
        public void Disconnect()
        {

            isConnect = false;
            Debug.Log("断开TCP");
            tcpSocket.Disconnect();
            Debug.Log("断开UDP");
            udpSocket.Stop();

            tcpReciver.Reset();
            udpReciver.Reset();

        }

        /// <summary>
        /// 注册频道
        /// </summary>
        /// <param name="loginChannelList">要登入的频道</param>
        /// <param name="logoutChannelList">要登出的频道</param>
        public void Regist(List<int> loginChannelList, List<int> logoutChannelList, Action<Package> callback = null)
        {
            string loginListStr = loginChannelList.ConverToString();
            string logoutListStr = logoutChannelList.ConverToString();
            RegistData data = new RegistData();
            data.LoginChannel = loginListStr;
            data.LogoutChannel = logoutListStr;

            Request<RegistData>(HandlerConst.RequestId.RegistChannelHandler, -1, data, SendType.TCP, callback);
        }


        /// <summary>
        /// 发送消息并且等待回调
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="channelId"></param>
        /// <param name="sendType"></param>
        /// <param name="callBack"></param>
        /// <param name="timeout"></param>
        public void Request(HandlerConst.RequestId eventId, int channelId, SendType sendType,
            Action<Package> callBack, Action timeoutAct = null, float timeout = 2)
        {
            Request<System.Object>(eventId, channelId, null, sendType, callBack, timeoutAct, timeout);
        }

        /// <summary>
        /// 发送消息并且等待回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventId"></param>
        /// <param name="channelId"></param>
        /// <param name="value"></param>
        /// <param name="sendType"></param>
        /// <param name="callBack"></param>
        public void Request<T>(HandlerConst.RequestId eventId, int channelId, T value, SendType sendType, 
            Action<Package> callBack,Action timeoutAct = null, float timeout = 2)
        {
            if (isConnect == false)
            {
                Debug.unityLogger.LogError("NetPost", "服务器未连接");
                return;
            }
            Package pkg = Package.MakePakage<T>((int)eventId, channelId, value, sendType);
            if (sendType == SendType.TCP)
            {
                tcpSocket.Send(pkg.totalData);
            }
            else
            {
                udpSocket.Send(pkg.totalData, pkg.totalLength);
            }

            //如果没有回调函数则直接返回
            if (callBack == null)
                return;

            EventDispatcher.AddEventListener<Package>(ServerEvent.GetResponseEvent(pkg.requestId), HandleRequeset, this);
            responseHandlerPool.Add(pkg.requestId, callBack);
            //超时检查
            CoroutineTaskManager.Instance.WaitSecondTodo(() =>
            {
                if (responseHandlerPool.ContainsKey(pkg.requestId))
                {
                    EventDispatcher.RemoveEventListener<Package>(ServerEvent.GetResponseEvent(pkg.requestId), HandleRequeset, this);
                    responseHandlerPool.Remove(pkg.requestId);
                    Debug.unityLogger.LogError("NetPost", "请求超时，请求Id为" + pkg.requestId + " 处理Id为" + EnumEx.GetValue<HandlerConst.RequestId>(pkg.eventId));
                    if(timeoutAct != null)
                    {
                        timeoutAct();
                    }
                }
            }, 100000);
;        }

        //回调
        private void HandleRequeset(Package pkg)
        {
            EventDispatcher.RemoveEventListener<Package>(ServerEvent.GetResponseEvent(pkg.requestId), HandleRequeset, this);
            responseHandlerPool[pkg.requestId](pkg);
            responseHandlerPool.Remove(pkg.requestId);
        }

       

        #endregion

        #region Tcp回调行为

        private void TcpOnCloseSocket(CloseType type, SocketState state, Exception e = null)
        {
            MonoActionPool.AddAction(() =>
            {
                EventDispatcher.TriggerEvent<CloseType, SocketState, Exception>
                (ServerEvent.TcpOnCloseSocket, type, state, e);
                //Todo
            });
        }

        private void TcpOnConnect(Exception e = null)
        {
            MonoActionPool.AddAction(() =>
            {
                tcpSocket.BeginReceive();
                EventDispatcher.TriggerEvent<Exception>(ServerEvent.TcpOnConnect, e);
                Debug.unityLogger.Log("Tcp Socket已连接");
            });
        }

        private void TcpOnError(SocketState state, Exception e = null)
        {
            MonoActionPool.AddAction(() =>
            {
                Debug.unityLogger.LogError("ServerError", "在" + state.ToString() + "下报错");
                Debug.LogException(e);
                EventDispatcher.TriggerEvent<SocketState, Exception>(ServerEvent.TcpOnError, state, e);
                //Todo
                Disconnect();
            });
        }

        private void TcpOnListen(Exception e = null)
        {
            MonoActionPool.AddAction(() =>
            {
                EventDispatcher.TriggerEvent<Exception>(ServerEvent.TcpOnListen, e);
                //Todo
            });
        }

        private void TcpOnReceive(int len, byte[] data)
        {
            MonoActionPool.AddAction(() =>
            {
                EventDispatcher.TriggerEvent<int, byte[]>(ServerEvent.TcpOnReceive, len, data);
                //Todo
                lock (tcpLockObject)
                {
                    tcpReciver.ReceivePackage(len, data);
                }
            });
        }

        private void TcpOnSend(int len)
        {
            MonoActionPool.AddAction(() =>
            {
                EventDispatcher.TriggerEvent<int>(ServerEvent.TcpOnSend, len);
                //Todo
            });
        }
        #endregion Tcp回调行为

        #region Udp回调行为
        private void UdpOnBind(Exception e = null)
        {
            MonoActionPool.AddAction(() =>
            {
                EventDispatcher.TriggerEvent<Exception>(ServerEvent.UdpOnBind, e);
                //Todo
                Debug.unityLogger.Log("Udp Socket已经绑定");
            });
        }

        private void UdpOnError(SocketState state, Exception e = null)
        {
            MonoActionPool.AddAction(() =>
            {
                Debug.unityLogger.LogError("ServerError", "在" + state.ToString() + "下报错");
                Debug.LogException(e);
                EventDispatcher.TriggerEvent<SocketState, Exception>(ServerEvent.UdpOnError, state, e);
                //Todo
                Disconnect();
            });
        }

        private void UdpOnListen(Exception e = null)
        {
            MonoActionPool.AddAction(() =>
            {
                EventDispatcher.TriggerEvent<Exception>(ServerEvent.UdpOnListen, e);
                //Todo
            });
        }

        private void UdpOnReceive(int len, byte[] data, string remoteAddress, int remotePort)
        {
            MonoActionPool.AddAction(() =>
            {
                EventDispatcher.TriggerEvent<int, byte[], string, int>
               (ServerEvent.UdpOnReceive, len, data, remoteAddress, remotePort);
                //Todo
                lock (tcpLockObject)
                {
                    Debug.unityLogger.Log("Udp Socket接收包，长度为：" + len);
                    udpReciver.ReceivePackage(len, data);
                }
            });
        }

        #endregion Udp回调行为

    }
}
