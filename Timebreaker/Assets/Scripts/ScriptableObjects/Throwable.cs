using UnityEngine;
using System.Collections;

[System.Serializable]
[CreateAssetMenu(fileName = "Throwable", menuName = "Define New Throwable")]
public class Throwable : ScriptableObject
{
    public string throwableName = "New Throwable";
    public GameObject fragObject;
    public bool isExplosive = false;
    public float velocityToFrag = 10f;
    public int damage = 10; 
    public float hitStunDuration = 2f;
    public Vector3 hitboxSize = new Vector3(10, 10, 10);
    public bool knockback = true;
    public bool knockdown = false;
    public bool knockup = true;
    public float knockbackVelocity = 10f;
    public float knockupVelocity = 10f;
}
