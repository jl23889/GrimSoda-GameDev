using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;

[System.Serializable]
[CreateAssetMenu(fileName = "Character", menuName = "Define New Ranged Weapon")]
public class RangedWeapon : ScriptableObject
{
    public string weaponName = "New Ranged Weapon";
    public Preset locationRotationScalePreset;
    public GameObject projectilePrefab;
    public float projectileVelocity = 30f;
    public int damage = 10;
    public int ammo = 5;
    public float hitStunDuration = 2f;
    public Vector3 hitboxSize = new Vector3(10, 10, 10);
    public bool knockback = true;
    public bool knockdown = false;
    public bool knockup = true;
    public float knockbackVelocity = 10f;
    public float knockupVelocity = 5f;
}
