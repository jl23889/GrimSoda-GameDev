using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reads player input and translates input into actions
public class CharacterControl : MonoBehaviour
{
    public enum Player { P1, P2, P3, P4 };
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
    private bool _lightAttackPress;   
    private bool _heavyAttackPress;
    private bool _heavyAttackHold;
    private bool _heavyAttackUp;


    private Vector3 _movement = Vector3.zero;
    private CharacterManager _charManager;
    private AutoLock _autoLock;
    private Character _char;
    private ThrowObject _throwObj;
    public GameObject _cameraRig;

    // get/set methods
    public string PlayerString
    {
        get { return player; }
    }

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        _charManager = GetComponent<CharacterManager>();
        _autoLock = GetComponent<AutoLock>();
        _char = _charManager._character;
        _throwObj = GetComponent<ThrowObject>();
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
        //blocking
        if (Input.GetButton(player + "Block"))
        {
            if (!animator.GetBool("Jumping") && !animator.GetBool("Dodging") && !animator.GetBool("Sprinting"))
            {
                animator.SetBool("Blocking", true);
            }
        }
        else
        {
            animator.SetBool("Blocking", false);
        }

        TakeInput();

        Attack();

        // _movement animations and speed controls (might move speed control to fix update later)
        if (_movement != Vector3.zero)
        {
            animator.SetBool("Moving", true);

            //hold dodge key to sprint, press dodge key to dodge
            if (!animator.GetBool("Jumping") && !animator.GetBool("Attacking") && !animator.GetBool("Dodging"))
            {
                //sprinting
                if (Input.GetButton(player + "Dodge"))
                {
                    dodgeButtonTime = dodgeButtonTime + Time.deltaTime;
                    if (dodgeButtonTime > 0.2f)
                    {
                        animator.SetBool("Sprinting", true);
                    }
                }

                //dodging
                if (Input.GetButtonUp(player + "Dodge") && dodgeButtonTime <= 0.2f)
                {
                    animator.SetBool("Dodging", true);
                    _charManager.IsInvincible = true;
                    dodgeButtonTime = 0f;   //reset dodgeButtonTime
                    _movement = _movement * _char.dodgeMultiplier;
                }

                //end sprinting
                else if (Input.GetButtonUp(player + "Dodge"))
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
        // Movement states
        CalculateMovement();
    }

    void FixedUpdate()
    {
        //ground checker
        _isGrounded = _charManager.IsGrounded;

        if (_heavyAttackPress || _heavyAttackHold || _heavyAttackUp)
        {
            Targeting(18f, 60f);//hard code for now, replace with Attack._range           
        }
        else if (_lightAttackPress)
        {
            Targeting(8f, 120f);
        }

        // check if holding weapon
        if (_charManager.IsHoldingWeapon && _charManager.RangedWeapon.remainingAmmo <= 0)
        { _charManager.RangedWeapon = null; }
        // shoot weapon if holding weapon and not shooting
        if (_charManager.IsHoldingWeapon && _charManager.CanShoot)
        {
            if (_lightAttackPress){ _charManager.RangedWeapon.ShootLight(transform.forward); } 
            else if (_heavyAttackHold) { _charManager.RangedWeapon.ShootHeavy(transform.forward); }
        }


        //move rigidbody 
        Move();

        //_movements when grounded and not animation locked
        Jump();

        ResetInput();
    }

    private void TakeInput()
    {
        //jumping
        if (Input.GetButton(player + "Jump"))
        {
            _jumpKeyHold = true;
        }
        if (Input.GetButtonDown(player + "Jump"))
        {
            _jumpKeyPress = true;
        }

        //attacking
        if (Input.GetButtonDown(player + "LightAttack"))
        {
            _lightAttackPress = true;
        }
        if (Input.GetButtonDown(player + "HeavyAttack"))
        {
            _heavyAttackPress = true;
        }
        if (Input.GetButton(player + "HeavyAttack"))
        {
            _heavyAttackHold = true;
        }
        if (Input.GetButtonUp(player + "HeavyAttack"))
        {
            _heavyAttackUp = true;
        }
    }

    private void ResetInput()
    {
        _jumpKeyPress = false;
        _jumpKeyHold = false;
        _lightAttackPress = false;
        _heavyAttackPress = false;
        _heavyAttackHold = false;
        _heavyAttackUp = false;
    }

    // this calculates the movement based on the animations and current state of the character
    private void CalculateMovement()
    {
        if (animator.GetBool("Dodging"))
        {
            return;
        }

        if (_charManager.IsHitStunned || animator.GetBool("ChargingAttack") || animator.GetBool("Blocking"))
        {
            _movement.x = 0;
            _movement.z = 0;
            return;
        }

        // normalized input ensures that magnitude of movement will always be the same
        _movement = new Vector3(Input.GetAxis(player + "Horizontal"), 0, Input.GetAxis(player + "Vertical")).normalized;
        // rotating input to match camera's perspective
        if (_cameraRig != null)
        {
            var movX = _movement.x;
            var movZ = _movement.z;
            _movement.x = movX * Mathf.Cos(-_cameraRig.transform.eulerAngles.y * Mathf.Deg2Rad)
                - movZ * Mathf.Sin(-_cameraRig.transform.eulerAngles.y * Mathf.Deg2Rad);
            _movement.z = movX * Mathf.Sin(-_cameraRig.transform.eulerAngles.y * Mathf.Deg2Rad)
                + movZ * Mathf.Cos(-_cameraRig.transform.eulerAngles.y * Mathf.Deg2Rad);
        }

        _charManager.IsInvincible = false;

        if (animator.GetBool("Sprinting"))
        {
            _movement = _movement * _char.sprintMultiplier;
        }
        else if (animator.GetBool("Attacking") && animator.GetBool("Grounded"))
        {
            _movement = _movement * .3f;
        }

    }

    private void Move()
    {
        rb.MovePosition(transform.position + _movement * _char.runSpeed * Time.fixedDeltaTime);

        // faces target if attacking and has a target 
        if (animator.GetBool("Attacking") && _autoLock.Target != null)
        {
            transform.LookAt(new Vector3(_autoLock.Target.transform.position.x, transform.position.y, _autoLock.Target.transform.position.z));
        }
        // otherwise move in the direction of input
        else if (_movement != Vector3.zero)
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
                // using the physics formula 
                //  mgh = (1/2)mv^2  => v = sqrt(2gh)
                // this will always make the player jump the same height
                rb.AddForce(Vector3.up * Mathf.Sqrt(2 * -Physics.gravity.y * _char.jumpHeight), ForceMode.VelocityChange);
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

    private void Attack()
    {
        if (!_charManager.IsHoldingWeapon) {
            if (_heavyAttackPress)
            {
                animator.SetBool("HeavyAttack", true);
            }

            if (_heavyAttackHold && _isGrounded && !animator.GetBool("Sprinting"))
            {
                animator.SetBool("ChargingAttack", true);
            }
             
            if (_heavyAttackUp)
            {
                animator.SetBool("ChargingAttack", false);
            }

            if (_lightAttackPress)
            {
                animator.SetBool("LightAttack", true);
            }
        }
    }

    public void Targeting(float _attackRange, float _autolockMaxAngle)
    {
        _autoLock.AttackRange = _attackRange;
        _autoLock.AutolockMaxAngle = _autolockMaxAngle;
    }
}