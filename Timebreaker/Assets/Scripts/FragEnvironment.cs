using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragEnvironment : MonoBehaviour
{

    public GameObject fragObject;      // this is the prefab object that is replaced upon fragmentation   
    public float velocityToFrag;       // this is the velocity threshold at which the object should fragment

    // Use this for initialization
    void Awake()
    {
    }

    // replace object with fragobject upon collision at velocity threshold
    // and trigger damage hitbox
    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > velocityToFrag)
        {
            Instantiate(fragObject, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}