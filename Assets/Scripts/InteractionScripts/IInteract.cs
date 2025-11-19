/// <summary>
/// Two interfaces, one allows the system to know it can be interacted with
/// The other knows that it is an interactor, meaning it can interact with those objects
/// </summary>

public interface IInteract
{
    void Interact(IInteractor interactor);
}

/// <summary>
/// Currently the only getter is for an inventory
/// Others could be added
/// The inventory alone however gives a lot of options
/// For example, checking if the user has a key before it opens a door
/// </summary>
public interface IInteractor
{
    IInventory GetInventory { get; }
}
