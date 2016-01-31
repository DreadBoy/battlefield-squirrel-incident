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

    public void OnDrop(PointerEventData eventData)
    {
        var card = eventData.pointerDrag.GetComponent<Card>();
        card.target = this;
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

    public void CreateChild(GameObject gameObject)
    {
        gameObject.GetComponent<Card>().parent = this;
        gameObject.GetComponent<Card>().target = null;
        cards.Add(gameObject);
        gameObject.transform.SetParent(transform);
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
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
        {
            c.transform.SetParent(transform);
            card.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
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
        {
            c.transform.SetParent(transform);
            card.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
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
