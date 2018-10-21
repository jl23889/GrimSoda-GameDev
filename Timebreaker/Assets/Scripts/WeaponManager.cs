using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
    
    public EquippableWeapon _weapon;
    public int remainingCharges;
    public RespawnPointManager _respawnPoint;
    private SpinGameObject _spinScript;
    private MeshRenderer _renderer;
    public float timeToRespawn = 3f; // set value to negative if should not respawn
    private bool isResetting;
    private float offsetY;
    private float initialSpinSpeed;
    private Vector3 initialRotation;

    private AudioSource _audioSource;

    // Use this for initialization
    void Awake () {
        _renderer = GetComponent<MeshRenderer>();
        _spinScript = GetComponent<SpinGameObject>();
        _audioSource = GetComponent<AudioSource>();
        
        remainingCharges = _weapon.uses;
        isResetting = false;
        offsetY = transform.position.y;
        initialSpinSpeed = _spinScript.rotationSpeed;
        initialRotation = transform.rotation.eulerAngles;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (remainingCharges <= 0 && !isResetting)
        {
            ResetWeapon();
        }
	}

    // either reset the weapon or destroy it
    private void ResetWeapon()
    {
        if (timeToRespawn >= 0 && _respawnPoint != null)
        {
            if (!_respawnPoint.HasGob())
            {
                StartCoroutine(StartResetWeapon());
                _renderer.enabled = false;
                isResetting = true;
            }
            else Destroy(gameObject);
        }
        else Destroy(gameObject);
    }

    // reset weapon stats and move it to location
    IEnumerator StartResetWeapon(bool resetRotation = true)
    {
        yield return new WaitForSecondsRealtime(timeToRespawn);
        remainingCharges = _weapon.uses;
        GetComponent<Collider>().enabled = true; // enable collider

        transform.parent = _respawnPoint.transform; // make weapon child of respawn point
        transform.position = _respawnPoint.transform.position + new Vector3(0, offsetY, 0);
        if (resetRotation) transform.rotation = Quaternion.Euler(initialRotation);
        _spinScript.rotationSpeed = initialSpinSpeed;
        _renderer.enabled = true;
        isResetting = false;
    }

    public void ShootLight(Vector3 direction)
    {
        _audioSource.Play();

        remainingCharges--;
        GameObject projectile = Instantiate(_weapon.lightProjectilePrefab,
            transform.position,
            Quaternion.identity);
        projectile.GetComponent<Rigidbody>().AddRelativeForce(direction * _weapon.projectileVelocity, ForceMode.VelocityChange);
        projectile.GetComponent<Rigidbody>().useGravity = false;    
    }

    public void ShootLight(AutoLock autoLock)
    {
        _audioSource.Play();

        remainingCharges--;
        GameObject projectile = Instantiate(_weapon.lightProjectilePrefab,
            transform.position,
            Quaternion.identity);
        Vector3 direction = (autoLock.Target.transform.position + new Vector3(0, 2.12f, 0) - transform.position).normalized;
        projectile.GetComponent<Rigidbody>().AddRelativeForce(direction * _weapon.projectileVelocity, ForceMode.VelocityChange);
        projectile.GetComponent<Rigidbody>().useGravity = false;
    }

    public void ShootHeavy(Vector3 direction)
    {
        _audioSource.Play();

        remainingCharges = remainingCharges - 3;
        GameObject projectile = Instantiate(_weapon.heavyProjectilePrefab,
            transform.position,
            Quaternion.identity);
        projectile.GetComponent<Rigidbody>().AddForce(direction * _weapon.projectileVelocity, ForceMode.VelocityChange);
    }
}
