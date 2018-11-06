using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderCheck : MonoBehaviour {
    private CharacterManager _selfCM;

	// Use this for initialization
	void Start () {
        _selfCM = GetComponent<CharacterManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Border")
        {
            transform.position = new Vector3(16.1f, 7.3f, 48.3f);
            Debug.Log("1");
            _selfCM.CurrentHealth = _selfCM.CurrentHealth - 40f;
        }
    }
}
