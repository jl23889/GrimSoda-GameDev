using System.Collections;
using UnityEngine;

public class ThrowableFrag : MonoBehaviour {

    public GameObject throwableFrag;
    public float velocityToFrag= 4.0f; // this is the velocity threshold at which the object should explode

    // Use this for initialization
    void Start () {		
	}
	
	// Update is called once per frame
	void OnCollisionEnter (Collision collision) {
        if (collision.relativeVelocity.magnitude > velocityToFrag)
        {
            Instantiate(throwableFrag, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
