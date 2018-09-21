using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script to allow gameObjects with Player tag to pick up and throw object

public class ThrowObject : MonoBehaviour {

    //components
    public float holdDistanceY = 7.0f;
    public int throwForce = 200;
    private Rigidbody rb;
    private Collider col;
    private Vector3 throwDirection;
    private GameObject parentPlayer; //the player the object is parented to
    private Animator animator;

    // Use this for initialization
    void Awake () {
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<Collider>();
        throwDirection = Vector3.zero;
        parentPlayer = null;
        animator = null;
	}

    // detect player collision and allow pickup
    void OnCollisionStay(Collision collision)
    {
        if (Input.GetButtonDown("Throw") && collision.gameObject.tag == "Player")
        {
            parentPlayer = collision.gameObject;
            animator = parentPlayer.GetComponent<Animator>();
            animator.SetBool("GrabbingThrowable", true);

            // make gameobject child of player (so it moves with player)
            gameObject.transform.parent = parentPlayer.transform;

            rb.isKinematic = true; //disable unity physics
            col.enabled = false;   //disable collider
            
            // move gameobject above player
            // note: the engine only MovesPos of barrel when player is standing relatively still
            rb.MovePosition(parentPlayer.transform.position + (new Vector3(0, holdDistanceY)));
        }
    }

    // throw object
    private void FixedUpdate()
    {
        if (Input.GetButtonDown("Throw") && gameObject.transform.parent != null)
        {
            // release object if it is parented to player
            if (gameObject.transform.parent == parentPlayer.transform)
            {
                rb.isKinematic = false; // enable unity physics
                col.enabled = true;     // enable collider
                throwDirection.x = Input.GetAxis("Horizontal");
                throwDirection.z = Input.GetAxis("Vertical");
                gameObject.transform.parent = null;
                animator.SetBool("GrabbingThrowable", false);
                rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }
        }
    }
}
