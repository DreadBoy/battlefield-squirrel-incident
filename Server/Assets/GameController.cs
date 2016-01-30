using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    public NetworkManagerServer networkManager;
    public GameObject playerPrefab;
    public int BoardWidth;
    public int BoardHeight;
    public float TileSize;

    private List<Player> players = new List<Player>();

    private Vector3 moveStart = new Vector3();
    private Vector3 moveEnd = new Vector3();
    private bool playerMoving = false;
    private int activePlayerId = -1;
    private float moveSpeed = 0.03f;
    private float movePosition = 0;


    public class Player
    {
        public int id;
        public GameObject gameEntity;
        public List<int> handStack = new List<int>();
        public int positionX = 0;
        public int positionY = 0;
    }

    public void PlayerConnected(int id)
    {
        Player player = new Player();
        player.id = id;
        player.gameEntity = GameObject.Instantiate( playerPrefab );
        players.Add( player );

        player.positionX = Random.Range( 0, BoardWidth );
        player.positionY = Random.Range( 0, BoardHeight );

        player.gameEntity.transform.position = GetLocationFromBoardTile( player.positionX, player.positionY );
    }

    public void PlayerDisconnected(int id)
    {
        for (int i = 0; i < players.Count; ++i )
        {
            if (players[i].id == id )
            {
                if (i == activePlayerId)
                {
                    activePlayerId--;
                }

                GameObject.Destroy( players[i].gameEntity );
                players.RemoveAt( i );
                break;
            }
        }
    }

    public void PlayerDrawHand( NetworkManagerServer.PlayerHandMessage hand )
    {
        foreach (Player player in players)
        {
            if( player.id == hand.connectionId )
            {
                player.handStack.AddRange( hand.command );
            }
        }
    }


    public void MovePlayer()
    {
        players[activePlayerId].gameEntity.transform.position = Vector3.Lerp( moveStart, moveEnd, movePosition );
        movePosition += moveSpeed;

        if (movePosition > 1.0f)
        {
            players[activePlayerId].gameEntity.transform.position = moveEnd;
            playerMoving = false;
        }
    }

    private Vector3 GetLocationFromBoardTile(int x, int y)
    {
        float halfW = TileSize * BoardWidth * 0.5f;
        float halfH = TileSize * BoardHeight * 0.5f;
        return new Vector3( x * TileSize - halfW, y * TileSize * halfH, 0 );
    }

	// Use this for initialization
	void Start () {

        if( networkManager == null )
            networkManager = FindObjectOfType<NetworkManagerServer>();

        networkManager.onClientConnected += new NetworkManagerServer.ClientConnectEventHandler( PlayerConnected );
        networkManager.onClientDisconnected += new NetworkManagerServer.ClientDisconnectEventHandler( PlayerDisconnected );
        networkManager.onClientData += new NetworkManagerServer.CliendDataEventHandler( PlayerDrawHand );
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (playerMoving)
        {
            MovePlayer();
        }
        else if( players.Count > 0 )
        {
            // select player
            {
                if( activePlayerId < 0 )
                    activePlayerId = 0; // first player
                else if (players[activePlayerId].handStack.Count == 0) // player has end his turn
                    activePlayerId = (activePlayerId + 1) % players.Count; // move to next player
            }

            // select card
            Player player = players[activePlayerId];
            if (player.handStack.Count > 0)
            {
                player.handStack.RemoveAt( 0 );
                moveStart = player.gameEntity.transform.position;

                moveEnd = GetLocationFromBoardTile( player.positionX + Random.Range( -1, 2 ), player.positionY + Random.Range( -1, 2 ) );
                movePosition = 0.0f;
                playerMoving = true;
            }
        }
	}
}
