using UnityEngine;
using System.Collections;

namespace ResetCore.Util
{
    public class GameObjectCallBack : UnityEngine.EventSystems.EventTrigger
    {
        

        public delegate void AwakeCallback();
        public AwakeCallback awakeCallback;
        void Awake()
        {
            if(awakeCallback!= null)
            {
                awakeCallback();
            }
        }

        public delegate void FixedUpdateCallback();
        public FixedUpdateCallback fixedUpdate;
        void FixedUpdate()
        {
            if(fixedUpdate != null)
            {
                fixedUpdate();
            }
        }

        public delegate void LateUpdateCallback();
        public LateUpdateCallback lateUpdate;
        void LateUpdate()
        {
            if (lateUpdate != null)
            {
                lateUpdate();
            }
        }

        public delegate void OnAnimatorIkCallback();
        public OnAnimatorIkCallback onAnimatorIK;
        void OnAnimatorIK()
        {
            if (onAnimatorIK != null)
            {
                onAnimatorIK();
            }
        }

        public delegate void OnAnimatorMoveCallback();
        public OnAnimatorMoveCallback onAnimatorMove;
        void OnAnimatorMove()
        {
            if (onAnimatorMove != null)
            {
                onAnimatorMove();
            }
        }

        public delegate void OnApplicationFocusCallback();
        public OnApplicationFocusCallback onApplicationFocus;
        void OnApplicationFocus()
        {
            if (onApplicationFocus != null)
            {
                onApplicationFocus();
            }
        }

        public delegate void OnApplicationPauseCallback();
        public OnApplicationPauseCallback onApplicationPause;
        void OnApplicationPause()
        {
            if (onApplicationPause != null)
            {
                onApplicationPause();
            }
        }

        public delegate void OnApplicationQuitCallback();
        public OnApplicationQuitCallback onApplicationQuit;
        void OnApplicationQuit()
        {
            if (onApplicationQuit != null)
            {
                onApplicationQuit();
            }
        }

        public delegate void OnAudioFilterReadCallback(float[] data, int channels);
        public OnAudioFilterReadCallback onAudioFilterRead;
        void OnAudioFilterRead(float[] data, int channels)
        {
            if (onAudioFilterRead != null)
            {
                onAudioFilterRead(data, channels);
            }
        }

        public delegate void OnBecameInvisibleCallback();
        public OnBecameInvisibleCallback onBecameInvisible;
        void OnBecameInvisible()
        {
            if (onBecameInvisible != null)
            {
                onBecameInvisible();
            }
        }

        public delegate void OnBecameVisibleCallback();
        public OnBecameVisibleCallback onBecameVisible;
        void OnBecameVisible()
        {
            if (onBecameVisible != null)
            {
                onBecameVisible();
            }
        }

        public delegate void OnCollisionEnterCallback(Collision collision);
        public OnCollisionEnterCallback onCollisionEnter;
        void OnCollisionEnter(Collision collision)
        {
            if (onCollisionEnter != null)
            {
                onCollisionEnter(collision);
            }
        }

        public delegate void OnCollisionEnter2DCallback(Collision2D coll);
        public OnCollisionEnter2DCallback onCollisionEnter2D;
        void OnCollisionEnter2D(Collision2D coll)
        {
            if (onCollisionEnter2D != null)
            {
                onCollisionEnter2D(coll);
            }
        }

        public delegate void OnCollisionExitCallback(Collision collision);
        public OnCollisionExitCallback onCollisionExit;
        void OnCollisionExit(Collision collision)
        {
            if (onCollisionExit != null)
            {
                onCollisionExit(collision);
            }
        }

        public delegate void OnCollisionExit2DCallback(Collision2D coll);
        public OnCollisionExit2DCallback onCollisionExit2D;
        void OnCollisionExit2D(Collision2D coll)
        {
            if (onCollisionExit2D != null)
            {
                onCollisionExit2D(coll);
            }
        }

