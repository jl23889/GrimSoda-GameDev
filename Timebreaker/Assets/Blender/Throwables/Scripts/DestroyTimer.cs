using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour {

    public float timeToDestroy = 3f;
    public float explosionRadius = 5f;
    public float power = 1000f;
    public float upForce = 3f;

    // Use this for initialization
    void Start () {
        StartCoroutine(Destroy());
        // add explosion force to the children
        Vector3 explosionPos = transform.position;
        Rigidbody[] rbFragments = gameObject.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbFragments)
        {
            rb.AddExplosionForce(power, explosionPos, explosionRadius, upForce);
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSecondsRealtime(timeToDestroy);
        Destroy(gameObject);
    }


    // Update is called once per frame
    void Update () {
		
	}
}
