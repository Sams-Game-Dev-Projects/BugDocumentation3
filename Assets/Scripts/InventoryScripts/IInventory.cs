/// <summary>
/// An inventory to determine what objects have an inventory
/// The current game only has the player with an inventory
/// However, other objects in future such as a desk could have an inventory
/// </summary>

public interface IInventory
{
    Inventory GetInventory { get; }
}