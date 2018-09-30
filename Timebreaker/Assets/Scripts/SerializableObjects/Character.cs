using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "Character", menuName = "Define New Character")]
public class Character : ScriptableObject
{
    public string charName = "New Character";
    public int healthTotal = 100;
    public int staminaTotal = 100;
    public float runSpeed = 1;
    public List<Attack> attackList;
}
