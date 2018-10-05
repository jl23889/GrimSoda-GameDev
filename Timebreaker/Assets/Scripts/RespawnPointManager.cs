using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPointManager : MonoBehaviour
{

    [SerializeField]
    private GameObject currentGob;  // this represents the gameObject currently occupying the respawn point;

    // Use this for initialization
    void FixedUpdate()
    {
        // update currentGob based on if respawnPoint has a child attached
        if (transform.childCount == 0)
        {
            currentGob = null;
        } else if (transform.GetChild(0) != null)
        {
            currentGob = transform.GetChild(0).gameObject;
        }
    }

    // return if respawn point has a currentGob
    public bool HasGob()
    {
        return currentGob != null;
    }
}
