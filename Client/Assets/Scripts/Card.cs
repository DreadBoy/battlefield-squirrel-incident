using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour {

    public CardType type;

    public CardPanel parent;

    private CardManager manager;

    void Start () {
        manager = FindObjectOfType<CardManager>();
	}
	
    public void changeParent()
    {
        manager.changeParent(this);
    }
}
