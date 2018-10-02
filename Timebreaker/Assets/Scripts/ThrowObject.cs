using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script to allow players to pick up and throw Throwable tagged objects

public class ThrowObject : MonoBehaviour {

    //components
    private float offsetY;
    private float throwVelocity;
    private Rigidbody rb;   // rigidbody of throwableObject
    private Collider col;   // collider of throwableObject
    private GameObject throwableObject;

    private CharacterManager _charManager;
    private Animator animator;
    private string playerStr; 

    // Use this for initialization
    void Start () {
        rb = null;
        col = null;
        throwableObject = null;

        _charManager = GetComponent<CharacterManager>();
        offsetY = _charManager._character.grabbingThrowableOffsetY;
        throwVelocity = _charManager._character.throwVelocity;
        animator = GetComponent<Animator>();
        playerStr = GetComponent<CharacterControl>().PlayerString;
	}

    // detect player collision and allow pickup
    void OnCollisionStay(Collision collision)
    {
        if (Input.GetButtonDown(playerStr + "Throw") && collision.gameObject.tag == "Throwable")
        {
            throwableObject = collision.gameObject;
            rb = throwableObject.GetComponent<Rigidbody>();
            col = throwableObject.GetComponent<Collider>();
            
            // make gameobject child of player (so it moves with player)
            throwableObject.transform.parent = transform;
            // set isGrabbing flag to true
            _charManager.IsGrabbingThrowable = true;

            rb.isKinematic = true; //disable unity physics
            col.enabled = false;   //disable collider
        }
    }

    void Update()
    {
        if (_charManager.IsGrabbingThrowable)
        {
            animator.SetBool("GrabbingThrowable", true);
        }
        else
        {
            animator.SetBool("GrabbingThrowable", false);
        }
    }

    void FixedUpdate()
    {
        // move throwable above player if it exists
        if (rb != null)
        {
            rb.MovePosition(transform.position + (new Vector3(0, offsetY)));
        }

        // throw object
        if (Input.GetButtonDown(playerStr + "Throw"))
        {
            ReleaseThrowable(true);
        }
        else if (!_charManager.IsGrabbingThrowable)
        {
            ReleaseThrowable();
        }
    }

    public void ReleaseThrowable(bool addForce = false)
    {
        // release object if it is parented to player
        if (throwableObject != null && throwableObject.transform.IsChildOf(transform))
        {
            rb.isKinematic = false; //enable unity physics
            col.enabled = true;   // enable collider

            // unparent the player to the throwable
            throwableObject.transform.parent = null;
            // set is grabbing flag to false
            _charManager.IsGrabbingThrowable = false;

            if (addForce)
            {
                rb.AddForce(transform.forward * throwVelocity, ForceMode.VelocityChange);
            }

            throwableObject = null;
            rb = null;
            col = null;
        }
    }
}
