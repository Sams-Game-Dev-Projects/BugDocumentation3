using UnityEngine;

/// <summary>
/// A script that is for interactable items that can be picked up
/// </summary>
[RequireComponent(typeof(MeshFilter), (typeof(MeshRenderer)), (typeof(MeshCollider)))]
public class ItemPickup : MonoBehaviour, IInteract
{
    //What is the item being collected?
    [SerializeField] private ItemBase _itemBaseToCollect;

    //A generated token based on the item base
    //This is needed because the inventory does not contain items, but item tokens
    private ItemToken _itemToken;

    private void OnEnable()
    {
        ChangeMesh();
    }

    /// <summary>
    /// Make this object look like the object it is meant to be representing
    /// All items have a field for their mesh and material
    /// </summary>
    private void ChangeMesh()
    {
        GetComponent<MeshFilter>().mesh = _itemBaseToCollect.mesh;
        GetComponent<MeshCollider>().sharedMesh = _itemBaseToCollect.mesh;
        GetComponent<MeshRenderer>().material = _itemBaseToCollect.material;
    }

    /// <summary>
    /// If this object is spawned and is meant to represent a specific item
    /// It can be told what token it is meant to have
    /// For example, ammo when collected gives a random number of ammo
    /// However, ammo dropped from the inventory has a known quantity
    /// It would be odd to drop 5 bullets and then pick up the same thing you dropped to find you now have 7
    /// </summary>
    public void SetItemToken(ItemToken newToken)
    {
        _itemToken = newToken;
        ChangeMesh();
    }

    /// <summary>
    /// An interactor is passed in so that needed data can be observed
    /// In this case, the inventory of the interactor is needed
    /// Once the item is colleged it needs to go inside that inventory
    /// 
    /// Assuming the inventory exists and the token exists
    /// Put the item into the inventory, then despawn this object so that it cannot be picked up again
    /// </summary>
    public void Interact(IInteractor interactor)
    {
        if (interactor.GetInventory == null || _itemBaseToCollect == null)
        {
            return;
        }

        if(_itemToken == null)
        {
            _itemToken = _itemBaseToCollect.GenerateToken(_itemBaseToCollect);
        }

        IInventory inventory = interactor.GetInventory;
        inventory.GetInventory.CanAddItem(_itemToken, InventoryUIController.inventoryDictionary[inventory.GetInventory]);
        ObjectPooling.Despawn(gameObject);
    }
}