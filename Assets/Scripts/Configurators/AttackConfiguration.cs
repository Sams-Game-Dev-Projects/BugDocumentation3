using UnityEngine;

/// <summary>
/// Controls how this weapon performs an attack
/// For example, is this weapon automatic?
/// How much damage does an attack deal?
/// </summary>
[CreateAssetMenu(menuName = "Attack Configuration")]
public class AttackConfiguration : ScriptableObject
{
    public Vector2Int damageRange;  //Holds a min and max damage amound
    public LayerMask hitMask;       //What can this weapon hit? Could be used to allow for weapons that ignore objects like walls
    public Vector3 spread;          //How accurate is this weapon? The more spread, the less accurate
    public bool automaticAttacking; //Does this attack happen automatically when the attak command is held
    public float attackRate;        //How quickly can the player use this weapon since the last time it attack
    public float maxDistance;       //How far can a target be
    public float sphereRadius;      //Used to calculate if the projectile hits a surface such as a wall
    public int ammoNeededPerAttack; //How much ammo does one attack consume? A weapon like plasma rifle might consume 2 plasma per shot whereas a plasma pistol only uses 1
    public int projectilesPerShot = 1; //How many projectiles are fired per attack (e.g., shotgun pellets)

    //Randomly selects a number between the min and max damage range
    public int CalculateDamage { get { return Random.Range(damageRange.x, damageRange.y + 1); } }
}
