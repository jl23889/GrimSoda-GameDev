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
    private Animator animator;
    private string playerStr; 

    // Use this for initialization
    void Start () {
        rb = null;
        col = null;
        throwableObject = null;

        _charManager = GetComponent<CharacterManager>();
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

        // throw object
        if (_throwKeyPress)
        {
            if (_charManager.IsGrabbingThrowable)
            {
                _characterControl.Targeting(40, 60);
                if (_autolock.Target != null)
                    transform.LookAt(_autolock.Target.transform);
                ReleaseThrowable(true);
            }
            else
            {
                //try pick things up
                Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 5f + transform.up * 3f, _pickUpRadius, layerMask);
                if (hitColliders.Length != 0)
                {
                    foreach (Collider i in hitColliders)
                    {
                        if (i.gameObject.tag == "Throwable" && !_charManager.IsGrabbingThrowable)
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
        
        // drop the item
        if (!_charManager.IsGrabbingThrowable)
        {
            ReleaseThrowable();
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
                rb.AddForce((transform.forward + transform.up) * throwVelocity *0.8f, ForceMode.Impulse);
                rb.AddTorque(Random.Range(-500,500), Random.Range(-500, 500), Random.Range(-500, 500), ForceMode.Impulse);
            }

            throwableObject = null;
            rb = null;
            col = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (_throwObjectCreated)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + transform.forward * 5f + transform.up * 3f, _pickUpRadius);
        }
    }
}
