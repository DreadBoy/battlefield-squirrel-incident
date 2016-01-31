using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NetworkManagerServer : NetworkManager
{
    public delegate void ClientConnectEventHandler(int connectionId);
    public delegate void ClientDisconnectEventHandler(int connectionId);
    public delegate void CliendDataEventHandler(PlayerHandMessage message);
    public event ClientConnectEventHandler onClientConnected;
    public event ClientDisconnectEventHandler onClientDisconnected;
    public event CliendDataEventHandler onClientData;

    private List<int> connections = new List<int>();


    void SetPort()
    {
        networkPort = 7777;
    }
    public void StartupServer()
    {
        SetPort();
        StartServer(); //Unity method
        NetworkServer.RegisterHandler(1337, OnServerReadHand);
    }


    public void ShutdownServer()
    {
        StopServer(); //Unity method
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        SceneManager.LoadScene("online");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        SceneManager.LoadScene("offline");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        if (onClientConnected != null) onClientConnected(conn.connectionId);
        NetworkServer.SendToClient(conn.connectionId, 1339, new ConnectionIdMessage()
        {
            connectionId = conn.connectionId
        });
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        if (onClientDisconnected != null) onClientDisconnected(conn.connectionId);
    }

    void OnServerReadHand(NetworkMessage netMsg)
    {
        var message = netMsg.ReadMessage<PlayerHandMessage>();
        if (onClientData != null) onClientData(message);
    }

    public void ServerSendHand(PlayerHandMessage message)
    {
        NetworkServer.SendToClient(message.connectionId, 1338, message);
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

}
