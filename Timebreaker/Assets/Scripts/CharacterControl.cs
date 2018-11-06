using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// reads player input and translates input into actions
public class CharacterControl : MonoBehaviour
{
    public enum Player { P1, P2, P3, P4 };
    public Player _player;
    public GameObject _hudUI;
    public  GameObject _playerUI;
    private string player;
    public GameObject walkingFx;
    public GameObject jumpingFx;
    public GameObject chargingFx;
    private GameObject chargingFxInstance;
    public GameObject releaseFx;
    private float walkingFxTimer;
    public Material material;

    public float gravity = -20f;
    //components
    private Rigidbody rb;
    private Animator animator;

    //used to figure out how long button is held down;
    private float dodgeButtonTime;
    private float heavyButtonTime;

    private bool _isGrounded;

    //input keys
    private bool _jumpKeyPress;
    private bool _jumpKeyHold;
    private bool _lightAttackPress;   
    private bool _heavyAttackPress;
    private bool _heavyAttackHold;
    private bool _heavyAttackUp;
    private bool _dodgeKeyHold;
    private bool _dodgeKeyUp;

    private Vector3 _movement = Vector3.zero;
    private CharacterManager _charManager;
    private AutoLock _autoLock;
    private Character _char;
    public GameObject _cameraRig;

    // get/set methods
    public string PlayerString
    {
        get { return player; }
    }

    public Vector3 Movement
    {
        set { _movement = value; }
    }

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        _charManager = GetComponent<CharacterManager>();
        _autoLock = GetComponent<AutoLock>();
        _char = _charManager._character;

        Physics.gravity = new Vector3(0f, gravity, 0f);
        _movement = Vector3.zero;
        dodgeButtonTime = 0f;

        switch (_player)
        {
            case Player.P1:
                player = "P1";
                if (_charManager._character.playerUI != null)
                {
                    GameObject _playerUIinstance = 
                        (GameObject) Instantiate(_playerUI, _hudUI.transform.GetChild(0).transform, false);
                    _playerUIinstance.transform.GetChild(2).GetComponent<RawImage>().color = Color.red;
                    GetComponentInChildren<Image>().color = Color.red;
                }
                break;
            case Player.P2:
                player = "P2";
                if (_charManager._character.playerUI != null)
                {
                    GameObject _playerUIinstance = 
                        (GameObject) Instantiate(_playerUI, _hudUI.transform.GetChild(1).transform, false);
                    _playerUIinstance.transform.GetChild(2).GetComponent<RawImage>().color = Color.green;
                    GetComponentInChildren<Image>().color = Color.green;
                }
                break;
            case Player.P3:
                player = "P3";
                if (_charManager._character.playerUI != null)
                {
                    GameObject _playerUIinstance = 
                        (GameObject) Instantiate(_playerUI, _hudUI.transform.GetChild(2).transform, false);
                    GetComponentInChildren<Image>().color = _playerUI.transform.GetChild(2).GetComponent<RawImage>().color;
                }
                break;
            case Player.P4:
                player = "P4";
                if (_charManager._character.playerUI != null)
                {
                    GameObject _playerUIinstance = 
                        (GameObject) Instantiate(_playerUI, _hudUI.transform.GetChild(3).transform, false);
                    _playerUIinstance.transform.GetChild(2).GetComponent<RawImage>().color = new Color(255, 176, 0, 255);
                    GetComponentInChildren<Image>().color = new Color(255, 176, 0, 255);
                }
                break;
        }
        transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = material;
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
        DodgeAndSprint();
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
        //move rigidbody 
        Move();

        //_movements when grounded and not animation locked
        Jump();

