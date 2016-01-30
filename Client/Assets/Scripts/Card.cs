using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public CardType type;
    
    public CardPanel parent;
    public CardPanel target;
    public int index = 0;
    public int indexTarget = 0;

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        index = parent.GetChildIndex(this);
        parent.RemoveChild(this);
        index = parent.GetChildIndex(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        //sem jo izpustil nekje vmes med paneli, daj jo kar nazaj na izhodišče
        if(target == null)
        {
            parent.AddChild(this, index);
        }
        //dodam ga nazaj v isti panel
        else if(parent == target)
        {
            parent.AddChild(this, indexTarget);
        }
        //dodam ga v grugi panel
        else
        {
            target.AddChild(this, indexTarget);
        }
    }
}
