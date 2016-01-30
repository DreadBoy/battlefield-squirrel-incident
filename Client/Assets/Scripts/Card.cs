using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public CardType type;
    
    public CardPanel parent;
    public int index = -1;

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        parent.RemoveChild(this);
        index = parent.GetChildIndex();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if(!transform.parent.GetComponent<CardPanel>())
        {
            parent.AddChild(this);
        }
    }
}
