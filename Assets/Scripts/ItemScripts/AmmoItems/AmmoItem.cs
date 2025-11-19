using UnityEngine;

/// <summary>
/// Ammo is needed to use some weapons
/// This scriptable object dictates what type of ammo it is
/// And how much extra damage this ammo does to a target
/// </summary>
[CreateAssetMenu(menuName = "Ammo Item")]
public class AmmoItem : ItemBase
{
    public int clipSize;
    public AmmoType ammoType;
    public Vector2Int damageModifierRange;

    public int CalculateDamage { get { return Random.Range(damageModifierRange.x, damageModifierRange.y + 1); } }

    public override string GetItemType { get { return "Ammo"; } }
}
