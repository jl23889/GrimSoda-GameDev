using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHitbox : MonoBehaviour {

    private bool hitboxCreated;
    public LayerMask layerMask;
    public Character _character;

    private Vector3 _boxPosition;   // this is the center of the hitbox
    private Vector3 _boxSize;       // each vector is a fullsize
    private Quaternion _boxRotation;
    [SerializeField]
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
        StopHitboxCollision();
    }

    public Collider[] GetHurtboxCollisions()
    {
        // return empty array if collider state is inactive
        if (_colliderState == ColliderState.Inactive) {
            return new Collider[0];
        }

        _boxRotation = transform.rotation;
        
        // we divide boxSize by 2 to get the halfSize here
        Collider[] hitColliders = Physics.OverlapBox(_boxPosition, _boxSize/2, _boxRotation, layerMask);
        // set collider state to active if colliders are not hitting anything
        if (hitColliders.Length == 0)
        {
            _colliderState = ColliderState.Active;
        }
        return hitColliders;
    }

    public void DetectCollision()
    {
        _colliderState = ColliderState.Colliding;
    }

    public void StartHitboxCollision()
    {
        if (_colliderState != ColliderState.Colliding) 
        {
            _colliderState = ColliderState.Active;
        }
    }

    public void StopHitboxCollision()
    {
        _colliderState = ColliderState.Inactive;
    }

    public void ResizeHitbox(Vector3 newHitboxSize)
    {
        _boxSize = newHitboxSize;
    }

    // hitbone should be a bone located on the middle of the limb we're targeting
    public void AttachHitbone(GameObject hitbone)
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