        public delegate void OnCollisionStayCallback(Collision collision);
        public OnCollisionStayCallback onCollisionStay;
        void OnCollisionStay(Collision collision)
        {
            if (onCollisionStay != null)
            {
                onCollisionStay(collision);
            }
        }

        public delegate void OnCollisionStay2DCallback(Collision2D coll);
        public OnCollisionStay2DCallback onCollisionStay2D;
        void OnCollisionStay2D(Collision2D coll)
        {
            if (onCollisionStay2D != null)
            {
                onCollisionStay2D(coll);
            }
        }

        public delegate void OnConnectedToServerCallback();
        public OnConnectedToServerCallback onConnectedToServer;
        void OnConnectedToServer()
        {
            if (onConnectedToServer != null)
            {
                onConnectedToServer();
            }
        }

        public delegate void OnControllerColliderHitCallback(ControllerColliderHit hit);
        public OnControllerColliderHitCallback onControllerColliderHit;
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (onControllerColliderHit != null)
            {
                onControllerColliderHit(hit);
            }
        }

        public delegate void OnDestroyCallback();
        public OnDestroyCallback onDestroy;
        void OnDestroy()
        {
            if (onDestroy != null)
            {
                onDestroy();
            }
        }

        public delegate void OnDisableCallback();
        public OnDisableCallback onDisable;
        void OnDisable()
        {
            if (onDisable != null)
            {
                onDisable();
            }
        }

        public delegate void OnDisconnectedFromServerCallback(NetworkDisconnection info);
        public OnDisconnectedFromServerCallback onDisconnectedFromServer;
        void OnDisconnectedFromServer(NetworkDisconnection info)
        {
            if (onDisconnectedFromServer != null)
            {
                onDisconnectedFromServer(info);
            }
        }

        public delegate void OnDrawGizmosCallback();
        public OnDrawGizmosCallback onDrawGizmos;
        void OnDrawGizmos()
        {
            if (onDrawGizmos != null)
            {
                onDrawGizmos();
            }
        }

        public delegate void OnDrawGizmosSelectedCallback();
        public OnDrawGizmosSelectedCallback onDrawGizmosSelected;
        void OnDrawGizmosSelected()
        {
            if (onDrawGizmosSelected != null)
            {
                onDrawGizmosSelected();
            }
        }

        public delegate void OnEnableCallback();
        public OnEnableCallback onEnable;
        void OnEnable()
        {
            if (onEnable != null)
            {
                onEnable();
            }
        }

        public delegate void OnFailedToConnectCallback(NetworkConnectionError error);
        public OnFailedToConnectCallback onFailedToConnect;
        void OnFailedToConnect(NetworkConnectionError error)
        {
            if (onFailedToConnect != null)
            {
                onFailedToConnect(error);
            }
        }

        public delegate void OnFailedToConnectToMasterServerCallback(NetworkConnectionError info);
        public OnFailedToConnectToMasterServerCallback onFailedToConnectToMasterServer;
        void OnFailedToConnectToMasterServer(NetworkConnectionError info)
        {
            if (onFailedToConnectToMasterServer != null)
            {
                onFailedToConnectToMasterServer(info);
            }
        }

        public delegate void OnGUICallback();
        public OnGUICallback onGUI;
        void OnGUI()
        {
            if (onGUI != null)
            {
                onGUI();
            }
        }

        public delegate void OnJointBreakCallback(float breakForce);
        public OnJointBreakCallback onJointBreak;
        void OnJointBreak(float breakForce)
        {
            if (onJointBreak != null)
            {
                onJointBreak(breakForce);
            }
        }

        public delegate void OnJointBreak2DCallback(Joint2D brokenJoint);
        public OnJointBreak2DCallback onJointBreak2D;
        void OnJointBreak2D(Joint2D brokenJoint)
        {
            if (onJointBreak2D != null)
            {
                onJointBreak2D(brokenJoint);
            }
        }
#if !UNITY_5
        public delegate void OnLevelWasLoadedCallback(int level);
        public OnLevelWasLoadedCallback onLevelWasLoaded;
        void OnLevelWasLoaded(int level)
        {
            if (onLevelWasLoaded != null)
            {
                onLevelWasLoaded(level);
            }
        }
#endif

