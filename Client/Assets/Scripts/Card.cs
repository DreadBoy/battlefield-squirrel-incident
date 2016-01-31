using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{

    public CardType type;
    
    public CardPanel parent;
    public CardPanel target;
    public Card targetCard;
    public int index = 0;

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        index = parent.GetChildIndex(this);
        parent.RemoveChild(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        //dropnem ga na drugo kartico, torej vzami index in daj na panel
        if (targetCard != null)
        {
            index = targetCard.parent.GetChildIndex(targetCard);
            targetCard.parent.AddChild(this, index);
            target = null;
            targetCard = null;
            return;
        }
        //sem jo izpustil nekje vmes med paneli, daj jo kar nazaj na izhodišče
        if (target == null)
        {
            parent.AddChild(this, index);
            target = null;
            targetCard = null;
            return;
        }
        //dodam ga samo nazaj v isti panel na zadnje mesto
        if(target == parent)
        {
            parent.AddChild(this);
        }
        //dodam ga v drugi panel na konec
        else
        {
            target.AddChild(this);
        }
        target = null;
        targetCard = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var card = eventData.pointerDrag.GetComponent<Card>();
        card.targetCard = this;
    }
}
