using UnityEngine;
using System.Collections;

[System.Serializable]
[CreateAssetMenu(fileName = "Attack", menuName = "Define New Attack")]
public class Attack : ScriptableObject
{
    public string attackName = "New Attack";
    public AnimationClip animationClip;
    public float damage = 1;
    public float hitStunDuration = 0.5f;
    public string attackLimb;
    public Vector3 hitboxSize = new Vector3(1,1,1);
    public bool knockback = false;
    public bool knockdown = false;
    public bool knockup = false;
    public float knockbackVelocity = 0f;
    public float knockupVelocity = 0f;
}
