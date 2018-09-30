using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  this is the top level class that manages each character
public class CharacterManager : MonoBehaviour {

    public Character _character;
    public CharacterHitbox _hitbox;
    public GameObject leftArm;  // this should be the left arm bone
    public GameObject rightArm; // this should be the right arm bone
    public GameObject head;     // this should be the head bone
    public LayerMask groundLayer;

    private Animator animator;
    private AnimationClip currentClip;
    private Rigidbody rb;
    private CapsuleCollider pushbox;
    private Quaternion rbInitial;

    private int startingHealth = 100; //set 100 as default
    private int startingStamina = 100;
    private int currentHealth;
    private int currentStamina;
    private bool isDead;
    private bool isHit = false;
    private bool isGrounded = true;
    
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
    public bool IsHit {
        get { return isHit; }
        set { isHit = value; }
    }
    public bool IsGrounded
    {
        get { return isGrounded; }
        set { isGrounded = value; }
    }

    // Use this for initialization
    void Start () {
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
        if (isGrounded)
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
        isGrounded = GroundedCheck();
    }

    public void TriggerKnockup(float kbf)
    {
        animator.SetTrigger("Knockup");
        rb.AddForce(new Vector3(0,1,0) * kbf, ForceMode.Impulse);
    }

    public void TriggerKnockdown()
    {
        animator.SetTrigger("Knockdown");
    }

    public void TakeDamage(Attack attack)
    {
        // Reduce current health by the amount of damage done.
        currentHealth -= attack.damage;
        animator.SetTrigger("GetHit");
        isHit = true;
        
        if (attack.knockdown)
        {
            TriggerKnockdown();
        }
        else if (attack.knockup)
        {
            TriggerKnockup(attack.knockupForce);
        }
        
        //Debug.Log("Attack Name: " + attack.attackName + "  Damage:" + attack.damage);
        //Debug.Log("Knockdown: " + attack.knockdown + " Knockback: " + attack.knockback);


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

    private void OnDeath()
    {
        isDead = true;

        // Turn the character off.
        //gameObject.SetActive(false);
        animator.SetBool("Dead", true);
    }
}
