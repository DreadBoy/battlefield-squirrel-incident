using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CardTypeManager : MonoBehaviour {

    public Dictionary<CardType, GameObject> prefabs = new Dictionary<CardType, GameObject>();
    
	void Start () {
        foreach (var prefab in Resources.LoadAll<GameObject>("Cards"))
        {
            Card cardPrefab = prefab.GetComponent<Card>();
            prefabs.Add(cardPrefab.type, prefab);
        }
        
    }
	
	void Update () {
	
	}

}