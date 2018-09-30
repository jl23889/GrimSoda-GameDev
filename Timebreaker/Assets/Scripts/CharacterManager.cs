using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {

    public Character _character;
    public CharacterHitbox _hitbox;
    public GameObject leftArm;  // this should be the left arm bone
    public GameObject rightArm; // this should be the right arm bone
    public GameObject head;     // this should be the head bone
    private Animator animator;
    private AnimationClip currentClip;

    private int startingHealth = 100; //set 100 as default
    private int startingStamina = 100;
    private int currentHealth;
    private int currentStamina;
    private bool isDead;
    private bool isHit = false;

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

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        // initialize character health and stamina
        startingHealth = _character.healthTotal;
        startingStamina = _character.staminaTotal;
        currentHealth = _character.healthTotal;
        currentStamina = _character.staminaTotal;
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
        
        //Debug.Log("Attack Name: " + attack.attackName + "  Damage:" + attack.damage);
        //Debug.Log("Knockdown: " + attack.knockdown + " Knockback: " + attack.knockback);


        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (CurrentHealth <= 0 && !isDead)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        isDead = true;

        // Turn the character off.
        //gameObject.SetActive(false);
        animator.SetBool("Dead", true);
    }
}
