using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnPress : MonoBehaviour {
    // Use this for initialization

    public UIManager hook;
	
	// Update is called once per frame
	void Update () {
        if(Input.anyKey)
        {
            hook.showMenu();
        }
	}
}
