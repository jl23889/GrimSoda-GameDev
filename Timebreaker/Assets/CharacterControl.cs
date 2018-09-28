using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    //public variable
    public float speed = 8f;
    public float jumpSpeed = 65f;
    public float gravity = -20f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public float _sprintMultiplier = 2f;
    public float _dodgeMultiplier = 2f;

    public LayerMask groundLayer;
    public CapsuleCollider charCollider;

    //components
    private Rigidbody rb;
    private Animator animator;
   
    //used to figure out how long button is held down;
    private float dodgeButtonTime;

    private bool _isGrounded;
    private bool _jumpKeyPress;
    private bool _jumpKeyHold;
    private Vector3 _movement = Vector3.zero;


    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Physics.gravity = new Vector3(0f, gravity, 0f);
        _movement = Vector3.zero;
        dodgeButtonTime = 0f;
    }

    //buttons should be prioritized from top to bottom
    void Update()
    {
        _jumpKeyHold = Input.GetButton("Jump");
        _jumpKeyPress = Input.GetButtonDown("Jump");

        if (animator.GetBool("Blocking") || animator.GetBool("ChargingAttack"))
        {
            _movement = Vector3.zero;
        }
        else
        {
            _movement.x = Input.GetAxis("Horizontal");
            _movement.z = Input.GetAxis("Vertical");
        }

        //blocking
        if (Input.GetButton("Block"))
        {
            animator.SetBool("Blocking", true);
        }
        else
        {
            animator.SetBool("Blocking", false);
        }

        // check attack animations
        if (Input.GetButtonDown("HeavyAttack"))
        {
            animator.SetBool("HeavyAttack", true);
        }
        else if (Input.GetButton("HeavyAttack") && _isGrounded)
        {
            animator.SetBool("ChargingAttack", true);
        }
        else if (Input.GetButtonUp("HeavyAttack"))
        {
            animator.SetBool("ChargingAttack", false);
        }
        else if (Input.GetButtonDown("LightAttack"))
        {
            animator.SetBool("LightAttack", true);
        }
        else if (_isGrounded)
        {
            animator.SetBool("Grounded", true);
        }
        else
        {
            animator.SetBool("Grounded", false);
        }

        // _movement animations and speed controls (might move speed control to fix update later)
        if (_movement != Vector3.zero)
        {
            animator.SetBool("Moving", true);

            //hold dodge key to sprint, press dodge key to dodge
            if (!animator.GetBool("Jumping") && !animator.GetBool("Attacking"))
            {
                //sprinting
                if (Input.GetButton("Dodge"))
                {
                    dodgeButtonTime = dodgeButtonTime + Time.deltaTime; 
                    if (dodgeButtonTime > 0.2f)
                    {
                        _movement = _movement * _sprintMultiplier;        //increase _movement speed by _sprintMultiplier
                        animator.SetBool("Sprinting", true);
                    }
                }

                //dodging
                if (Input.GetButtonUp("Dodge") && dodgeButtonTime <= 0.2f)
                {
                    _movement = _movement * _dodgeMultiplier;            //increase _movement speed by _dodgeMultiplier
                    animator.SetBool("Dodging", true);
                    dodgeButtonTime = 0f;   //reset dodgeButtonTime
                }

                //end sprinting
                else if (Input.GetButtonUp("Dodge"))
                {
                    animator.SetBool("Sprinting", false);
                    dodgeButtonTime = 0f;   //reset dodgeButtonTime
                }
            }

        }
        else
        {
            animator.SetBool("Moving", false);
        }
    }

    void FixedUpdate()
    {
        //ground checker
        _isGrounded = isGrounded();

        //move rigidbody 
        Move();

        //_movements when grounded and not animation locked
        Jump();
    }

    private bool isGrounded()
    {
        return Physics.CheckCapsule(charCollider.bounds.center, new Vector3(charCollider.bounds.center.x, charCollider.bounds.min.y, charCollider.bounds.center.z), charCollider.radius, groundLayer);
    }

    private void Move()
    {
        rb.MovePosition(transform.position + _movement * speed * Time.fixedDeltaTime);
        
        //facing the character to the correst direction
        if (_movement != Vector3.zero)
        {
            transform.forward = _movement;
        }
    }

    private void Jump()
    {
        if (_isGrounded && !animator.GetBool("Attacking") && !animator.GetBool("Sprinting") && !animator.GetBool("Jumping"))
        {
            //jump
            if (_jumpKeyPress)
            {

                rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                animator.SetBool("Jumping", true);
            }
        }

        //use fallMultiplier and lowJumpMultiplier to make character fall faster and implementing short and long jump
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !_jumpKeyHold) // if player is not holding the jump button, do short jump
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
}