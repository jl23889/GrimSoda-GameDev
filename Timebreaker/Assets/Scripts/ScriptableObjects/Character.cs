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
    public float throwVelocity = 50f;
    public float grabbingThrowableOffsetY = 7f;
    public float runSpeed = 8f;
    public float jumpHeight = 10f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float sprintMultiplier = 2f;
    public float dodgeMultiplier = 2f;
    public List<Attack> attackList;
    public List<AnimationClip> canShootAnimationList;
}
