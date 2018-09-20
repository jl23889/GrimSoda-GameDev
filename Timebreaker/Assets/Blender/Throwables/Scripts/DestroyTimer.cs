using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour {

    public GameObject throwablePrefab;
    public GameObject respawnPoint;
    public GameObject explosionFx;

    public float timeToDestroy = 3f;
    public float explosionRadius = 5f;
    public float power = 1000f;
    public float upForce = 3f;
    
    // Use this for initialization
    void Start () {
        StartCoroutine(Destroy());
        // Instantiate particle effects
        Instantiate(explosionFx, gameObject.transform.position, Quaternion.identity);
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
        Instantiate(throwablePrefab, respawnPoint.transform.position, new Quaternion(-90, 0, 0, 1)); // respawn object; for testing purposes
    }


    // Update is called once per frame
    void Update () {
		
	}
}
