using UnityEngine;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour
{
    public int id;
    public GameObject gameEntity;
    public Animator animator;
    public List<int> commandStack = new List<int>();
    public List<int> handStack = new List<int>();
    public int positionX = 0;
    public int positionY = 0;
    public int rotation = 0;
    public Facing facing = Facing.right;

    GameController controller;

    public ActionData actionData;
    Boolean acting = false;
    internal int score;


    public void Init(int id, GameObject gameEntity, GameController controller)
    {
        this.controller = controller;

        this.id = id;

        positionX = UnityEngine.Random.Range(-controller.BoardWidth, controller.BoardWidth);
        positionY = UnityEngine.Random.Range(-controller.BoardHeight, controller.BoardHeight);

        this.gameEntity = gameEntity;
        this.gameEntity.transform.position = controller.GetLocationFromBoardTile(positionX, positionY);

        rotation = UnityEngine.Random.Range(0, 3);

        animator = GetComponentInChildren<Animator>();
        animator.SetInteger("Direction", rotation);
        SetSpriteFace();

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
        actionData.moveSpeed = 18.0f / number;
        switch (rotation)
        {
            //up
            case 0:
                if (positionY + number < controller.BoardHeight)
                    positionY += number;
                else
                    positionY = controller.BoardHeight - 1;
                break;
            //right
            case 1:
                if (positionX + number < controller.BoardWidth)
                    positionX += number;
                else
                    positionX = controller.BoardWidth - 1;
                break;
            //down
            case 2:
                if (-controller.BoardHeight <= positionY - number)
                    positionY -= number;
                else
                    positionY = -controller.BoardHeight;
                break;
            //left
            case 3:
                if (-controller.BoardWidth <= positionX - number)
                    positionX -= number;
                else
                    positionX = -controller.BoardWidth;
                break;
        }
        actionData.moveEnd = controller.GetLocationFromBoardTile(positionX, positionY);
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

    public enum Facing
    {
        right,
        left
    }

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
        acting = true;
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

                animator.SetBool("Running", true);

                if (moveData.movePosition > 1.0f)
                {
                    gameEntity.transform.position = moveData.moveEnd;
                    animator.SetBool("Running", false);
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
                SetSpriteFace();

                if (turnData.turnRotation > 1.0f)
                {
                    rotation = turnData.turnEnd % 4;
                    animator.SetInteger("Direction", rotation);
                    SetSpriteFace();
                    return true;
                }
                break;
        }
        return false;
    }

    void FixedUpdate()
    {

        if (acting)
            acting = !doAction();
        else if (commandStack.Count > 0)
        {
            var activeAction = (CardType)commandStack[0];
            commandStack.RemoveAt(0);
            var activeNumber = commandStack[0];
            commandStack.RemoveAt(0);

            createAction(activeAction, activeNumber);
        }
    }

    void SetSpriteFace()
    {

        Vector3 theScale = transform.localScale;

        if (rotation == 3 || rotation == 2)
            theScale.x = Math.Abs(theScale.x) * -1;
        else
            theScale.x = Math.Abs(theScale.x) * 1;

        transform.localScale = theScale;
    }
}

