using UnityEngine;
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
        cardTypeManager = GetComponent<CardTypeManager>();

        DontDestroyOnLoad(gameObject);
    }

    public void EndTurnAndSendMessage()
    {
        if (!hand || !command)
            return;

        var message = new NetworkManagerClient.PlayerHandMessage()
        {
            hand = hand.cards.Select<GameObject, int>(go => (int)go.GetComponent<Card>().type).ToArray(),
            command = command.cards.Select<GameObject, int>(go => (int)go.GetComponent<Card>().type).ToArray(),
            connectionId = client.connectionId
        };
        while (command.cards.Count > 0)
        {
            command.DeleteChild(command.cards[0].GetComponent<Card>());
        }
        client.EndTurn(message);
    }

    public void newCards(int[] cards)
    {
        if (cardTypeManager == null)
            return;

        var i = 0;
        while (hand.cards.Count < hand.maxCards && i < cards.Length)
        {
            hand.CreateChild(Instantiate(cardTypeManager.prefabs[(CardType)cards[i]]));
            i++;
        }

    }

    public void changeParent(Card card)
    {
        var target = card.parent == hand ? command : hand;
        if (target.hasSpace())
        {
            card.parent.RemoveChild(card);
            target.AddChild(card);

        }
    }
}
