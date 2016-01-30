using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NetworkManagerServer : NetworkManager
{
    public delegate void ClientConnectEventHandler( int connectionId );
    public delegate void ClientDisconnectEventHandler( int connectionId );
    public delegate void CliendDataEventHandler( PlayerHandMessage message );
    public event ClientConnectEventHandler onClientConnected;
    public event ClientDisconnectEventHandler onClientDisconnected;
    public event CliendDataEventHandler onClientData;

    private List<int> connections = new List<int>();
    public GameController gameController;

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

    public override void OnServerConnect( NetworkConnection conn )
    {
        base.OnServerConnect( conn );
        if( onClientConnected != null ) onClientConnected( conn.connectionId );
    }

    public override void OnServerDisconnect( NetworkConnection conn )
    {
        base.OnServerDisconnect( conn );
        if( onClientDisconnected != null ) onClientDisconnected( conn.connectionId );
    }

    void SetPort()
    {
        networkPort = 7777;
    }
    
    void OnServerReadyToBeginMessage(NetworkMessage netMsg)
    {
        var beginMessage = netMsg.ReadMessage<PlayerHandMessage>();
        if( onClientData != null ) onClientData( beginMessage );
        //Debug.Log("Message from connection " + beginMessage.connectionId);
    }

    public class PlayerHandMessage : MessageBase
    {
        public int[] hand;
        public int[] command;
        public int connectionId;
    }

}
