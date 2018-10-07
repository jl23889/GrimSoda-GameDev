using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script to allow players to pick up and throw Throwable tagged objects

public class ThrowObject : MonoBehaviour {

    //components
    private float offsetY;
    private float throwVelocity;
    private Rigidbody rb;   // rigidbody of throwableObject
    private Collider col;   // collider of throwableObject
    private GameObject throwableObject;

    public LayerMask layerMask;
    public float _pickUpRadius;

    private bool _throwKeyPress;
    private AutoLock _autolock;
    private CharacterControl _characterControl;
    private bool _throwObjectCreated;

    private CharacterManager _charManager;
    private CharacterControl _charControl;
    private Animator animator;
    private string playerStr; 

    // Use this for initialization
    void Start () {
        rb = null;
        col = null;
        throwableObject = null;

        _charManager = GetComponent<CharacterManager>();
        _charControl = GetComponent<CharacterControl>();

        offsetY = _charManager._character.grabbingThrowableOffsetY;
        throwVelocity = _charManager._character.throwVelocity;
        animator = GetComponent<Animator>();
        playerStr = GetComponent<CharacterControl>().PlayerString;

        _autolock = GetComponent<AutoLock>();
        _characterControl = GetComponent<CharacterControl>();
        _throwKeyPress = false;
        _throwObjectCreated = true;
    }

    void Update()
    {
        if (Input.GetButtonDown(playerStr + "Throw"))
            _throwKeyPress = true;

        if (_charManager.IsGrabbingThrowable)
        {
            animator.SetBool("GrabbingThrowable", true);
        }
        else
        {
            animator.SetBool("GrabbingThrowable", false);
        }
    }

    void FixedUpdate()
    {
        // move throwable above player if it exists
        if (rb != null)
        {
            rb.MovePosition(transform.position + (new Vector3(0, offsetY)));
        }

        if (_charManager.IsGrabbingThrowable)
        {
            _characterControl.Targeting(40f, 75f);
        }
        else
        {
            ReleaseThrowable();
        }

        if (_charManager.IsHoldingWeapon)
        {
            _characterControl.Targeting(40f, 75f);
        }

        // throw object
        if (_throwKeyPress)
        {
            // shoot weapon if holding weapon and not shooting
            if (_charManager.IsHoldingWeapon && _charManager.CanShoot)
            {
                if (_autolock.Target != null)
                {
                    _characterControl.Movement = Vector3.zero;
                    transform.LookAt(new Vector3(_autolock.Target.transform.position.x, transform.position.y, _autolock.Target.transform.position.z));
                }
                ShootBigGun();
            }

            if (_charManager.IsGrabbingThrowable)
            {
                if (_autolock.Target != null)
                {
                    _characterControl.Movement = Vector3.zero;
                    transform.LookAt(new Vector3(_autolock.Target.transform.position.x, transform.position.y, _autolock.Target.transform.position.z));
                }
                ReleaseThrowable(true);
            }
            else //try pick things up
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 5f + transform.up * 3f, _pickUpRadius, layerMask);
                if (hitColliders.Length != 0)
                {
                    foreach (Collider i in hitColliders)
                    {
                        if (i.gameObject.tag == "Throwable" && !_charManager.IsGrabbingThrowable 
                            && !_charManager.IsHoldingWeapon && !animator.GetBool("Sprinting"))
                        {
                            throwableObject = i.gameObject;
                            rb = throwableObject.GetComponent<Rigidbody>();
                            col = throwableObject.GetComponent<MeshCollider>();

                            // make gameobject child of player (so it moves with player)
                            throwableObject.transform.parent = transform;
                            // set isGrabbing flag to true
                            _charManager.IsGrabbingThrowable = true;

                            rb.isKinematic = true; //disable unity physics
                            col.enabled = false;   //disable collider
                        }
                    }
                }

            }
        }
        
        _throwKeyPress = false;
    }

    public void ReleaseThrowable(bool addForce = false)
    {
        // release object if it is parented to player
        if (throwableObject != null && throwableObject.transform.IsChildOf(transform))
        {
            rb.isKinematic = false; //enable unity physics
            col.enabled = true;   // enable collider

            // unparent the player to the throwable
            throwableObject.transform.parent = null;
            // set is grabbing flag to false
            _charManager.IsGrabbingThrowable = false;

            if (addForce)
            {
                Vector3 velocity = Vector3.zero;
                if (_autolock.Target == null)
                {
                    velocity = (transform.forward.normalized + Vector3.up) * throwVelocity;
                }
                else
                {
                    velocity = Launch(throwableObject.transform, _autolock.Target.transform, 45);
                }

                rb.velocity = velocity;
                rb.AddTorque(Random.Range(-500,500), Random.Range(-500, 500), Random.Range(-500, 500), ForceMode.Impulse);
            }

            throwableObject = null;
            rb = null;
            col = null;
        }
    }

    public void ShootBigGun()
    {
        if (_throwKeyPress)
        {
            // attach gun to hand
            _charManager.RangedWeapon.transform.parent = _charManager.rightHand.transform;
            _charManager.RangedWeapon._rangedWeapon.handLocRotPreset.ApplyTo(_charManager.RangedWeapon.transform);
            animator.SetBool("ShootingBigGun", true);
            if (_autolock.Target != null)
            {
                _charManager.RangedWeapon.ShootLight(_autolock);
            }
            else
            {
                _charManager.RangedWeapon.ShootLight(transform.forward);
            }

            // reattach gun to hand
            StartCoroutine(PlaceBigGunChest(0.5f));
        }
    }

    IEnumerator PlaceBigGunChest(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("ShootingBigGun", false);
        _charManager.RangedWeapon.transform.parent = _charManager.chest.transform;
        _charManager.RangedWeapon._rangedWeapon.chestLocRotPreset.ApplyTo(_charManager.RangedWeapon.transform);
    }

    private void OnDrawGizmos()
    {
        if (_throwObjectCreated)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + transform.forward * 5f + transform.up * 3f, _pickUpRadius);
        }
    }

    Vector3 Launch(Transform throwableObject, Transform TargetObjectTF, float LaunchAngle)
    {
        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.
        Vector3 throwableXZPos = new Vector3(throwableObject.transform.position.x, throwableObject.position.y, throwableObject.transform.position.z);
        Vector3 targetXZPos = new Vector3(TargetObjectTF.position.x, throwableObject.position.y, TargetObjectTF.position.z);

        // rotate the object to face the target
        throwableObject.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(throwableXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(LaunchAngle * Mathf.Deg2Rad);
        float H = TargetObjectTF.position.y - throwableObject.transform.position.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = throwableObject.transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        return globalVelocity;
    }
}
