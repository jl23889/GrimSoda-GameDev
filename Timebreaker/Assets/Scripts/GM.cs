using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour {

    public static GM manager;

    public enum GameState { ONINTRO, ONTITLE, ONMAINMENU, ONARENASELECTION, ONRAIDSELECTION, ONARENA, ONRAID, UNASSIGNED };
    public GameState state;

    // do not assign, will be private soon
    public int playerCount;
    public List<GameObject> playerList;
    void Awake ()
    {
        // if manager is not set, set manager to this
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
            state = GameState.UNASSIGNED;
        } else if (manager != this)
        {
            Destroy(gameObject);
        }
    }
}
