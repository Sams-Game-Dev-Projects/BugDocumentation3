using UnityEngine;
using System;

/// <summary>
/// The base class that all items are based on
/// Scriptable objects cannot be saved and loaded so a token is required
/// This scriptable object stores all of the non changing data such as the item's name
/// </summary>
public abstract class ItemBase : DatabaseElement
{
    public string itemName;                 //Item's name
    public Vector2Int itemSize;             //In the inventory grid, how large is this object
    public Sprite itemIcon;                 //What image is used to represent this object in the inventory
    public Mesh mesh;                       //When this object is in the world and can be collected, what mesh does it use
    public Material material;               //When this object is in the world and can be collected, what material does it use
    public GameObject itemBaseGameObject;   //In the event this item can be used, such as a weapon, what prefab does it use
    public bool isStackable;                //Is this item stackable
    public int stackMax;                    //What is the max stack that this item can include
    public Vector2Int startingStackSize;    //A vector2 that stores a min and max range of of many of these items are within a stack the player can find

    public abstract string GetItemType { get; }

    /// <summary>
    /// Pass in an item to create a token of that item
    /// If the item is stackable, randomly assign how many are in the stack, if it is not just assign 1
    /// </summary>
    public ItemToken GenerateToken(ItemBase item)
    {
        int amount = item.isStackable ? UnityEngine.Random.Range(item.startingStackSize.x, item.startingStackSize.y +1) : 1;

        return new ItemToken(this, amount);
    }
}

/// <summary>
/// Token of the scriptable object is needed for save and load
/// Sadly this token contains an action which might allow for save and load functions
/// In this current version of the project, save and load is not needed so it is not an issue
/// However in future iterations this would need to be corrected
/// </summary>
[Serializable]
public class ItemToken
{
    protected int _index;   //What index within the item database does the original scriptable object live
    protected int _amount;  //How many are within the current stack

    public Action<int> onAmountChanged; //Whenever the amount changes this event is called, being stored here as data makes this otherwise Serializable class no longer so

    //Returns the base item using the stored index to find it
    public ItemBase GetItemBase { get { return DatabaseContainer.Instance.itemDatabase.elements[_index] as ItemBase; } }
    //Returns the amount of this item in the stack
    public int GetAmount { get { return _amount; } }

    /// <summary>
    /// Adds an amount to the stack
    /// Each stack has a max value, if the added content makes the stack go above the allowed limit something should happen
    /// For example, adding a new item to the inventory based on the excess amount. In the event that the new excess item can not fit
    /// You could have the item thrown to the ground
    /// </summary>
    public bool AdjustAmount(int amount)
    {
        bool adjustedWithoutOverflow = true;

        if(GetItemBase.isStackable == false)
        {
            return adjustedWithoutOverflow;
        }

        _amount += amount;

        if (_amount > GetItemBase.stackMax)
        {
            Debug.Log("Too much ammo for one stack, do something else");
            adjustedWithoutOverflow = false;
        }

        _amount = Mathf.Clamp(_amount, 0, GetItemBase.stackMax);

        onAmountChanged?.Invoke(_amount);
        return adjustedWithoutOverflow;
    }

    /// <summary>
    /// A constructor for the item token
    /// The index is equal to that of the base item so it can be found in the database
    /// The amount passed in dictates how many are in the stack, a default value is used in the constructor
    /// To make sure that a logical amount is used if not specified
    /// </summary>
    public ItemToken(ItemBase item, int amount = 0)
    {
        _index = item.GetDatabaseIndex;
        _amount = amount;
    }
}
