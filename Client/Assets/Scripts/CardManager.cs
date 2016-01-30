﻿using UnityEngine;
using System.Linq;

[RequireComponent(typeof(CardTypeManager))]
public class CardManager : MonoBehaviour
{

    public NetworkManagerClient client;
    public CardPanel hand;
    public CardPanel command;
    private CardTypeManager cardTypeManager;

    void Start()
    {
        if (client == null)
            client = FindObjectOfType<NetworkManagerClient>();
        client.OnNewCardsEvent += new NetworkManagerClient.NewCardsEventHandler(newCards);
        cardTypeManager = GetComponent<CardTypeManager>();
    }

    public void EndTurnAndSendMessage()
    {
        if (!hand || !command)
            return;

        var message = new NetworkManagerClient.PlayerHandMessage()
        {
            hand = hand.cards.Select<GameObject, int>(go => (int)go.GetComponent<Card>().type).ToArray(),
            command = command.cards.Select<GameObject, int>(go => (int)go.GetComponent<Card>().type).ToArray(),
            connectionId = client.client.connection.connectionId
        };
        while (command.cards.Count > 0)
        {
            command.DeleteChild(command.cards[0].GetComponent<Card>());
        }
        client.EndTurn(message);
    }

    public void newCards(object sender, NetworkManagerClient.NewCardsEventArgs cardsEvents)
    {
        if (cardTypeManager == null)
            return;

        foreach (var card in cardsEvents.cards)
        {
            GameObject gameObject = Instantiate(cardTypeManager.prefabs[(CardType)card]);
            hand.AddChild(gameObject.GetComponent<Card>());
        }
    }
}
