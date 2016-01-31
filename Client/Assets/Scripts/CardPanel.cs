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
    public Button EndTurn;

    public int maxCards;

    public void OnDrop(PointerEventData eventData)
    {
        var card = eventData.pointerDrag.GetComponent<Card>();
        card.target = this;
        if (card.target.cards.Count >= card.target.maxCards)
            card.target = null;
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
        CheckPattern();
    }

    public void CreateChild(GameObject gameObject)
    {
        gameObject.GetComponent<Card>().parent = this;
        gameObject.GetComponent<Card>().target = null;
        cards.Add(gameObject);
        gameObject.transform.SetParent(transform);
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        FlexibleHeight();
        CheckPattern();
    }

    public void AddChild(Card card)
    {
        if(card.target.cards.Count >= card.target.maxCards)
        {
            card.target = card.parent;
            AddChild(card);
            return;
        }
        card.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        card.target.cards.Add(card.gameObject);
        foreach (var c in card.target.cards)
            c.transform.SetParent(card.target.transform.parent);
        foreach (var c in card.target.cards)
        {
            c.transform.SetParent(card.target.transform);
            card.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
        card.parent = this;
        card.target = null;
        FlexibleHeight();
        CheckPattern();
    }
    internal void AddChild(Card card, int index)
    {
        if (card.parent.cards.Count >= card.parent.maxCards)
        {
            card.target = card.parent;
            AddChild(card);
            return;
        }
        if (index > cards.Count)
            index = cards.Count;
        card.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        card.target.cards.Insert(index, card.gameObject);
        foreach (var c in card.target.cards)
            c.transform.SetParent(card.target.transform.parent);
        foreach (var c in card.target.cards)
        {
            c.transform.SetParent(card.target.transform);
            card.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
        card.parent = this;
        card.target = null;
        FlexibleHeight();
        CheckPattern();
    }

    internal int GetChildIndex(Card card)
    {
        return cards.FindIndex(c => c.gameObject == card.gameObject);
    }

    void CheckPattern()
    {
        if (EndTurn == null)
            return;
        if (cards.Count % 2 != 0)
        {
            EndTurn.gameObject.SetActive(false);
            return;
        }
        var valid = true;
        for (int i = 0; i < cards.Count - 1; i+=2)
        {
            var type1 = (int)cards[i].GetComponent<Card>().type;
            var type2 = (int)cards[i + 1].GetComponent<Card>().type;
            if (!(10 <= type1 && type1 <= 11 && 0 <= type2 && type2 <= 9))
            {
                valid = false;
                break;
            }
        }
        EndTurn.gameObject.SetActive(valid);
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
