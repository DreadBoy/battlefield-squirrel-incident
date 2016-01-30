using UnityEngine;
using System.Collections;

public class QuitApplication : MonoBehaviour {
    
	void Update () {
	    if(Input.GetAxis("Cancel") > 0)
        {
            Application.Quit();
        }
	}
}
