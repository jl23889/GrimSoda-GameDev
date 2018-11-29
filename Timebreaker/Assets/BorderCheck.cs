using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderCheck : MonoBehaviour {
    private CharacterManager _selfCM;
    private CharacterControl _selfCC;
    private Rigidbody _rb;
    private Animator animator;
    private float inputTimer;

    // Use this for initialization
    void Start () {
        _selfCM = GetComponent<CharacterManager>();
        _selfCC = GetComponent<CharacterControl>();
        animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Border")
        {
            //transform.position = new Vector3(16.1f, 7.3f, 48.3f);
            _selfCM.CurrentHealth = _selfCM.CurrentHealth - 40f;
            _selfCC.Movement = Vector3.zero;

            animator.SetTrigger("GetHit");
            StartCoroutine(DisableInput(0.4f));

            Vector3 pushback_direction = new Vector3(other.transform.position.x - transform.position.x, 0f, other.transform.position.z - transform.position.z);
            _rb.AddForce(pushback_direction * 4f, ForceMode.VelocityChange);            
        }

        if (_selfCM.CurrentHealth <= 0 && !_selfCM.IsDead)
        {
            _selfCM.OnDeath();
        }
    }

    // disables player input for length of hsd
    IEnumerator DisableInput(float hsd)
    {
        inputTimer = hsd;
        _selfCM.IsHitStunned = true;
        animator.SetBool("HitStun", true);
        while (inputTimer > 0)
        {
            inputTimer -= Time.deltaTime;
            yield return null;
        }
        _selfCM.IsHitStunned = false;
        animator.SetBool("HitStun", false);
    }
}
