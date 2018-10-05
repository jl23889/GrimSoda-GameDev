using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  this is the top level class that manages each character
public class CharacterManager : MonoBehaviour {

    public Character _character;
    public GameObject leftArm;  // this should be the left arm bone
    public GameObject rightArm; // this should be the right arm bone
    public GameObject head;     // this should be the head bone
    public GameObject chest;    // chest bone
    public GameObject leftHand;     //left hand bone
    public GameObject rightHand;    //right hand bone
    public GameObject headEnd;         //head_end bone
    public LayerMask groundLayer;

    private Animator animator;
    private AnimationClip currentClip;
    private Rigidbody rb;
    private CapsuleCollider pushbox;
    private Quaternion rbInitial;

    private RangedWeaponManager _rwManager = null; // currently equipped weapon; null if false

    private int startingHealth = 100; //set 100 as default
    private int startingStamina = 100;
    private int currentHealth;
    private int currentStamina;
    private bool isDead;
    private bool isGrounded = true;
    private float inputTimer;
    private bool isHitStunned = false;
    private bool isInvincible = false;
    private bool isGrabbingThrowable = false;
    private bool canShoot = false;
    
    // get/set methods
    public int StartingHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }
    public int StartingStamina
    {
        get { return currentStamina; }
        set { currentStamina = value; }
    }
    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }
    public int CurrentStamina
    {
        get { return currentStamina; }
        set { currentStamina = value; }
    }
    public bool IsDead { get { return isDead; } }
    public bool IsGrounded
    {
        get { return isGrounded; }
        set { isGrounded = value; }
    }
    public bool IsHitStunned
    {
        get { return isHitStunned; }
        set { isHitStunned = value; }
    }
    public bool IsInvincible
    {
        get { return isInvincible; }
        set { isInvincible = value; }
    }
    public bool IsGrabbingThrowable
    {
        get { return isGrabbingThrowable; }
        set { isGrabbingThrowable = value; }
    }
    public bool IsHoldingWeapon
    {
        get { return _rwManager!=null ? true : false; }
    }
    public RangedWeaponManager RangedWeapon
    {
        get { return _rwManager; }
        set { _rwManager = value; }
    }
    public bool CanShoot
    {
        get { return canShoot; }
        set { canShoot = value; }
    }

    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody>();
        rbInitial = rb.rotation;
        animator = GetComponent<Animator>();
        pushbox = GetComponent<CapsuleCollider>();
        // initialize character health and stamina
        startingHealth = _character.healthTotal;
        startingStamina = _character.staminaTotal;
        currentHealth = _character.healthTotal;
        currentStamina = _character.staminaTotal;
    }

    private void Update()
    {
        // get the current clip playing 
        currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        // set canShoot flag
        canShoot =
            _character.canShootAnimationList.Contains(currentClip) ? true : false;

        // set grounded animation update here
        //  separate the animation update from game logic so character has no lag time
        //  between animations
        if (GroundedCheck())
        {
            animator.SetBool("Grounded", true);
        }
        else
        {
            animator.SetBool("Grounded", false);
        }

        // weapon check 
        if (_rwManager != null)
        {
            animator.SetBool("RangedWeapon", true);
        }
        else animator.SetBool("RangedWeapon", false);
    }

    void FixedUpdate()
    {
        // check if character is grounded on regular interval
        isGrounded = GroundedCheck();
    }

    public void TriggerKnockup(float kbf)
    {
        animator.SetTrigger("Knockup");
        rb.AddForce(new Vector3(0,1,0) * kbf, ForceMode.VelocityChange);
    }

    public void TriggerKnockdown()
    {
        animator.SetTrigger("Knockdown");
    }

    // taking damage from an attack
    public void TakeDamage(Attack attack)
    {
        if (isInvincible) { return; }
        // Reduce current health by the amount of damage done.
        currentHealth -= attack.damage;
        // Drop throwable
        if (isGrabbingThrowable)
        {
            isGrabbingThrowable = false;
        }

        animator.SetTrigger("GetHit");
        chest.GetComponent<ParticleSystem>().Play(false);
        StartCoroutine(DisableInput(attack.hitStunDuration));

        if (attack.knockdown)
        {
            TriggerKnockdown();
        }
        else if (attack.knockup)
        {
            TriggerKnockup(attack.knockupVelocity);
        }
        
        //Debug.Log("Attack Name: " + attack.attackName + "  Damage:" + attack.damage);
        //Debug.Log("Knockdown: " + attack.knockdown + " Knockback: " + attack.knockback);


        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (CurrentHealth <= 0 && !isDead)
        {
            OnDeath();
        }
    }

    // taking damage from a throwable fragmentation
    public void TakeDamage(Throwable throwable)
    {
        if (isInvincible) { return; }
        // Reduce current health by the amount of damage done.
        currentHealth -= throwable.damage;
        // Drop throwable
        if (isGrabbingThrowable)
        {
            isGrabbingThrowable = false;
        }

        animator.SetTrigger("GetHit");
        chest.GetComponent<ParticleSystem>().Play(false);
        StartCoroutine(DisableInput(throwable.hitStunDuration));

        if (throwable.knockdown)
        {
            TriggerKnockdown();
        }
        else if (throwable.knockup)
        {
            TriggerKnockup(throwable.knockupVelocity);
        }

        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (CurrentHealth <= 0 && !isDead)
        {
            OnDeath();
        }
    }

    public bool GroundedCheck()
    {
        return Physics.CheckCapsule(pushbox.bounds.center, new Vector3(pushbox.bounds.center.x, pushbox.bounds.min.y, pushbox.bounds.center.z), pushbox.radius, groundLayer);
    }

    // disables player input for length of hsd
    IEnumerator DisableInput(float hsd)
    {
        inputTimer = hsd;
        isHitStunned = true;
        animator.SetBool("HitStun", true);
        while (inputTimer > 0)
        {
            inputTimer -= Time.deltaTime;
            yield return null;
        }
        isHitStunned = false;
        animator.SetBool("HitStun", false);
    }

    private void OnDeath()
    {
        isDead = true;

        // Turn off character input
        GetComponent<CharacterControl>().enabled = false;
        animator.SetBool("Dead", true);
    }
}
