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
            WeaponManager _wManager = _weapon.GetComponent<WeaponManager>();
        
            _weapon.transform.parent = _charManager.chest.transform;
            _wManager._weapon.chestLocRotPreset.ApplyTo(_weapon.transform);
            
            // set rotation on weapon off
            _weapon.GetComponent<SpinGameObject>().rotationSpeed = 0;
            _charManager.Weapon = _wManager;

            _wManager.GetComponent<Collider>().enabled = false; //disable weapon collider
        }
    }

    void FixedUpdate()
    {
    }
}