        public delegate void OnMasterServerEventCallback(MasterServerEvent msEvent);
        public OnMasterServerEventCallback onMasterServerEvent;
        void OnMasterServerEvent(MasterServerEvent msEvent)
        {
            if (onMasterServerEvent != null)
            {
                onMasterServerEvent(msEvent);
            }
        }

        public delegate void OnMouseDownCallback();
        public OnMouseDownCallback onMouseDown;
        void OnMouseDown()
        {
            if (onMouseDown != null)
            {
                onMouseDown();
            }
        }

        public delegate void OnMouseDragCallback();
        public OnMouseDragCallback onMouseDrag;
        void OnMouseDrag()
        {
            if (onMouseDrag != null)
            {
                onMouseDrag();
            }
        }

        public delegate void OnMouseEnterCallback();
        public OnMouseEnterCallback onMouseEnter;
        void OnMouseEnter()
        {
            if (onMouseEnter != null)
            {
                onMouseEnter();
            }
        }

        public delegate void OnMouseOverCallback();
        public OnMouseOverCallback onMouseOver;
        void OnMouseOver()
        {
            if (onMouseOver != null)
            {
                onMouseOver();
            }
        }

        public delegate void OnMouseExitCallback();
        public OnMouseExitCallback onMouseExit;
        void OnMouseExit()
        {
            if (onMouseExit != null)
            {
                onMouseExit();
            }
        }

        public delegate void OnMouseUpCallback();
        public OnMouseUpCallback onMouseUp;
        void OnMouseUp()
        {
            if (onMouseUp != null)
            {
                onMouseUp();
            }
        }

        public delegate void OnMouseUpAsButtonCallback();
        public OnMouseUpAsButtonCallback onMouseUpAsButton;
        void OnMouseUpAsButton()
        {
            if (onMouseUpAsButton != null)
            {
                onMouseUpAsButton();
            }
        }

        public delegate void OnNetworkInstantiateCallback(NetworkMessageInfo info);
        public OnNetworkInstantiateCallback onNetworkInstantiate;
        void OnNetworkInstantiate(NetworkMessageInfo info)
        {
            if (onNetworkInstantiate != null)
            {
                onNetworkInstantiate(info);
            }
        }

        public delegate void OnParticleCollisionCallback(GameObject other);
        public OnParticleCollisionCallback onParticleCollision;
        void OnParticleCollision(GameObject other)
        {
            if (onParticleCollision != null)
            {
                onParticleCollision(other);
            }
        }

        public delegate void OnPlayerConnectedCallback(NetworkPlayer player);
        public OnPlayerConnectedCallback onPlayerConnected;
        void OnPlayerConnected(NetworkPlayer player)
        {
            if (onPlayerConnected != null)
            {
                onPlayerConnected(player);
            }
        }

        public delegate void OnPlayerDisconnectedCallback(NetworkPlayer player);
        public OnPlayerDisconnectedCallback onPlayerDisconnected;
        void OnPlayerDisconnected(NetworkPlayer player)
        {
            if (onPlayerDisconnected != null)
            {
                onPlayerDisconnected(player);
            }
        }

        public delegate void OnPostRenderCallback();
        public OnPostRenderCallback onPostRender;
        void OnPostRender()
        {
            if (onPostRender != null)
            {
                onPostRender();
            }
        }

        public delegate void OnPreCullCallback();
        public OnPreCullCallback onPreCull;
        void OnPreCull()
        {
            if (onPreCull != null)
            {
                onPreCull();
            }
        }

        public delegate void OnPreRenderCallback();
        public OnPreRenderCallback onPreRender;
        void OnPreRender()
        {
            if (onPreRender != null)
            {
                onPreRender();
            }
        }

