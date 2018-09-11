using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour {
    public Transform player;
    public CharacterController controller;

    public float speed;
    public float range;

    private Animator mobAnimator;

	// Use this for initialization
	void Start () {
        mobAnimator = GetComponent<Animator>();

    }
	
	// Update is called once per frame
	void Update () {
        if (!inRange())
        {           
            chase();            
        }
        else
        {
            Vector3 movement = new Vector3(player.position.x - transform.position.x, 0.0f, player.position.z - transform.position.z);
            controller.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement.normalized), 0.2f);
            mobAnimator.SetBool("Moving", false);
        }
	}

    bool inRange() {
        return (Vector3.Distance(transform.position, player.position) < range);
    }

    void chase() {
        Vector3 movement = new Vector3(player.position.x - transform.position.x, 0.0f, player.position.z - transform.position.z);
        controller.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement.normalized), 0.2f);
        controller.transform.Translate(movement * Time.deltaTime * speed, Space.World);
        mobAnimator.SetBool("Moving", true);
    }
}
