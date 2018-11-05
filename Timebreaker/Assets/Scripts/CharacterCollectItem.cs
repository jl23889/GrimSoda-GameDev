using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
                    _weapon.transform.localPosition = _wManager._weapon.handLocRotPreset.transform.position;
                    _weapon.transform.localRotation = _wManager._weapon.handLocRotPreset.transform.rotation;
                    _weapon.transform.localScale = _wManager._weapon.handLocRotPreset.transform.localScale;
                    break;
                case EquippableWeapon.WeaponType.BigGun:
                    _weapon.transform.parent = _charManager.chest.transform;
                    _weapon.transform.localPosition = _wManager._weapon.chestLocRotPreset.transform.position;
                    _weapon.transform.localRotation = _wManager._weapon.chestLocRotPreset.transform.rotation;
                    _weapon.transform.localScale = _wManager._weapon.chestLocRotPreset.transform.localScale;
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
