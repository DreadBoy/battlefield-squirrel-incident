using UnityEngine;
using System.Collections;

public class Connect : MonoBehaviour {
    NetworkManagerClient client;

    void Start()
    {
        client = FindObjectOfType<NetworkManagerClient>();
    }

    public void ConnectToServer()
    {
        client.JoinGame();
    }
}
