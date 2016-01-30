using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManagerClient : NetworkManager
{
    public delegate void NewCardsEventHandler(object sender, NewCardsEventArgs e);
    public event NewCardsEventHandler OnNewCardsEvent;

    public void JoinGame()
    {
        SetIPAddress();
        SetPort();
        StartClient(); //Unity method
    }

    void SetIPAddress()
    {
        string ipAddress = GameObject.Find("InputField").transform.FindChild("Text").GetComponent<Text>().text;
        networkAddress = ipAddress;
    }

    void SetPort()
    {
        networkPort = 7777;
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        SceneManager.LoadScene("online");
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        SceneManager.LoadScene("offline");
    }

    public void EndTurn(PlayerHandMessage message)
    {
        if (client == null)
            return;

        client.Send(1337, message);
        OnMesssage();
    }

    public void OnMesssage()
    {
        int[] cards = new int[5];
        for (int i = 0; i < 3; i++)
            cards[i] = UnityEngine.Random.Range(0, 9);
        for (int i = 3; i < 5; i++)
            cards[i] = UnityEngine.Random.Range(10, 12);

        var cardEvent = new NewCardsEventArgs() { cards = cards };
        OnNewCards(cardEvent);
    }

    protected virtual void OnNewCards(NewCardsEventArgs e)
    {
        if (OnNewCardsEvent != null)
            OnNewCardsEvent(this, e);
    }

    public class NewCardsEventArgs : EventArgs
    {
        public int[] cards;
    }

    public class PlayerHandMessage : MessageBase
    {
        public int[] hand;
        public int[] command;
        public int connectionId;
    }


}
