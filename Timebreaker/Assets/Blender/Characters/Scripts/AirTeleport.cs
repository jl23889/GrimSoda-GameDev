using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// allow dodging mid air (visually represented as a teleport)

public class AirTeleport: MonoBehaviour {

    private Animator animator;
    private Vector3 movement = Vector3.zero;
    private Rigidbody rb;
    private Cloth[] cloths;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cloths = GetComponentsInChildren<Cloth>();
    }
    
	void Update () {
        // check if character is jumping
        if(animator.GetBool("Jumping") && !animator.GetBool("Grounded") && Input.GetButtonDown("Dodge"))
        {
            animator.SetBool("Dodging", true);
            movement.x = Input.GetAxis("Horizontal");
            movement.z = Input.GetAxis("Vertical");
            rb.isKinematic = true; // this is required to give the floating effect work
            // Disable cloths
            foreach(Cloth cloth in cloths)
            {
                cloth.enabled = false;
            }
            
            // Hang in the air based on timer
            StartCoroutine(AirMove());
        }
    }

    IEnumerator AirMove()
    {
        yield return new WaitForSecondsRealtime(.5f);
        // Enable cloths
        foreach (Cloth cloth in cloths)
        {
            cloth.enabled = true;
        }
        rb.isKinematic = false; //disable physics
    }
}
