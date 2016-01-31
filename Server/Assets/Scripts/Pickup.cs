using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{

    public GameObject gameEntity;
    public int positionX = 0;
    public int positionY = 0;

    GameController controller;
    
    // Update is called once per frame
    public void Init(GameObject entity, GameController controller)
    {
        this.controller = controller;

        positionX = UnityEngine.Random.Range(-controller.BoardWidth, controller.BoardWidth);
        positionY = UnityEngine.Random.Range(-controller.BoardHeight, controller.BoardHeight);

        gameEntity = entity;
        gameEntity.transform.position = controller.GetLocationFromBoardTile(positionX, positionY);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (string.Equals(other.tag.ToLower(), "player"))
        {
            other.GetComponent<Player>().score++;
            Destroy(gameObject);
        }

    }
}
