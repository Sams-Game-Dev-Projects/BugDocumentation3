using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This script controls unique inventories
/// For example, if a desk had an inventory it would need a version of this
/// Despite all inventories needing a version of this, a static dictionary can be found to make life easier
/// Any list of slots can be found based on the passed in inventory
/// All elements that require knowledge of this dictionary have access to it through it being static
/// </summary>
public class InventoryUIController : MonoBehaviour
{
    public static ItemToken currentlySelectedItem;

    [SerializeField] private RectTransform _inventoryPanel;
    [SerializeField] private GameObject _slotObject;
    [SerializeField] public GameObject _inventoryContainer;
    [SerializeField] private Vector2 _slotSize;

    public static Dictionary<Inventory, List<Slot>> inventoryDictionary;

    private IInventory _inventory;

    /// <summary>
    /// When this object is turned on, get the inventory attached to this object
    /// If the inventory is already in the dictionary, return
    /// Otherwise, set up the inventory and add it
    /// </summary>
    private void OnEnable()
    {
        _inventory = _inventoryContainer.GetComponent<IInventory>();

        if (inventoryDictionary.ContainsKey(_inventory.GetInventory) == false)
        {
            return;
        }

        InitInventory(_inventory.GetInventory);
    }

    /// <summary>
    /// The passed in inventory is not currently in the dictionary that stores all inventories and slots
    /// cycle through all spaces of that inventory and make a new slot to represent that space
    /// Add it to a list
    /// Once all slots have been generated add the inventory and list to the dictionary
    /// </summary>
    private void InitInventory(Inventory inventory)
    {
        List<Slot> slots = new List<Slot>();

        for (int y = 0; y < inventory.GetinventorySize.y; y++)
        {
            for (int x = 0; x < inventory.GetinventorySize.x; x++)
            {
                GameObject go = ObjectPooling.Spawn(_slotObject, _inventoryPanel);

                Slot curSlot = go.GetComponent<Slot>();

                if(curSlot == null)
                {
                    curSlot = go.AddComponent<Slot>();
                }

                curSlot.InitSlot(y, x, _slotSize, inventory);
                slots.Add(curSlot);
            }
        }

        inventoryDictionary.Add(inventory, slots);
    }
}
