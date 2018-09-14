using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAction : MonoBehaviour {
    //public variable
    public float speed = 10f;
    public float jumpSpeed = 10f;
    public float gravity = -20f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public LayerMask groundLayer;
    public CapsuleCollider charCollider;

    //components
    private Rigidbody rb;
    private Animator animator;

    private Vector3 movement = Vector3.zero;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Physics.gravity = new Vector3(0f, gravity, 0f);
        movement = Vector3.zero;
    }
	
    void Update()
    {
        if (isGrounded())
        {
            movement.x = Input.GetAxis("Horizontal");
            movement.z = Input.GetAxis("Vertical");
        }
        if (movement != Vector3.zero)
        {
            //facing the character to the correst direction
            transform.forward = movement;
            animator.SetBool("Moving", true);
        }
        else
        {
            animator.SetBool("Moving", false);
        }
    }

	void FixedUpdate () {
        //move
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);

        //jump
        if (Input.GetButtonDown("Jump") && isGrounded()) // need ground checker here
        {
            //rb.velocity = Vector3.up * jumpSpeed;            
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }

        //use fallMultiplier and lowJumpMultiplier to make character fall faster and implementing short and long jump
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if(rb.velocity.y > 0 && !Input.GetButton("Jump")) // if player is not holding the jump button, do short jump
        {           
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private bool isGrounded()
    {
        return Physics.CheckCapsule(charCollider.bounds.center, new Vector3(charCollider.bounds.center.x, charCollider.bounds.min.y, charCollider.bounds.center.z), charCollider.radius, groundLayer);  
    }
}
