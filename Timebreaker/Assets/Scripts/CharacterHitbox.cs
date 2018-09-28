using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHitbox : MonoBehaviour {

    private bool hitboxCreated;
    public LayerMask layerMask;

    private Vector3 _boxPosition;   // this is the center of the hitbox
    private Vector3 _boxSize;       // each vector is a fullsize
    private Quaternion _boxRotation;
    private ColliderState _colliderState;
    private enum ColliderState
    {
        Inactive,
        Active,       
        Colliding   
    }
    private Collider _hurtboxCol;    // this is the character's own hurtbox

    // Use this for initialization
    void Start () {
        hitboxCreated = true;
        _boxPosition = new Vector3(1, 1, 1);
        _boxSize = new Vector3(1, 1, 1);
        _hurtboxCol = GetComponent<CapsuleCollider>(); 
        stopHitboxCollision();
    }
	
	void Update () {
    }

    void FixedUpdate()
    {
        hurtboxCollision();
    }

    private void hurtboxCollision()
    {
        if (_colliderState == ColliderState.Inactive) {
            return;
        }

        _boxRotation = transform.rotation;
        
        // we divide boxSize by 2 to get the halfSize here
        Collider[] hitColliders = Physics.OverlapBox(_boxPosition, _boxSize/2, _boxRotation, layerMask);

        if (hitColliders.Length > 0)
        {
            foreach (Collider col in hitColliders)
            {
                // ensure hurtbox is on another character
                if (col != _hurtboxCol)
                {
                    _colliderState = ColliderState.Colliding;
                    // TODO: ADD WHAT HAPPENS WHEN HITBOX HITS COLLIDER HERE
                }
            }
        }
        
        else
        {
            _colliderState = ColliderState.Active;
        }
    }

    public void startHitboxCollision()
    {
        if (_colliderState != ColliderState.Colliding) 
        {
            _colliderState = ColliderState.Active;
        }
    }

    public void stopHitboxCollision()
    {
        _colliderState = ColliderState.Inactive;
    }

    public void resizeHitbox(Vector3 newHitboxSize)
    {
        _boxSize = newHitboxSize;
    }

    // hitbone should be a bone located on the middle of the limb we're targeting
    public void attachHitbone(GameObject hitbone)
    {
        _boxPosition = hitbone.transform.position;
    }

    
    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    private void OnDrawGizmos()
    {
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (hitboxCreated)
        {
            Gizmos.color = Color.red;
            //change hitbox color depending on state
            switch (_colliderState)
            {
                case ColliderState.Inactive:
                    Gizmos.color = Color.blue;
                    break;
                case ColliderState.Active:
                    Gizmos.color = Color.red;
                    break;
                case ColliderState.Colliding:
                    Gizmos.color = Color.yellow;
                    break;
            }

            //draw cube where Hitbox is
            Gizmos.matrix = Matrix4x4.TRS(_boxPosition, transform.rotation, transform.localScale);
            Gizmos.DrawWireCube(Vector3.zero, _boxSize);
        }
    }
}
