using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelFrag : MonoBehaviour {

    public GameObject barrelFrag;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Instantiate(barrelFrag, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
