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

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
    }
	
	void FixedUpdate () {

        // get the current clip playing and see if it matches an attack animation clip in the attackList
        currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        int attackIndex = _character.attackList.FindIndex(attack => attack.animationClip == currentClip);

        // display name of current animation clip in console
        //Debug.Log("Starting clip : " + currentClip);

        if (attackIndex >= 0)
        {
            // get current attack
            Attack _attack = _character.attackList[attackIndex];
            switch (_attack.attackLimb) //TODO: we shouldn't need to lookup the attack limb using strings, rather we should just be able to assign limbs as gameobjects in inspector
            {
                case "leftArm":
                    _hitbox.attachHitbone(leftArm);
                    break;
                case "rightArm":
                    _hitbox.attachHitbone(rightArm);
                    break;
                case "head":
                    _hitbox.attachHitbone(head);
                    break;
            }
            _hitbox.resizeHitbox(_attack.hitboxSize); 
            _hitbox.startHitboxCollision();

        } else
        {
            _hitbox.stopHitboxCollision();
        }
    }
}
