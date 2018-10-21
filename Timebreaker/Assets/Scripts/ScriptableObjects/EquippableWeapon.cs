using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;

[System.Serializable]
[CreateAssetMenu(fileName = "Weapon", menuName = "Define New Weapon")]
public class EquippableWeapon : ScriptableObject
{
    public string weaponName = "New Weapon";
    public Preset handLocRotPreset;
    public Preset chestLocRotPreset;
    public int damage = 30;
    public int uses = 5;
    public GameObject lightProjectilePrefab;
    public GameObject heavyProjectilePrefab;
    public float projectileVelocity = 30f;

    public enum WeaponType { TwoHandMelee, BigGun };
    public WeaponType weaponType;
}
