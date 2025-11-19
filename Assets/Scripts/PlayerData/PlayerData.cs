using UnityEngine;

/// <summary>
/// A script that stores all the player's content
/// At the moment it is just the inventory
/// In future however this could include other things such as where in the story the player is
/// If this was going to be able to be saved, a Serializable version of the player data would also need to be made
/// </summary>
public class PlayerData : MonoBehaviourSingleton<PlayerData>, IInventory
{
    [SerializeField] private Inventory _myInventory = new Inventory();

    public Inventory GetInventory { get { return _myInventory; } }
}
