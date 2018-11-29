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

    public GameObject landingFx;
    public LayerMask groundLayer;

    private Animator animator;
    private AnimationClip currentClip;
    private Rigidbody rb;
    private CapsuleCollider pushbox;
    private AudioSource _audioSource;

    private WeaponManager _weaponManager = null; // currently equipped weapon; null if false

    private float startingHealth = 100f; //set 100 as default
    private float startingStamina = 100f;
    private float currentHealth;
    private float currentStamina;
    private bool isDead;
    private bool isGrounded = true;
    private float inputTimer;
    private bool isHitStunned = false;
    private bool isInvincible = false;
    private bool isGrabbingThrowable = false;
    private bool canShoot = false;

    private int _staminaCooldown = 60;
    
    // get/set methods
    public float StartingHealth
    {
        get { return startingHealth; }
        set { startingHealth = value; }
    }
    public float StartingStamina
    {
        get { return startingStamina; }
        set { startingStamina = value; }
    }
    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }
    public float CurrentStamina
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
        get { return _weaponManager!=null ? true : false; }
    }
    public bool IsHoldingMeleeWeapon
    {
        get{ return _weaponManager != null
            && _weaponManager._weapon.weaponType.Equals(EquippableWeapon.WeaponType.TwoHandMelee)
            ? true : false;
        }
    }
    public bool IsHoldingRangedWeapon
    {
        get { return _weaponManager != null 
            && _weaponManager._weapon.weaponType.Equals(EquippableWeapon.WeaponType.BigGun)
            ? true : false;
        }
    }
    public WeaponManager Weapon
    {
        get { return _weaponManager; }
        set { _weaponManager = value; }
    }
    public bool CanShoot
    {
        get { return canShoot; }
        set { canShoot = value; }
    }
    public AnimationClip CurrentClip
    {
        get { return currentClip; }
    }

    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        pushbox = GetComponent<CapsuleCollider>();
        _audioSource = GetComponent<AudioSource>();
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

        // set melee animation flag
        if (IsHoldingWeapon)
        {
            switch (_weaponManager._weapon.weaponType)
            {
                case EquippableWeapon.WeaponType.TwoHandMelee:
                    animator.SetInteger("Weapon", 1); // holding weapon
                    break;
        }
            
        } else
        {
            animator.SetInteger("Weapon", 0); // not holding weapon
        }

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
    }

    void FixedUpdate()
    {
        // check if character is grounded on regular interval
        bool landed = GroundedCheck();
        if (!isGrounded && landed)
        {
            // play landing visual effect
            PlayLandingFx(2.0f);
        }
        isGrounded = landed;

        // check if holding weapon
        if (IsHoldingWeapon && Weapon.remainingCharges <= 0)
        { Weapon = null; }

        // refill stamina
        _staminaCooldown -= 1;
        if (_staminaCooldown <= 0)
        {
            FillStamina(1f);
        }

        if (!animator.GetBool("Recovery") && !animator.GetBool("Dodging"))
        {
            isInvincible = false;
        }
    }

    public void TriggerKnockup(float kbf)
    {
        animator.SetTrigger("Knockup");
        rb.AddForce(new Vector3(0,1,0) * kbf, ForceMode.VelocityChange);
    }

    public void TriggerKnockdown()
    {
        animator.SetBool("Knockdown", true);
    }

    public void TriggerKnockback(Transform attackFrom, float knockBackForce)
    {
        Vector3 direction = new Vector3(transform.position.x - attackFrom.position.x, 0f, transform.position.z - attackFrom.position.z).normalized;
        rb.AddForce(direction * knockBackForce, ForceMode.VelocityChange);
    }

    // taking damage from an attack
    public void TakeDamage(Attack attack, Transform attackFrom)
    {
        transform.LookAt(new Vector3(attackFrom.position.x, transform.position.y, attackFrom.position.z));

        if (isInvincible) { return; }
        // play getting hit sound
        _audioSource.Play();

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

        if (attack.knockback)
        {
            TriggerKnockback(attackFrom, attack.knockbackVelocity);
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

    // consumes stamina
    public void UseStamina(float stm)
    {
        currentStamina -= stm;
        _staminaCooldown = 60;

        if (currentStamina < 0)
            currentStamina = 0;
            
    }

    public void FillStamina(float stm)
    {
        currentStamina += stm;
        if (currentStamina > startingStamina)
            currentStamina = startingStamina;
    }

    public bool GroundedCheck()
    {
        return Physics.CheckCapsule(pushbox.bounds.center, new Vector3(pushbox.bounds.center.x, pushbox.bounds.min.y, pushbox.bounds.center.z), pushbox.radius, groundLayer);
    }

    private void PlayLandingFx(float timeToDestroy)
    {
        GameObject landingFxInstance = (GameObject) Instantiate(landingFx, gameObject.transform.position, Quaternion.Euler(-90f, 0f, 0f));
        Destroy(landingFxInstance, timeToDestroy);
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

    public void OnDeath()
    {
        isDead = true;

        // Turn off character input
        GetComponent<CharacterControl>().enabled = false;
        animator.SetBool("Dead", true);
    }
}
