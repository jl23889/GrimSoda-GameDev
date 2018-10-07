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
    public Preset handLocRotPreset;
    public Preset chestLocRotPreset;
    public GameObject lightProjectilePrefab;
    public GameObject heavyProjectilePrefab;
    public float projectileVelocity = 30f;
    public int damage = 10;
    public int ammo = 5;
}
