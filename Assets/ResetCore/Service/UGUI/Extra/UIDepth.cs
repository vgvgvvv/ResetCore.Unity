using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIDepth : MonoBehaviour
{
    public int order;
    public bool isUI = true;
    public bool hasEvent = false;
    void Start()
    {
        if (isUI)
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
            if (hasEvent)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
        }
        else
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>();

            foreach (Renderer render in renders)
            {
                render.sortingOrder = order;
            }
        }
        
    }
}