using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour {
    public float _minSize = 120f;
    public float _shrinkRate = 1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (transform.localScale.x > _minSize)
        {
            transform.localScale -= new Vector3(1f, 1f, 1f) * Time.fixedDeltaTime * _shrinkRate;
        }		
	}
}
