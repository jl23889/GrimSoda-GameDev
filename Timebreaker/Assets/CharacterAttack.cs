using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{

    public Character _character;
    public BoxCollider leftArm;  
    public BoxCollider rightArm; 
    public SphereCollider head;     
    private Animator animator;
    private AnimationClip currentClip;

    [HideInInspector]
    private Attack _attack = null;

    [HideInInspector]
    public bool _gethit = false;

    // Use this for initialization
    void Start()
    {
        _attack = null;
        _gethit = false;
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {

        // get the current clip playing and see if it matches an attack animation clip in the attackList
        currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        int attackIndex = _character.attackList.FindIndex(attack => attack.animationClip == currentClip);

        // display name of current animation clip in console
        //Debug.Log("Starting clip : " + currentClip);

        if (attackIndex >= 0)
        {
            // get current attack
            _attack = _character.attackList[attackIndex];
            switch (_attack.attackLimb) //TODO: we shouldn't need to lookup the attack limb using strings, rather we should just be able to assign limbs as gameobjects in inspector
            {
                case "leftArm":
                    leftArm.enabled = true;
                    rightArm.enabled = false;
                    head.enabled = false;                    
                    break;
                case "rightArm":
                    leftArm.enabled = false;
                    rightArm.enabled = true;
                    head.enabled = false;
                    break;
                case "head":
                    leftArm.enabled = false;
                    rightArm.enabled = false;
                    head.enabled = true;
                    break;
            }           
        }
        else
        {
            leftArm.enabled = false;
            rightArm.enabled = false;
            head.enabled = false;
            _attack = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherPlayer = other.GetComponentInChildren<CharacterAttack>();

        if (otherPlayer != null)
        {           
            otherPlayer.takeDamage(_attack);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var otherPlayer = other.GetComponentInChildren<CharacterAttack>();

        if (otherPlayer != null)
        {
            otherPlayer._gethit = false;
            Debug.Log("reset _gethit");
        }
    }

    private void takeDamage(Attack _attack)
    {
        if (!_gethit)
        {
            Debug.Log(gameObject.name + " takes a hit");
            Debug.Log("Attack Name: " + _attack.attackName + "  Damage:" + _attack.damage);
            Debug.Log("Knockdown: " + _attack.knockdown + " Knockback: " + _attack.knockback);
            _gethit = true;
        }
    }
}
