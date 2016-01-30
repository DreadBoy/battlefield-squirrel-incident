using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour, IDropHandler
{
    public Boolean flexibleHeight = true;

    List<GameObject> cards = new List<GameObject>();

    public void Start()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<Card>().parent = this;
            cards.Add(child.gameObject);
        }
        FlexibleHeight();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!cards.Contains(eventData.pointerDrag))
        {
            AddChild(eventData.pointerDrag.GetComponent<Card>());
        }
    }

    public void RemoveChild(Card card)
    {
        cards.Remove(card.gameObject);
        card.gameObject.transform.SetParent(this.transform.parent);
        FlexibleHeight();
    }

    public void AddChild(Card card)
    {
        cards.Add(card.gameObject);
        card.gameObject.transform.SetParent(transform);
        card.gameObject.GetComponent<Card>().parent = this;
        FlexibleHeight();
    }

    void FlexibleHeight()
    {
        if (!flexibleHeight)
            return;
        var grid = GetComponent<GridLayoutGroup>();
        int rows = (int)Math.Ceiling((double)cards.Count / grid.constraintCount);
        if (rows < 1)
            rows = 1;
        var height = rows * grid.cellSize.y + (rows - 1 + 2) * grid.spacing.y;
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, height);
    }
}
