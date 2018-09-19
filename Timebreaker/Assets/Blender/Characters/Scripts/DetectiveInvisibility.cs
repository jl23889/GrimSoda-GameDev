using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectiveInvisibility : MonoBehaviour {

	private GameObject body;
	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		body = gameObject.transform.Find("mdBody").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		// Find body object and turn off its rendering 
		if (animator.GetBool("Dodging")) {
			body.GetComponent<SkinnedMeshRenderer>().enabled = false;
		} else {
			body.GetComponent<SkinnedMeshRenderer>().enabled = true;
		}

	}
}
