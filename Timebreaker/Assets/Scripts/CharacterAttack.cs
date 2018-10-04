using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    private CharacterManager _char;
    private List<Attack> _charAttacks;
    private Hitbox _charHitbox;
    private Collider _charHurtboxCol;    // this is the character's own hurtbox collider
    private List<Collider> _hurtboxColBuffer; // this contains the colliders hit by the current attack   
    private Animator animator;
    private AnimationClip currentClip;
    private GameObject vfxBone;         // this is the bone that has the vfx to be played
    private bool attackInProgress;

    [HideInInspector]
    private Attack _attack; // the current attack

    // Use this for initialization
    void Start()
    {
        _char = GetComponent<CharacterManager>();
        _charAttacks = _char._character.attackList;
        _charHitbox = GetComponent<Hitbox>();
        _charHurtboxCol = GetComponent<CapsuleCollider>();
        _hurtboxColBuffer = new List<Collider>();
        _hurtboxColBuffer.Add(_charHurtboxCol);
        _attack = null;
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // get the current clip playing 
        currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;

        // check if currentclip matches an attack animation clip in the attackList
        int attackIndex = _charAttacks.FindIndex(attack => attack.animationClip == currentClip);

        // display name of current animation clip in console
        //Debug.Log("Starting clip : " + currentClip);

        if (attackIndex >= 0)
        {
            // check if animation is the same attack
            //  if not, we clear the hurtbox buffer so we can triggerhits again
            if (_attack != null)
            {
                if (_attack.animationClip != currentClip)
                {
                    ClearHurtboxBuffer();
                }
            }

            // get current attack
            _attack = _charAttacks[attackIndex];
            
            switch (_attack.attackLimb) //TODO: we shouldn't need to lookup the attack limb using strings, rather we should just be able to assign limbs as gameobjects in inspector
            {
                case "leftArm":
                    _charHitbox.HitboxCenterPosition(_char.leftArm);
                    vfxBone = _char.leftHand;
                    break;
                case "rightArm":
                    _charHitbox.HitboxCenterPosition(_char.rightArm);
                    vfxBone = _char.rightHand;
                    break;
                case "head":
                    _charHitbox.HitboxCenterPosition(_char.head);
                    vfxBone = _char.headEnd;
                    break;
            }
            _charHitbox.ResizeHitbox(_attack.hitboxSize);
            _charHitbox.StartHitboxCollision();
            TriggerHit(_charHitbox.GetHurtboxCollisions());
        }
        else
        {
            _charHitbox.StopHitboxCollision();
            ClearHurtboxBuffer();
            _attack = null;
        }

        attackInProgress = false;
    }

    // trigger a hit for every hurtbox collider in the list
    private void TriggerHit(Collider[] hurtboxCols)
    {
        if (hurtboxCols == null || hurtboxCols.Length == 0)
            return;
        else
        {
            foreach (Collider col in hurtboxCols)
            { 
                // check if the hurtbox has not already been affected by this attack
                if (!_hurtboxColBuffer.Contains(col))
                {
                    var _charHit = col.gameObject.GetComponent<CharacterManager>();

                    if (_charHit != null)
                    {
                        // trigger vfx on the bone that is in motion
                        vfxBone.GetComponent<ParticleSystem>().Play(false);
                        // make the character that is hit take damage from attack
                        _charHit.TakeDamage(_attack);
                        _charHitbox.DetectCollision();
                    }

                    // add the colider to the buffer so that it is not affected by the same attack
                    _hurtboxColBuffer.Add(col);
                }
            }
        }
    }

    // clear the buffer
    private void ClearHurtboxBuffer()
    {
        _hurtboxColBuffer.Clear();
        _hurtboxColBuffer.Add(_charHurtboxCol); //we add the characters own hurtbox collider back in
    }
}
