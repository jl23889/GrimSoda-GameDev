using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class train_move : MonoBehaviour {
    private Vector3 _moving_direction;
    private float _currentSpeed;
    public float _speed = 0.1f;
    public float stoptime = 20f;
    private float time_counter = 0;

	// Use this for initialization
	void Start () {
        _currentSpeed = _speed;
        _moving_direction = new Vector3(0f, 0f, _currentSpeed);
    }

    void FixedUpdate()
    {
        if (transform.localPosition.z < -1f)
        {
            _moving_direction = new Vector3(0f, 0f, _speed);
        }
        if (transform.localPosition.z > 1f)
        {
            _moving_direction = new Vector3(0f, 0f, -_speed);
        }

        if (transform.localPosition.z > -0.5f && transform.localPosition.z < -0.1f)
        {
            _currentSpeed = Mathf.Lerp(0, _speed, Mathf.Abs(transform.localPosition.z) / 0.5f);
            _moving_direction = new Vector3(0f, 0f, _currentSpeed);
        }
        else if (transform.localPosition.z > -0.1f && transform.localPosition.z < -0.05f)
        {
            _moving_direction = new Vector3(0f, 0f, _speed / 5f);
        }
        else if (transform.localPosition.z > 0f)
        {
            _moving_direction = Vector3.zero;
        }


        transform.position = transform.position + _moving_direction;
    }
}
