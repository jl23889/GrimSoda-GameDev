using UnityEngine;
using System.Collections;

public class vikingAnimTest : MonoBehaviour
{
    public float speed;
    public float jumpForce;

    public CharacterController controller;
    public LayerMask groundLayers;
    public CapsuleCollider col;

    private Animator animator;

    // Helper method to wait
    bool attacking = false;
    // bool waitActive = false;

    // IEnumerator Wait(){
    //     // waitActive = true;
    //     Debug.Log("Waiting");
    //     yield return new WaitForSeconds (3.0f);
    //     Debug.Log("Stopped waiting");
    //     animator.SetBool("KickH", false);
    //     Debug.Log("STOPPED ATTACK");
    //     // canAttack = true;
    //     // waitActive = false;
    //  }

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move(x, z);
        punchM();
        jump();
        
    }
    void jump() //testing kick for now
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SPACE PRESSED");
            animator.SetBool("KickH", true);
        }
    }
    void punchM()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P PRESSED");
            animator.SetBool("PunchM", true);
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