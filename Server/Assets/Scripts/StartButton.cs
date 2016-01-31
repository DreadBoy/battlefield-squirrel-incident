using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

    public void StartServer()
    {
        FindObjectOfType<NetworkManagerServer>().StartupServer();
    }
}
