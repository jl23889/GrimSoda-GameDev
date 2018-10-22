using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayScript : MonoBehaviour {

    private bool started = false;
	// Use this for initialization
	void Start () {
		
	}

    public void setStarted()
    {
        started = !started;
    }

    public void playGame()
    {
        // Show animation on sides
        // Make title smaller and higher
        // show real menu
    }

	// Update is called once per frame
	void Update () {
        if (Input.anyKey && !started)
        {
            // when back button is pressed, return to title page and set started = false;
            setStarted();
            playGame();
        }
	}
}
