using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An inventory of items
/// Each item is stored on a grid and takes up x and y space within the grid
/// </summary>

[System.Serializable]
public class Inventory
{

    /// <summary>
    /// How large is the inventory?
    /// </summary>
    [SerializeField] private int _inventorySizeX;
    [SerializeField] private int _inventorySizeY;

    //What items are within the inventory?
    private List<ItemToken> _itemsInInventory = new List<ItemToken>();

    /// <summary>
    /// Returns a vertor2 based on the size of the inventory
    /// A vector2 cannot be a member variable if we intend to save and load this data which is why it is a property instead
    /// </summary>
    public Vector2Int GetinventorySize { get { return new Vector2Int(_inventorySizeX, _inventorySizeY); } }

    /// <summary>
    /// Calculates the index needed based on the x and y position
    /// Always draw out the grid and determine if this is correct
    /// Using a flat 2D array can be confusing so make sure it is accurate
    /// </summary>
    public int CalculateIndex(int x, int y)
    {
        return y * _inventorySizeX + x;
    }

    /// <summary>
    /// Calculates the index needed based on the x and y position
    /// Always draw out the grid and determine if this is correct
    /// Using a flat 2D array can be confusing so make sure it is accurate
    /// </summary>
    public int CalculateIndex(Vector2Int position)
    {
        return position.y * _inventorySizeX + position.x;
    }

    /// <summary>
    /// Loops through all stored items looking for ones of a specific type
    /// It will return the list based on all the ones found of that type
    /// It the type matches, it is added to a list that is returned
    /// </summary>
    public List<ItemToken> GetAllItemsOfType(string itemType)
    {
        List<ItemToken> itemsFound = new List<ItemToken>();

        foreach (ItemToken item in _itemsInInventory)
        {
            if (string.Equals(item.GetItemBase.GetItemType, itemType, System.StringComparison.OrdinalIgnoreCase))
            {
                itemsFound.Add(item);
            }
        }

        return itemsFound;
    }

    /// <summary>
    /// Checks if the passed in x and y position is out of bounds of the inventory grid
    /// If either x or y is below 0 or above the length range it will throw an error which is why this is needed
    /// </summary>
    private bool IsOutOfBounds(int x, int y)
    {
        if (x >= _inventorySizeX || y >= _inventorySizeY || x < 0 || y < 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Loops through all grid spaces
    /// Finds the first empty space, or the first full space that contains the same item if it is stackable
    /// In the event the item is stackable it can add its amount to the stack that already exists
    /// Otherwise, it needs to find an empty space, once one is found. It will check the neighbours to make sure they are also empty
    /// If all neighbours are empty, the item is save to put into the inventory at X position
    /// 
    /// Slots are not of a serializable type which is why they must be send into the inventory from elsewhere
    /// </summary>
    public bool CanAddItem(ItemToken itemToAdd, List<Slot> slots)
    {
        if (itemToAdd == null)
        {
            return false;
        }

        ItemBase itemBase = itemToAdd.GetItemBase;

        for (int y = 0; y < _inventorySizeY; y++)
        {
            for (int x = 0; x < _inventorySizeX; x++)
            {
                if (slots[CalculateIndex(x, y)].IsOccupied == true)
                {
                    if (itemBase.isStackable == true
                        && itemBase == slots[CalculateIndex(x, y)].GetItemInSlot.GetItemBase
                        && slots[CalculateIndex(x, y)].GetItemInSlot.GetAmount < itemBase.stackMax)
                    {
                        //The space is full, but is of a stackable type that matches the requested object
                        //Combine the amounts then return true (it is true that the item was added)
                        slots[CalculateIndex(x, y)].GetItemInSlot.AdjustAmount(itemToAdd.GetAmount);
                        return true;
                    }
                    continue;
                }

                Vector2Int position = new Vector2Int(x, y);

                if (CheckNeighboursAreEmpty(slots, position, itemBase) == false)
                {
                    continue;
                }

                //This space is useable, add the item into this space
                AddItem(slots, itemToAdd, position);
                return true;
            }
        }

        //If it is false that the item was added a message will be sent back
        //Ideally this would make an object be thrown away from the player
        //And an audio source would play something like "I am carrying too much stuff"
        return false;
    }

    /// <summary>
    /// Similar to the above method
    /// However, instead of iterating through all spaces, this checks a specific space instead
    /// </summary>
    public bool CanAddItem(Vector2Int position, ItemToken itemToAdd, List<Slot> slots)
    {
        if (itemToAdd == null)
        {
            return false;
        }

        ItemBase itemBase = itemToAdd.GetItemBase;

        if (CheckNeighboursAreEmpty(slots, position, itemBase) == false)
        {
            //The spaces are full, therefore return false (we did not add the item)
            return false;
        }

        //The spaces are empty, therefore we can return true that the item was added
        AddItem(slots, itemToAdd, position);
        return true;
    }

    /// <summary>
    /// Called once the item has been determined to fit within its desired location
    /// First add the item to the "main" slot so that the image can be updated as needed
    /// Then loop through all remaining slots based on the size of the object and inform them that the item within them is this item
    /// </summary>
    private void AddItem(List<Slot> slots, ItemToken itemToAdd, Vector2Int startingPosition)
    {
        ItemBase itemBase = itemToAdd.GetItemBase;

        _itemsInInventory.Add(itemToAdd);
        slots[CalculateIndex(startingPosition)].ChangeSlotUIData(itemToAdd);
        slots[CalculateIndex(startingPosition)].AddItem(itemToAdd, true);

        for (int y = startingPosition.y; y < startingPosition.y + itemBase.itemSize.y; y++)
        {
            for (int x = startingPosition.x; x < startingPosition.x + itemBase.itemSize.x; x++)
            {
                if (x == startingPosition.x && y == startingPosition.y)
                {
                    continue;
                }
                slots[CalculateIndex(x, y)].AddItem(itemToAdd);
            }
        }
    }

    /// <summary>
    /// Loop through all spaces starting from the start position up until the start position plus the size of the object
    /// If all those spaces are empty, this item can fit
    /// If any of those spaces are full, this item cannot fit
    /// Likewise, if the search goes out of bounds the item cannot fit
    /// </summary>
    private bool CheckNeighboursAreEmpty(List<Slot> slots, Vector2Int startingPosition, ItemBase itemToAdd)
    {
        for (int y = startingPosition.y; y < startingPosition.y + itemToAdd.itemSize.y; y++)
        {
            for (int x = startingPosition.x; x < startingPosition.x + itemToAdd.itemSize.x; x++)
            {
                if (IsOutOfBounds(x, y) == true)
                {
                    return false;
                }

                if (slots[CalculateIndex(x, y)].IsOccupied == true)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// The item is being removed
    /// Loop through all spaces starting from the start position up until the start position plus the size of the object
    /// Each of those slots are sent a message that the space needs to be cleared
    /// Optionally remove the token from the inventory list
    /// </summary>
    public void RemoveItem(List<Slot> slots, Vector2Int sizeToClear, Vector2Int startingPosition, ItemToken itemToRemove = null)
    {
        for (int y = startingPosition.y; y < startingPosition.y + sizeToClear.y; y++)
        {
            for (int x = startingPosition.x; x < startingPosition.x + sizeToClear.x; x++)
            {
                slots[CalculateIndex(x, y)].RemoveItem();
            }
        }

        if (itemToRemove != null)
        {
            _itemsInInventory.Remove(itemToRemove);
        }
    }
}
