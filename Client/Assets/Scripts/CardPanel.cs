using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour
{
    public Boolean flexibleHeight = true;

    public List<GameObject> cards = new List<GameObject>();

    public int maxCards;

    GameObject button;

    void Start()
    {
        button = GameObject.Find("EndPhase");
    }

    public void DeleteChild(Card card)
    {
        cards.Remove(card.gameObject);
        GameObject.Destroy(card.gameObject);
        FlexibleHeight();
    }


    public void RemoveChild(Card card)
    {
        cards.Remove(card.gameObject);
        card.gameObject.transform.SetParent(transform.parent);
        FlexibleHeight();
    }

    public void CreateChild(GameObject gameObject)
    {
        gameObject.GetComponent<Card>().parent = this;
        cards.Add(gameObject);
        gameObject.transform.SetParent(transform);
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        FlexibleHeight();
    }

    public bool AddChild(Card card)
    {
        cards.Add(card.gameObject);
        foreach (var c in cards)
            c.transform.SetParent(transform.parent);
        foreach (var c in cards)
        {
            c.transform.SetParent(transform);
            card.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
        card.parent = this;
        FlexibleHeight();

        return true;
    }

    private bool CheckPattern()
    {
        if (cards.Count % 2 != 0)
            return false;

        for (int i = 0; i < cards.Count - 1; i += 2)
        {
            var first = (int)cards[i].GetComponent<Card>().type;
            var second = (int)cards[i+1].GetComponent<Card>().type;
            if (!(10 <= first && first <= 11 && 0 <= second && second <= 9))
                return false;
        }
        return true;
    }

    internal bool hasSpace()
    {
        return cards.Count < maxCards;
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
