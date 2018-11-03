using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerVFX : MonoBehaviour {

    public GameObject groundHitFx;
    public AnimationClip hammerVerticalAnim;

    private WeaponManager _weaponManager;
    private AudioSource _sfx;

    private void Awake()
    {
        _weaponManager = GetComponentInParent<WeaponManager>();
        _sfx = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Terrain" && _weaponManager.CharacterManager.CurrentClip == hammerVerticalAnim)
        {
            // play sound
            _sfx.Play();

            // instantiate vfx
            ContactPoint contact = col.contacts[0];
            GameObject groundHitFxInstance = (GameObject)Instantiate(groundHitFx, contact.point + Vector3.up, Quaternion.Euler(-90f, 0f, 0f));
            Destroy(groundHitFxInstance, 3.0f);
        }
    }
}
