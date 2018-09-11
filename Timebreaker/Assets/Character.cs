using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
    public float speed;
    public float jumpForce;

    public CharacterController controller;
    public LayerMask groundLayers;
    public CapsuleCollider col;

    private Animator animator;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
    }
	
	// Update is called once per frame
	void Update () {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move(x, z);
        jump();
    }
    void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("here");
        }
    }


    void move(float x, float z)
    {
        Vector3 movement = new Vector3(x, 0.0f, z);
        movement = movement.normalized * Time.deltaTime * speed;
        if (movement != Vector3.zero)
        {
            controller.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.2f);
            controller.transform.Translate(movement, Space.World);     
            animator.SetBool("Moving", true);
        }
        else
        {
            animator.SetBool("Moving", false);
        }
    }
}
