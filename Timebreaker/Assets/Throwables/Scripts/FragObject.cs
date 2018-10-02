using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragObject : MonoBehaviour {

    public Throwable _throwable;
    private GameObject fragObject;      // this is the prefab object that is replaced upon fragmentation   
    private float velocityToFrag;       // this is the velocity threshold at which the object should fragment
    private Hitbox _hitbox;
    private List<Collider> _hurtboxColBuffer; // this contains the colliders hit by the current attack   

    // Use this for initialization
    void Awake () {
        if (_throwable != null)
        {
            fragObject = _throwable.fragObject;
            velocityToFrag = _throwable.velocityToFrag;
            _hitbox = GetComponent<Hitbox>();
            _hurtboxColBuffer = new List<Collider>();
        }
	}
	
    // replace object with fragobject upon collision at velocity threshold
    // and trigger damage hitbox
	void OnCollisionEnter (Collision collision) {
        if (collision.relativeVelocity.magnitude > velocityToFrag)
        {
            _hitbox.HitboxCenterPosition(gameObject);
            _hitbox.ResizeHitbox(_throwable.hitboxSize);
            _hitbox.StartHitboxCollision();
            TriggerHit(_hitbox.GetHurtboxCollisions());
            _hitbox.StopHitboxCollision();
            Instantiate(fragObject, transform.position, transform.rotation);
            Destroy(gameObject);
        }
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
                        // make the character that is hit take damage from attack
                        _charHit.TakeDamage(_throwable);
                        _hitbox.DetectCollision();
                    }

                    // add the colider to the buffer so that it is not affected by the same attack
                    _hurtboxColBuffer.Add(col);
                }
            }
        }
    }
}
