using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vikingAnimTest : MonoBehaviour
{
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
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Physics.gravity = new Vector3(0f, gravity, 0f);
        movement = Vector3.zero;
    }

    void Update()
    {
        if (isGrounded())
        {
            //if (animator.GetBool("Jumping"))
            //{
            //    animator.SetBool("Jumping", false);
            //}
            movement.x = Input.GetAxis("Horizontal");
            movement.z = Input.GetAxis("Vertical");
        }
        if (movement != Vector3.zero)
        {
            animator.SetBool("Moving", true);
            //sprinting check
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("Sprinting", true);
                movement = movement * 1.7f; // hardcoded sprint movement multiplier TODO: allow this to be set as a public variable
            }
            else
            {
                animator.SetBool("Sprinting", false);
            }
            //facing the character to the correst direction
            transform.forward = movement;
        }
        else
        {
            animator.SetBool("Moving", false);
        }
    }

    void FixedUpdate()
    {
        //move
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);

        //grounded movements
        if (isGrounded()) // need ground checker here
        {
            //jump
            if (Input.GetButtonDown("Jump"))
            {
                //rb.velocity = Vector3.up * jumpSpeed;            
                rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                animator.SetBool("Jumping", true);
            }
            else
            {
                animator.SetBool("Jumping", false);
            }
        }

        //lightAttackGround
        if (Input.GetKeyDown(KeyCode.L) && isGrounded())
        {
            animator.SetBool("L", true);
        }

        //use fallMultiplier and lowJumpMultiplier to make character fall faster and implementing short and long jump
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) // if player is not holding the jump button, do short jump
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private bool isGrounded()
    {
        return Physics.CheckCapsule(charCollider.bounds.center, new Vector3(charCollider.bounds.center.x, charCollider.bounds.min.y, charCollider.bounds.center.z), charCollider.radius, groundLayer);
    }
}