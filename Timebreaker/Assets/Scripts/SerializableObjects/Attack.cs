using UnityEngine;
using System.Collections;

[System.Serializable]
[CreateAssetMenu(fileName = "Attack", menuName = "Define New Attack")]
public class Attack : ScriptableObject
{
    public string attackName = "New Attack";
    public AnimationClip animationClip;
    public int damage = 1;
    public string attackLimb;
    public Vector3 hitboxSize = new Vector3(1,1,1);
    public bool knockback = false;
    public bool knockdown = false;
    public bool knockup = false;
}
