using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using ResetCore.Event;

namespace ResetCore.UGUI
{
    [AddComponentMenu("UI/Extra/UIEventListener")]
    public class UIEventListener : UnityEngine.EventSystems.EventTrigger
    {
        public delegate void VoidDelegate(GameObject go);
        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;
        public VoidDelegate onUpdateSelect;

        static public UIEventListener Get(GameObject go)
        {
            UIEventListener listener = go.GetComponent<UIEventListener>();
            if (listener == null) listener = go.AddComponent<UIEventListener>();
            return listener;
        }

#region 重载触发事件的按钮函数
#if EVENT
        [SerializeField]
        public string pointerClickEventName;
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null)
            {
                onClick(gameObject);
                if (!string.IsNullOrEmpty(pointerClickEventName))
                {
                    EventDispatcher.TriggerEvent(pointerClickEventName);
                }
            }
        }
        [SerializeField]
        public string pointDownEventName;
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null)
            {
                onDown(gameObject);
                if (!string.IsNullOrEmpty(pointDownEventName))
                {
                    EventDispatcher.TriggerEvent(pointDownEventName);
                }
            }
        }
        public string pointEnterEventName;
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null)
            {
                onEnter(gameObject);
                if (!string.IsNullOrEmpty(pointEnterEventName))
                {
                    EventDispatcher.TriggerEvent(pointEnterEventName);
                }
            }
        }
        public string pointExitEventName;
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null)
            {
                onExit(gameObject);
                if (!string.IsNullOrEmpty(pointExitEventName))
                {
                    EventDispatcher.TriggerEvent(pointExitEventName);
                }
            }
        }
        public string pointUpEventName;
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null)
            {
                onUp(gameObject);
                if (!string.IsNullOrEmpty(pointUpEventName))
                {
                    EventDispatcher.TriggerEvent(pointUpEventName);
                }
            }
        }
        public string selectEventName;
        public override void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null)
            {
                onSelect(gameObject);
                if (!string.IsNullOrEmpty(selectEventName))
                {
                    EventDispatcher.TriggerEvent(selectEventName);
                }
            }
        }

        public string updateSelectedEventName;
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (onUpdateSelect != null)
            {
                onUpdateSelect(gameObject);
                if (!string.IsNullOrEmpty(updateSelectedEventName))
                {
                    EventDispatcher.TriggerEvent(updateSelectedEventName);
                }
            }
        }
#else
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null)
            {
                onClick(gameObject);
            }
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null)
            {
                onDown(gameObject);
            }
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null)
            {
                onEnter(gameObject);
            }
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null)
            {
                onExit(gameObject);
            }
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null)
            {
                onUp(gameObject);
            }
        }
        public override void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null)
            {
                onSelect(gameObject);
            }
        }

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (onUpdateSelect != null)
            {
                onUpdateSelect(gameObject);
            }
        }
#endif
#endregion
    }
}