        ResetInput();
    }

    private void TakeInput()
    {
        // only allow dodge and sprint when not holding weapon
        if (!_charManager.IsHoldingMeleeWeapon)
        {
            _dodgeKeyHold = Input.GetButton(player + "Dodge");
            _dodgeKeyUp = Input.GetButtonUp(player + "Dodge");
        } else
        {
            _dodgeKeyHold = false;
            _dodgeKeyUp = false;
        }

        if (!_dodgeKeyHold && !_dodgeKeyUp)
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

            if (!_jumpKeyHold && !_jumpKeyPress)
            {
                //heavy attack
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

                if (!_heavyAttackPress && !_heavyAttackHold && !_heavyAttackUp)
                {
                    if (Input.GetButtonDown(player + "LightAttack"))
                    {
                        _lightAttackPress = true;
                    }
                }
            }
        }
        
    }

    private void DodgeAndSprint()
    {
        // _movement animations and speed controls (might move speed control to fix update later)
        if (_movement != Vector3.zero)
        {
            animator.SetBool("Moving", true);

            //hold dodge key to sprint, press dodge key to dodge
            if (!animator.GetBool("Jumping") && !animator.GetBool("Attacking") && !animator.GetBool("Dodging") && !animator.GetBool("GrabbingThrowable"))
            {
                //sprinting
                if (_dodgeKeyHold && _charManager.CurrentStamina >= 100f)
                {
                    dodgeButtonTime = dodgeButtonTime + Time.deltaTime;
                    if (dodgeButtonTime > 0.2f)
                    {
                        animator.SetBool("Sprinting", true);
                    }
                }
                // cancel sprint if stamina is too low 
                if (_charManager.CurrentStamina <= 0f)
                    animator.SetBool("Sprinting", false);

                //dodging
                if (_dodgeKeyUp && dodgeButtonTime <= 0.2f && _charManager.CurrentStamina >= 70f)
                {
                    animator.SetBool("Dodging", true);
                    StartCoroutine(DodgeInvincibility(1.4f));
                    _charManager.IsInvincible = true;
                    dodgeButtonTime = 0f;   //reset dodgeButtonTime
                    _movement = _movement * _char.dodgeMultiplier;
                    _charManager.UseStamina(70f);
                }

                //end sprinting
                else if (_dodgeKeyUp)
                {
                    animator.SetBool("Sprinting", false);
                    dodgeButtonTime = 0f;   //reset dodgeButtonTime
                }
            }
        }
        else
        {
            
            if (Input.GetButtonUp(player + "Dodge") && _charManager.CurrentStamina >= 70f 
                && !animator.GetBool("Jumping") && !animator.GetBool("Attacking") 
                && !animator.GetBool("Dodging") && !animator.GetBool("GrabbingThrowable"))
            {
                animator.SetBool("Dodging", true);
                StartCoroutine(DodgeInvincibility(1.4f));
                _charManager.IsInvincible = true;
                _movement = transform.forward * _char.dodgeMultiplier;
                _charManager.UseStamina(70f);
            }
            animator.SetBool("Moving", false);
            animator.SetBool("Sprinting", false);
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
        MovementByCamera();

        if (_charManager.IsHitStunned || animator.GetBool("ChargingAttack") || animator.GetBool("Blocking") || animator.GetBool("ShootingBigGun"))
        {
            _movement.x = 0;
            _movement.z = 0;
            return;
        }

        // recovery
        if (animator.GetBool("Knockdown") && !animator.GetBool("HitStun"))
        {
            // if character is moving from knockdown while not hitstunned
            if (_movement.x != 0 || _movement.z != 0)
            {
                _charManager.IsInvincible = true;
                animator.SetBool("Recovery", true);
                StartCoroutine(RecoveryInvincibility(1.3f));
                return;
            }
        }

        if (animator.GetBool("Dodging"))
        {
            return;
        }

        if (animator.GetBool("Sprinting"))
        {
            _movement = _movement * _char.sprintMultiplier;
            _charManager.UseStamina(1f);
        }
        else if (animator.GetBool("Attacking") && animator.GetBool("Grounded"))
        {
            _movement = _movement * .3f;
        }

    }

    private void MovementByCamera()
    {
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
    }

    private void Move()
    {
        //rb.MovePosition(transform.position + _movement * _char.runSpeed * Time.fixedDeltaTime);
        rb.AddForce(_movement * _char.runSpeed * Time.fixedDeltaTime * 20000);

        // faces target if attacking and has a target 
        if (animator.GetBool("Attacking") && _autoLock.Target != null)
        {
            transform.LookAt(new Vector3(_autoLock.Target.transform.position.x, transform.position.y, _autoLock.Target.transform.position.z));
        }
        // otherwise move in the direction of input
        else if (_movement != Vector3.zero)
        {
            walkingFxTimer+= Time.deltaTime;
            if (_isGrounded)
            {
                // running 
                if (animator.GetBool("Sprinting"))
                {
                    PlayMovingFx(1.5f);
                }
                // walking
                else if (walkingFxTimer >= .3f)
                {
                    PlayMovingFx(1.5f);
                    walkingFxTimer = 0;
                }
            }
            transform.forward = _movement;
        }
    }

    private void PlayMovingFx(float timeToDestroy)
    {
        if (walkingFx != null)
        {
            GameObject walkingFxInstance = (GameObject)Instantiate(walkingFx, gameObject.transform.position, Quaternion.Euler(-90f, 0f, 0f));
            Destroy(walkingFxInstance, timeToDestroy);
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

                // create the jump vfx
                PlayJumpingFx(1.0f);
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

    private void PlayJumpingFx(float timeToDestroy)
    {
        if (jumpingFx != null)
        {
            GameObject jumpingFxInstance = (GameObject)Instantiate(jumpingFx, gameObject.transform.position, Quaternion.Euler(-90f, 0f, 0f));
            Destroy(jumpingFxInstance, timeToDestroy);
        }
    }

    private void Attack()
    {
        if (_heavyAttackPress)
        {
            animator.SetBool("HeavyAttack", true);
            if (_charManager.IsHoldingMeleeWeapon)
            {
                return;
            }
        }

        if (_heavyAttackHold && _isGrounded && !animator.GetBool("Sprinting"))
        { 
            // instantiate charging attack vfx
            if (chargingFx != null && chargingFxInstance == null && heavyButtonTime >= 1.0f && animator.GetBool("ChargingAttack"))
            {
                chargingFxInstance = (GameObject)Instantiate(chargingFx, gameObject.transform.position + Vector3.up, Quaternion.Euler(-90f,0f,0f));
            }

            heavyButtonTime += Time.fixedDeltaTime;
            animator.SetBool("ChargingAttack", true);
        }
             
        if (_heavyAttackUp)
        {
            // destroy charging attack vfx
            if (releaseFx != null && chargingFxInstance != null)
            {
                GameObject releaseFxInstance = (GameObject)Instantiate(releaseFx, gameObject.transform.position + Vector3.up, transform.rotation);
                Destroy(chargingFxInstance);
                Destroy(releaseFxInstance, 1.0f);
                chargingFxInstance = null;
            }

            animator.SetBool("ChargingAttack", false);
            heavyButtonTime = 0f;
        }

        if (_lightAttackPress)
        {
            animator.SetBool("LightAttack", true);
        }
    }

    public void Targeting(float _attackRange, float _autolockMaxAngle)
    {
        _autoLock.AttackRange = _attackRange;
        _autoLock.AutolockMaxAngle = _autolockMaxAngle;
    }

    private IEnumerator DodgeInvincibility(float duration)
    {
        yield return new WaitForSeconds(duration);
        _charManager.IsInvincible = false;
        animator.SetBool("Dodging", false);
    }

    private IEnumerator RecoveryInvincibility(float duration)
    {
        yield return new WaitForSeconds(duration);
        _charManager.IsInvincible = false;
        animator.SetBool("Recovery", false);
    }
}