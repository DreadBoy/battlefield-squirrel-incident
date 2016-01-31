using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManagerClient : NetworkManager
{
    public delegate void NewCardsEventHandler(object sender, NewCardsEventArgs e);
    public event NewCardsEventHandler OnNewCardsEvent;

    public int connectionId;

    public Canvas offline;
    public Canvas online;

    public void JoinGame()
    {
        SetIPAddress();
        SetPort();
        StartClient(); //Unity method
        client.RegisterHandler(1338, OnServerGetHand);
        client.RegisterHandler(1339, OnServerGetConnectionId);
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
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        goOffline();
        FindObjectOfType<CardManager>().hand.cards.Clear();
        FindObjectOfType<CardManager>().command.cards.Clear();
    }

    public void EndTurn(PlayerHandMessage message)
    {
        if (client == null)
            return;

        client.Send(1337, message);
    }

    public void OnServerGetHand(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<PlayerHandMessage>();
        FindObjectOfType<CardManager>().newCards(msg.hand);
    }

    public void OnServerGetConnectionId(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ConnectionIdMessage>();
        connectionId = msg.connectionId;
        goOnline();
        FindObjectOfType<CardManager>().hand.cards.Clear();
        FindObjectOfType<CardManager>().command.cards.Clear();
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

    public class ConnectionIdMessage : MessageBase
    {
        public int connectionId;
    }

    void goOnline()
    {
        if (offline == null || online == null)
            return;
        offline.gameObject.SetActive(false);
        online.gameObject.SetActive(true);
    }

    void goOffline()
    {
        if (offline == null || online == null)
            return;
        offline.gameObject.SetActive(true);
        online.gameObject.SetActive(false);

    }


}
