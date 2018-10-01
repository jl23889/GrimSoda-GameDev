using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reads player input and translates input into actions
public class CharacterControl : MonoBehaviour
{
    public enum Player {P1, P2, P3, P4};
    public Player _player;
    private string player;

    public float gravity = -20f;
    //components
    private Rigidbody rb;
    private Animator animator;
   
    //used to figure out how long button is held down;
    private float dodgeButtonTime;

    private bool _isGrounded;
    private bool _jumpKeyPress;
    private bool _jumpKeyHold;
    private Vector3 _movement = Vector3.zero;
    private CharacterManager _charManager;
    private Character _char;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        _charManager = GetComponent<CharacterManager>();
        _char = _charManager._character;
        Physics.gravity = new Vector3(0f, gravity, 0f);
        _movement = Vector3.zero;
        dodgeButtonTime = 0f;
        switch (_player)
        {
            case Player.P1:
                player = "P1";
                break;
            case Player.P2:
                player = "P2";
                break;
            case Player.P3:
                player = "P3";
                break;
            case Player.P4:
                player = "P4";
                break;
        }
    }

    //buttons should be prioritized from top to bottom
    void Update()
    {
        // animation locks (these are resolved via scripts in the animator)
        if (animator.GetBool("Dodging")) { return; }
        if (animator.GetBool("Jumping") && animator.GetBool("Attacking")) { return; }

        _movement.x = Input.GetAxis(player + "Horizontal");
        _movement.z = Input.GetAxis(player + "Vertical");

        //blocking
        if (Input.GetButton(player+"Block"))
        {
            if (!animator.GetBool("Jumping") && !animator.GetBool("Dodging") && !animator.GetBool("Sprinting"))
            {
                animator.SetBool("Blocking", true);
                _movement = Vector3.zero;
            }
        }
        else
        {
            animator.SetBool("Blocking", false);
        }

        //jumping
        _jumpKeyHold = Input.GetButton(player + "Jump");
        _jumpKeyPress = Input.GetButtonDown(player + "Jump");

        //attacking
        if (Input.GetButtonDown(player + "HeavyAttack"))
        {
            animator.SetBool("HeavyAttack", true);
        }
        else if (Input.GetButton(player + "HeavyAttack") && _isGrounded)
        {
            animator.SetBool("ChargingAttack", true);
        }
        else if (Input.GetButtonUp(player + "HeavyAttack"))
        {
            animator.SetBool("ChargingAttack", false);
            _movement = _movement * _char.dodgeMultiplier;
        }
        else if (Input.GetButtonDown(player + "LightAttack"))
        {
            animator.SetBool("LightAttack", true);
        }

        // _movement animations and speed controls (might move speed control to fix update later)
        if (_movement != Vector3.zero)
        {
            animator.SetBool("Moving", true);

            //hold dodge key to sprint, press dodge key to dodge
            if (!animator.GetBool("Jumping") && !animator.GetBool("Attacking"))
            {
                //sprinting
                if (Input.GetButton(player + "Dodge"))
                {
                    dodgeButtonTime = dodgeButtonTime + Time.deltaTime; 
                    if (dodgeButtonTime > 0.2f)
                    {
                        _movement = _movement * _char.sprintMultiplier;        //increase _movement speed by _sprintMultiplier
                        animator.SetBool("Sprinting", true);
                    }
                }

                //dodging
                if (Input.GetButtonUp(player + "Dodge") && dodgeButtonTime <= 0.2f)
                {
                    _movement = _movement * _char.dodgeMultiplier;            //increase _movement speed by _dodgeMultiplier
                    animator.SetBool("Dodging", true);
                    dodgeButtonTime = 0f;   //reset dodgeButtonTime
                }

                //end sprinting
                else if (Input.GetButtonUp(player+"Dodge"))
                {
                    animator.SetBool("Sprinting", false);
                    dodgeButtonTime = 0f;   //reset dodgeButtonTime
                }
            }

            if (animator.GetBool("ChargingAttack"))
            {
                _movement = _movement * 0f;
            }
            else if (animator.GetBool("Attacking"))
            {
                _movement = _movement * .3f;
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
        _isGrounded = _charManager.IsGrounded;

        //move rigidbody 
        Move();

        //_movements when grounded and not animation locked
        Jump();
    }

    private void Move()
    {
        rb.MovePosition(transform.position + _movement * _char.runSpeed * Time.fixedDeltaTime);
        
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

                rb.AddForce(Vector3.up * _char.jumpSpeed, ForceMode.Impulse);
                animator.SetBool("Jumping", true);
            }
        }

        //use fallMultiplier and lowJumpMultiplier to make character fall faster and implementing short and long jump
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (_char.fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !_jumpKeyHold) // if player is not holding the jump button, do short jump
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (_char.lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
}