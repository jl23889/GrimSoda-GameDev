using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class train_move : MonoBehaviour {
    private Vector3 _moving_in_direction;
    private float _direction = 1;
    private float _currentSpeed;
    public float _speed = 1f;

    private float _inStationStopTime;
    public float _inStationTimeMin = 2f;
    public float _inStationTimeMax = 10f;

    private float _turnAroundStopTime;
    public float _turnAroundTimeMin = 2f;
    public float _turnAroundTimeMax = 10f;

    private float time_counter = 0;
    private float time_counter2 = 0;
    private bool _isAccelerating = false;

    public AudioClip _trainWhistleLongClip;
    public AudioSource _trainWhistleAudio;

	// Use this for initialization
	void Start () {
        _currentSpeed = _speed;
        _moving_in_direction = new Vector3(0f, 0f, _currentSpeed * _direction);
        _turnAroundStopTime = Random.Range(_turnAroundTimeMin, _turnAroundTimeMax);
        _inStationStopTime = Random.Range(_inStationTimeMin, _inStationTimeMax);
       
    }

    void FixedUpdate()
    {
        if (transform.localPosition.z < -2f && _direction == -1)
        {
            TurnAround();
            _direction = 1;            
        }
        
        if (transform.localPosition.z > 2f && _direction == 1)
        {
            TurnAround();
            _direction = -1;           
        }

        StopAtTurning(_turnAroundStopTime);

        if (!_isAccelerating)
            Stoping();
        else
            Accelerating();

        _moving_in_direction = new Vector3(0f, 0f, _currentSpeed * _direction);
        transform.position = transform.position + _moving_in_direction;
    }

    private void Accelerating()
    {
        if (Mathf.Abs(transform.localPosition.z) < 0.05f)
        {
            _currentSpeed = _speed / 10f;
        }
        else if (Mathf.Abs(transform.localPosition.z) < 0.5f && Mathf.Abs(transform.localPosition.z) > 0.05f)
        {
            _currentSpeed = Mathf.Lerp(0, _speed, Mathf.Abs(transform.localPosition.z) / 0.5f);
        }
        else
        {
            _currentSpeed = _speed;
            _isAccelerating = false;
        }
    }

    private void Stoping()
    {
        if (Mathf.Abs(transform.localPosition.z) < 0.5f && Mathf.Abs(transform.localPosition.z) > 0.05f)
        {
            _currentSpeed = Mathf.Lerp(0, _speed, Mathf.Abs(transform.localPosition.z) / 0.5f);
        }
        else if (Mathf.Abs(transform.localPosition.z) < 0.05f)
        {
            StopAtStation(_inStationStopTime);
        }
    }

    private void StopAtStation(float stoptime)
    {
        _currentSpeed = 0f;
        time_counter = time_counter + Time.fixedDeltaTime;
        if (time_counter > stoptime)
        {
            _isAccelerating = true;
            time_counter = 0;
        }
    }

    private void StopAtTurning(float stoptime)
    {
        _currentSpeed = 0f;
        time_counter2 = time_counter2 + Time.fixedDeltaTime;
        if (time_counter2 > stoptime)
        {
            _currentSpeed = _speed;
            
        }
    }

    private void TurnAround()
    {
        transform.rotation = Quaternion.Euler(0f, (90f + 90f *_direction), 0f);
        _trainWhistleAudio.clip = _trainWhistleLongClip;
        _trainWhistleAudio.Play();
        time_counter2 = 0;
    }
}
