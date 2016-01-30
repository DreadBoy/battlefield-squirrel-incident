using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkManagerServer : NetworkManager
{
    public void StartupHost()
    {
        SetPort();
        StartHost(); //Unity method
        client.RegisterHandler(1002, OnServerReadyToBeginMessage);
    }

    void SetPort()
    {
        networkPort = 7777;
    }

    void OnServerReadyToBeginMessage(NetworkMessage netMsg)
    {
        var beginMessage = netMsg.ReadMessage<PlayerHandMessage>();
        Debug.Log("received OnServerReadyToBeginMessage " + beginMessage.hand.Length);
    }

    public class PlayerHandMessage : MessageBase
    {
        public int[] hand;
        public int[] command;
    }

}
