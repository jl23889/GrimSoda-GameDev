using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

public class CharacterCollectItem : MonoBehaviour {

    private CharacterManager _charManager;
   
    void Start()
    {
        _charManager = GetComponent<CharacterManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Weapon") && !_charManager.IsHoldingWeapon)
        {
            GameObject _weapon = other.gameObject;
            RangedWeaponManager _rwManager = _weapon.GetComponent<RangedWeaponManager>();
            // make gameobject child of player (so it moves with player)
            _weapon.transform.parent = _charManager.rightHand.transform;
            _rwManager._rangedWeapon.locationRotationScalePreset.ApplyTo(_weapon.transform);
            
            // set rotation on weapon off
            _weapon.GetComponent<SpinGameObject>().rotationSpeed = 0;
            _charManager.RangedWeapon = _rwManager;

            _rwManager.GetComponent<Collider>().enabled = false; //disable weapon collider
        }
    }

    void FixedUpdate()
    {
    }
}
