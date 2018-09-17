using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script to check if player is in pickup range of gameobject

public class ThrowObject : MonoBehaviour {

    public GameObject player;

    //components
    public int throwForce = 200;
    private Rigidbody rb;
    private Collider col;
    private Vector3 throwDirection = Vector3.zero;

    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<Collider>();
	}
	
	void FixedUpdate () {
        if (Input.GetKeyDown(KeyCode.H))
        {
            // release object if it is parented to player
            if (gameObject.transform.parent == player.transform) 
            {
                rb.isKinematic = false; // enable unity physics
                col.enabled = true;     // enable collider
                throwDirection.x = Input.GetAxis("Horizontal");
                throwDirection.z = Input.GetAxis("Vertical");
                gameObject.transform.parent = null;
                rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }

            // pickup object(check if player is in range of gameobject)
            else if (Vector3.Distance(player.GetComponent<Rigidbody>().position, rb.position) <= 4)
            {
     
                rb.isKinematic = true; //disable unity physics
                col.enabled = false;   //disable collider
                
                // make gameobject child of player (so it moves with player)
                gameObject.transform.parent = player.transform;
                // move gameobject above player
                rb.MovePosition(player.transform.position + (new Vector3(0, 6.0f)));
            }

            
        }
	}
}
