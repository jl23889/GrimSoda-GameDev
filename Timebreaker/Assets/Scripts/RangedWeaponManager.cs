using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

public class RangedWeaponManager : MonoBehaviour {

    public enum WeaponType { BigGun }
    public RangedWeapon _rangedWeapon;
    public int remainingAmmo;

	// Use this for initialization
	void Awake () {
        remainingAmmo = _rangedWeapon.ammo;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (remainingAmmo <= 0)
        {
            Destroy(gameObject);
        }
	}

    public void Shoot(Vector3 direction)
    {
        remainingAmmo--;
        GameObject projectile = Instantiate(_rangedWeapon.projectilePrefab,
            transform.position,
            Quaternion.identity);
        projectile.GetComponent<Rigidbody>().AddForce(direction * _rangedWeapon.projectileVelocity, ForceMode.VelocityChange);
    }
}
