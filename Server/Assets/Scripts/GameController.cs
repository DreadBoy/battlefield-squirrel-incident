using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{

    public NetworkManagerServer networkManager;
    public GameObject playerPrefab;
    public int BoardWidth;
    public int BoardHeight;
    public float TileSize;

    private List<Player> players = new List<Player>();
    private List<Pickup> pickups = new List<Pickup>();

    private bool playerActing = false;
    private int activePlayerId = -1;
    
    public void PlayerConnected(int id)
    {
        Player player = (Instantiate(playerPrefab) as GameObject).GetComponent<Player>();
        player.Init(id, player.gameObject, this);
        players.Add(player);
        networkManager.ServerSendHand(new NetworkManagerServer.PlayerHandMessage()
        {
            command = new int[0],
            hand = player.handStack.ToArray(),
            connectionId = id
        });
    }

    public void PlayerDisconnected(int id)
    {
        for (int i = 0; i < players.Count; ++i)
        {
            if (players[i].id == id)
            {
                GameObject.Destroy(players[i].gameEntity);
                players.RemoveAt(i);
                break;
            }
        }
    }

    public void PlayerGetCommand(NetworkManagerServer.PlayerHandMessage playerHandMessage)
    {
        foreach (Player player in players)
        {
            if (player.id == playerHandMessage.connectionId)
            {
                player.commandStack.AddRange(playerHandMessage.command);
                player.generateHand();
                networkManager.ServerSendHand(new NetworkManagerServer.PlayerHandMessage()
                {
                    command = new int[0],
                    hand = player.handStack.ToArray(),
                    connectionId = playerHandMessage.connectionId
                });
            }
        }
    }


    public Vector3 GetLocationFromBoardTile(int x, int y)
    {
        return new Vector3(x * TileSize + TileSize / 2, y * TileSize + TileSize / 2, -9);
    }

    void Start()
    {
        if (networkManager == null)
            networkManager = FindObjectOfType<NetworkManagerServer>();

        networkManager.onClientConnected += new NetworkManagerServer.ClientConnectEventHandler(PlayerConnected);
        networkManager.onClientDisconnected += new NetworkManagerServer.ClientDisconnectEventHandler(PlayerDisconnected);
        networkManager.onClientData += new NetworkManagerServer.CliendDataEventHandler(PlayerGetCommand);

        for (int i = 0; i < 8; i++)
        {
            var prefabs = Resources.LoadAll("Pickups");
            var prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length)];

            Pickup pickup = (Instantiate(prefab) as GameObject).GetComponent<Pickup>();
            pickup.Init(pickup.gameObject, this);
            pickups.Add(pickup);
        }
    }
}
