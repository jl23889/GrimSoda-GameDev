using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// spins gameObject at a constant speed
public class SpinGameObject : MonoBehaviour {

    public float rotationSpeed = 20f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(0, 0, 1) * rotationSpeed * Time.deltaTime);
	}
}
