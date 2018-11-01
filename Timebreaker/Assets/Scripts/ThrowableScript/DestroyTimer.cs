using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour {

    public Throwable _throwable;
    public GameObject throwablePrefab;
    public GameObject respawnPoint;
    public GameObject explosionFx;
    private GameObject explosionFxInstance;

    public float timeToDestroy = 3f;
    public float explosionRadius = 5f;
    public float power = 1000f;
    public float upForce = 3f;
    
    // Use this for initialization
    void Start () {
        //play sound
        GetComponent<AudioSource>().Play();
        // Instantiate particle effects
        if (_throwable != null)
        {
            if (_throwable.isExplosive)
            {
                explosionFxInstance = (GameObject)Instantiate(explosionFx, gameObject.transform.position, Quaternion.Euler(-90f, 0f, 0f));
            }
            Destroy(explosionFxInstance, timeToDestroy);
        }

        // Start Respawn timer 
        StartCoroutine(Respawn());

        // add explosion force to the children
        Vector3 explosionPos = transform.position;
        Rigidbody[] rbFragments = gameObject.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbFragments)
        {
            rb.AddExplosionForce(power, explosionPos, explosionRadius, upForce);
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSecondsRealtime(timeToDestroy);
        Destroy(gameObject);            // remove object
        //Instantiate(throwablePrefab, respawnPoint.transform.position, 
         //   Quaternion.Euler(new Vector3(-90, 0, 0))); // respawn object; for testing purposes
    }


    // Update is called once per frame
    void Update () {
		
	}
}
