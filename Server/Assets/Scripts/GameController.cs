using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{

    public NetworkManagerServer networkManager;
    public GameObject playerPrefab;
    public static int _BoardWidth;
    public static int _BoardHeight;
    public int BoardWidth;
    public int BoardHeight;
    public static float _TileSize;
    public float TileSize;

    private List<Player> players = new List<Player>();

    private bool playerActing = false;
    private int activePlayerId = -1;


    public class Player
    {
        public int id;
        public GameObject gameEntity;
        public Animator animator;
        public List<int> commandStack = new List<int>();
        public List<int> handStack = new List<int>();
        public int positionX = 0;
        public int positionY = 0;
        public int rotation = 0;

        public Player(int id, GameObject gameEntity)
        {
            this.id = id;

            positionX = UnityEngine.Random.Range(0, _BoardWidth);
            positionY = UnityEngine.Random.Range(0, _BoardHeight);

            this.gameEntity = gameEntity;
            this.gameEntity.transform.position = GetLocationFromBoardTile(positionX, positionY);

            rotation = UnityEngine.Random.Range(0, 3);
            animator = gameEntity.GetComponent<Animator>();
            animator.SetInteger("Direction", rotation);

            generateHand();

        }

        public void generateHand()
        {
            int[] cards = new int[4];
            for (int i = 0; i < 2; i++)
                cards[i] = UnityEngine.Random.Range(0, 9);
            for (int i = 2; i < 4; i++)
                cards[i] = UnityEngine.Random.Range(10, 12);
            handStack.Clear();
            handStack.AddRange(cards);
        }

        public ActionData Move(int number)
        {
            MoveActionData actionData = new MoveActionData();
            actionData.action = CardType.move;
            actionData.moveStart = gameEntity.transform.position;
            actionData.movePosition = 0;
            actionData.moveSpeed = 50.0f / number;
            switch (rotation)
            {
                //up
                case 0:
                    positionY += number;
                    break;
                //right
                case 1:
                    positionX += number;
                    break;
                //down
                case 2:
                    positionY -= number;
                    break;
                //left
                case 3:
                    positionX -= number;
                    break;
            }
            actionData.moveEnd = GetLocationFromBoardTile(positionX, positionY);
            return actionData;
        }

        public ActionData Turn(int number)
        {
            TurnActionData actionData = new TurnActionData();
            actionData.action = CardType.turn;
            actionData.turnStart = rotation;
            actionData.turnEnd = rotation + number;
            actionData.turnRotation = 0;
            actionData.turnSpeed = 50.0f / number;
            return actionData;
        }

        public ActionData actionData;

        public class ActionData
        {
            public CardType action;
        }

        public class MoveActionData : ActionData
        {
            public Vector3 moveStart;
            public Vector3 moveEnd;
            public float movePosition;
            public float moveSpeed;
        }

        public class TurnActionData : ActionData
        {
            public int turnStart;
            public int turnEnd;
            public float turnRotation;
            public float turnSpeed;
        }

        internal void createAction(CardType activeAction, int activeNumber)
        {
            switch (activeAction)
            {
                case CardType.move:
                    actionData = Move(activeNumber);
                    break;
                case CardType.turn:
                    actionData = Turn(activeNumber);
                    break;
            }
        }

        internal bool doAction()
        {

            switch (actionData.action)
            {
                case CardType.move:
                    var moveData = (MoveActionData)actionData;
                    gameEntity.transform.position = Vector3.Lerp(
                        moveData.moveStart,
                        moveData.moveEnd,
                        moveData.movePosition);
                    moveData.movePosition += moveData.moveSpeed / (moveData.moveStart - moveData.moveEnd).magnitude * Time.fixedDeltaTime;

                    if (moveData.movePosition > 1.0f)
                    {
                        gameEntity.transform.position = moveData.moveEnd;
                        return true;
                    }
                    break;
                case CardType.turn:
                    var turnData = (TurnActionData)actionData;
                    rotation = (int)Math.Floor(Mathf.Lerp(
                        turnData.turnStart,
                        turnData.turnEnd,
                        turnData.turnRotation)) % 4;
                    turnData.turnRotation += turnData.turnSpeed / (turnData.turnEnd - turnData.turnStart) * Time.fixedDeltaTime;

                    animator.SetInteger("Direction", rotation);

                    if (turnData.turnRotation > 1.0f)
                    {
                        rotation = turnData.turnEnd % 4;
                        animator.SetInteger("Direction", rotation);
                        return true;
                    }
                    break;
            }
            return false;
        }
    }

    public void PlayerConnected(int id)
    {
        Player player = new Player(id, Instantiate(playerPrefab));
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
                if (i == activePlayerId)
                {
                    activePlayerId--;
                }

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


    private static Vector3 GetLocationFromBoardTile(int x, int y)
    {
        return new Vector3(x * _TileSize + _TileSize / 2, y * _TileSize + _TileSize / 2, -9);
    }

    void Start()
    {
        if (networkManager == null)
            networkManager = FindObjectOfType<NetworkManagerServer>();

        networkManager.onClientConnected += new NetworkManagerServer.ClientConnectEventHandler(PlayerConnected);
        networkManager.onClientDisconnected += new NetworkManagerServer.ClientDisconnectEventHandler(PlayerDisconnected);
        networkManager.onClientData += new NetworkManagerServer.CliendDataEventHandler(PlayerGetCommand);

        _TileSize = TileSize;
        _BoardHeight = BoardHeight;
        _BoardWidth = BoardWidth;
    }

    bool DoPlayerAction()
    {
        return !players[activePlayerId].doAction();
    }

    void FixedUpdate()
    {
        if (playerActing)
        {
            playerActing = DoPlayerAction();
        }
        else if (players.Count > 0)
        {
            // select player
            {
                if (activePlayerId < 0)
                    activePlayerId = 0; // first player
                else if (players[activePlayerId].commandStack.Count == 0) // player has end his turn
                    activePlayerId = (activePlayerId + 1) % players.Count; // move to next player
            }

            // select card
            Player player = players[activePlayerId];
            if (player.commandStack.Count > 0)
            {
                var activeAction = (CardType)player.commandStack[0];
                player.commandStack.RemoveAt(0);
                var activeNumber = player.commandStack[0];
                player.commandStack.RemoveAt(0);

                player.createAction(activeAction, activeNumber);
                playerActing = true;
            }
        }
    }
}
