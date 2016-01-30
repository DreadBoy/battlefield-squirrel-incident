using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class CardPanel : MonoBehaviour, IDropHandler
{
    public Boolean flexibleHeight = true;

    public List<GameObject> cards = new List<GameObject>();

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
        var card = eventData.pointerDrag.GetComponent<Card>();
        card.target = this;
        /*
                var grid = GetComponent<GridLayoutGroup>();
                var rectTransform = GetComponent<RectTransform>();

                Vector3[] corners = new Vector3[4];
                rectTransform.GetLocalCorners(corners);

                var position = Input.mousePosition - new Vector3(corners[0].y, corners[0].x);
                int rows = (int)Math.Ceiling((double)cards.Count / grid.constraintCount);
                var row = (int)Math.Floor(position.y / (rectTransform.rect.height / rows)) - 1;
                var column = (int)Math.Ceiling(position.x / (rectTransform.rect.width / grid.constraintCount));
                card.indexTarget = row * 8 + column;
                */
    }

    public void RemoveChild(Card card)
    {
        cards.Remove(card.gameObject);
        card.gameObject.transform.SetParent(this.transform.parent);
        FlexibleHeight();
    }


    public void DeleteChild(Card card)
    {
        cards.Remove(card.gameObject);
        GameObject.Destroy(card.gameObject);
        FlexibleHeight();

    }

    public void AddChild(Card card)
    {
        card.gameObject.GetComponent<Card>().parent = this;
        card.gameObject.GetComponent<Card>().target = null;
        card.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        cards.Add(card.gameObject);
        foreach (var c in cards)
            c.transform.SetParent(transform.parent);
        foreach (var c in cards)
            c.transform.SetParent(transform);
        FlexibleHeight();
    }
    internal void AddChild(Card card, int index)
    {
        if (index > cards.Count)
            index = cards.Count;
        card.gameObject.GetComponent<Card>().parent = this;
        card.gameObject.GetComponent<Card>().target = null;
        card.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        cards.Insert(index, card.gameObject);
        foreach (var c in cards)
            c.transform.SetParent(transform.parent);
        foreach (var c in cards)
            c.transform.SetParent(transform);
        FlexibleHeight();
    }

    internal int GetChildIndex(Card card)
    {
        return cards.FindIndex(c => c.gameObject == card.gameObject);
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
