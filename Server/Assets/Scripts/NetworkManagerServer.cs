using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class NetworkManagerServer : NetworkManager
{
    private List<int> connections = new List<int>();

    public void StartupServer()
    {
        SetPort();
        StartServer(); //Unity method
        NetworkServer.RegisterHandler(1337, OnServerReadyToBeginMessage);
    }


    public void ShutdownServer()
    {
        StopServer(); //Unity method
    }


    void SetPort()
    {
        networkPort = 7777;
    }
    
    void OnServerReadyToBeginMessage(NetworkMessage netMsg)
    {
        var beginMessage = netMsg.ReadMessage<PlayerHandMessage>();
        Debug.Log("Message from connection " + beginMessage.connectionId);
    }

    public class PlayerHandMessage : MessageBase
    {
        public int[] hand;
        public int[] command;
        public int connectionId;
    }

}
