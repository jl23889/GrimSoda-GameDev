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

            switch (_wManager._weapon.weaponType)
            {
                case EquippableWeapon.WeaponType.TwoHandMelee:
                    _weapon.transform.parent = _charManager.rightHand.transform;
                    _wManager._weapon.handLocRotPreset.ApplyTo(_weapon.transform);
                    break;
                case EquippableWeapon.WeaponType.BigGun:
                    _weapon.transform.parent = _charManager.chest.transform;
                    _wManager._weapon.chestLocRotPreset.ApplyTo(_weapon.transform);
                    break;
            }
            
            // set rotation on weapon off
            _weapon.GetComponent<SpinGameObject>().rotationSpeed = 0;
            _charManager.Weapon = _wManager;
            _wManager.CharacterManager = _charManager;

            _wManager.GetComponent<Collider>().enabled = false; //disable weapon collider
        }
    }

    void FixedUpdate()
    {
    }
}