        public delegate void OnRenderImageCallback(RenderTexture src, RenderTexture dest);
        public OnRenderImageCallback onRenderImage;
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (onRenderImage != null)
            {
                onRenderImage(src, dest);
            }
        }

        public delegate void OnRenderObjectCallback();
        public OnRenderObjectCallback onRenderObject;
        void OnRenderObject()
        {
            if (onRenderObject != null)
            {
                onRenderObject();
            }
        }

        public delegate void OnSerializeNetworkViewCallback(BitStream stream, NetworkMessageInfo info);
        public OnSerializeNetworkViewCallback onSerializeNetworkView;
        void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            if (onSerializeNetworkView != null)
            {
                onSerializeNetworkView(stream, info);
            }
        }

        public delegate void OnServerInitializedCallback();
        public OnServerInitializedCallback onServerInitialized;
        void OnServerInitialized()
        {
            if (onServerInitialized != null)
            {
                onServerInitialized();
            }
        }

        public delegate void OnTransformChildrenChangedCallback();
        public OnTransformChildrenChangedCallback onTransformChildrenChanged;
        void OnTransformChildrenChanged()
        {
            if (onTransformChildrenChanged != null)
            {
                onTransformChildrenChanged();
            }
        }

        public delegate void OnTransformParentChangedCallback();
        public OnTransformParentChangedCallback onTransformParentChanged;
        void OnTransformParentChanged()
        {
            if (onTransformParentChanged != null)
            {
                onTransformParentChanged();
            }
        }

        public delegate void OnTriggerEnterCallback(Collider other);
        public OnTriggerEnterCallback onTriggerEnter;
        void OnTriggerEnter(Collider other)
        {
            if (onTriggerEnter != null)
            {
                onTriggerEnter(other);
            }
        }

        public delegate void OnTriggerEnter2DCallback(Collider2D other);
        public OnTriggerEnter2DCallback onTriggerEnter2D;
        void OnTriggerEnter2D(Collider2D other)
        {
            if (onTriggerEnter2D != null)
            {
                onTriggerEnter2D(other);
            }
        }

        public delegate void OnTriggerExitCallback(Collider other);
        public OnTriggerExitCallback onTriggerExit;
        void OnTriggerExit(Collider other)
        {
            if (onTriggerExit != null)
            {
                onTriggerExit(other);
            }
        }

        public delegate void OnTriggerExit2DCallback(Collider2D other);
        public OnTriggerExit2DCallback onTriggerExit2D;
        void OnTriggerExit2D(Collider2D other)
        {
            if (onTriggerExit2D != null)
            {
                onTriggerExit2D(other);
            }
        }

        public delegate void OnTriggerStayCallback(Collider other);
        public OnTriggerStayCallback onTriggerStay;
        void OnTriggerStay(Collider other)
        {
            if (onTriggerStay != null)
            {
                onTriggerStay(other);
            }
        }

        public delegate void OnTriggerStay2DCallback(Collider2D other);
        public OnTriggerStay2DCallback onTriggerStay2D;
        void OnTriggerStay2D(Collider2D other)
        {
            if (onTriggerStay2D != null)
            {
                onTriggerStay2D(other);
            }
        }

        public delegate void OnValidateCallback();
        public OnValidateCallback onValidate;
        void OnValidate()
        {
            if (onValidate != null)
            {
                onValidate();
            }
        }

        public delegate void OnWillRenderObjectCallback();
        public OnWillRenderObjectCallback onWillRenderObject;
        void OnWillRenderObject()
        {
            if (onWillRenderObject != null)
            {
                onWillRenderObject();
            }
        }

        public delegate void ResetCallback();
        public ResetCallback reset;
        void Reset()
        {
            if (reset != null)
            {
                reset();
            }
        }

        public delegate void StartCallback();
        public StartCallback start;
        void Start()
        {
            if (start != null)
            {
                start();
            }
        }

        public delegate void UpdateCallback();
        public UpdateCallback update;
        void Update()
        {
            if (update != null)
            {
                update();
            }
        }
    }

}
