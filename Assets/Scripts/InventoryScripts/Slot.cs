using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Slots store the image and text information needed to display inventories in game
/// They also must know what inventory they belong to and what item is in that inventory slot
/// </summary>
public class Slot : MonoBehaviour
{
    [SerializeField] private Image _slotIconImage;  //Image to be displayed
    [SerializeField] private TMP_Text _stackText;   //Text to be displayed

    private Vector2 _slotSize;              //How large is a slot? Needed to calculate how large text and images should be
    private Vector2Int _slotPosition;       //Where in the inventory is this slot
    private ItemToken _itemInSlot = null;   //What item is stored in this slot
    private Inventory _myInventory;         //What inventory does this slot belong to

    //Returns the slot position as a vector2
    public Vector2Int GetSlotPosition { get { return _slotPosition; } }
    //Returns the item in the slot
    public ItemToken GetItemInSlot { get { return _itemInSlot; } }

    //Returns a true or false value based on if the slot is empty
    public bool IsOccupied { get { return _itemInSlot != null; } }

    /// <summary>
    /// Sets up the slot to know all the base information such as the inventory it is connected to, the size, and the position of this slot
    /// </summary>
    public void InitSlot(Vector2Int slotPosition, Vector2 slotSize, Inventory inventory)
    {
        _slotPosition = slotPosition;
        _slotSize = slotSize;
        _myInventory = inventory;
    }

    /// <summary>
    /// Sets up the slot to know all the base information such as the inventory it is connected to, the size, and the position of this slot
    /// </summary>
    public void InitSlot(int x, int y, Vector2 slotSize, Inventory inventory)
    {
        _slotPosition = new Vector2Int(x, y);
        _slotSize = slotSize;
        _myInventory = inventory;
    }

    /// <summary>
    /// Sets the item within this slot to the passed in item
    /// UpdateUI must be true for the object to change in appearance
    /// Only the slot with the "root" position should change the image
    /// Otherwise a 2x2 image would have 4 images representing it instead of 1
    /// The default passed in is false as it is more common for the slot not to want visual alterations
    /// Regardless if the image is changed or not, the slot will subscribe to the item's "onAmountChanged" action
    /// That way, when the amount changes, the slot will update to represent that instead of needing to check every frame
    /// Using Update for that is a huge waste when it could just be controlled via an event
    /// </summary>
    public void AddItem(ItemToken newItem, bool updateUI = false)
    {
        _itemInSlot = newItem;

        if(updateUI == false)
        {
            return;
        }

        _itemInSlot.onAmountChanged -= OnStackAmountChanged;
        _itemInSlot.onAmountChanged += OnStackAmountChanged;
        OnStackAmountChanged(_itemInSlot.GetAmount);
    }

    /// <summary>
    /// When the amount of items in the stack changes we need to update the UI text to show the new amount
    /// This method should be subscribed to in the AddItem method above
    /// </summary>
    private void OnStackAmountChanged(int newAmount)
    {
        _stackText.text = newAmount.ToString();
    }

    /// <summary>
    /// This method changes the visuals of the slot
    /// 
    /// In the event there is no item in the slot it should set the size of the image and text to 0
    /// In addition to this, the text should be set to an empty string to make sure it does not display anything it shouldn't
    /// The image is also set to null so that the image sprite does not display inaccurate information
    /// Then return
    /// 
    /// In the event there is an item
    /// The image and text must have their size changed based on that items size and the size of the slot itself
    /// Make sure to set the image to the item's icon
    /// </summary>
    public void ChangeSlotUIData(ItemToken itemToUse)
    {
        if(itemToUse == null)
        {
            _slotIconImage.rectTransform.sizeDelta = Vector2.zero;
            _stackText.rectTransform.sizeDelta = Vector2.zero;
            _slotIconImage.sprite = null;
            return;
        }

        ItemBase itemBase = itemToUse.GetItemBase;

        Vector2 pixelSize = Vector2.Scale(_slotSize, itemBase.itemSize);
        _slotIconImage.rectTransform.sizeDelta = pixelSize;
        _stackText.rectTransform.sizeDelta = pixelSize;
        _slotIconImage.sprite = itemBase.itemIcon;
    }

    /// <summary>
    /// Whenever the slot is clicked on, this method should be run
    /// First we check if there is currently an item selected
    /// If we click on this space with an item selected a few things could happen
    /// For example, if the item is stackable, combine the items
    /// Or swap the items if the intended item fits in that space, minus the one item
    /// If the space is empty, just put the item into the free space
    /// 
    /// If there is no selected object, but the slot has an item in it
    /// Remove the item from the inventory and make this item the newley selected item
    /// If an item is removed, make sure to unsubscribe the stackAmountChanged method from the item's on amount changed event
    /// </summary>
    public void OnSlotClick()
    {
        if(InventoryUIController.currentlySelectedItem != null)
        {
            if(_myInventory.CanAddItem(_slotPosition, InventoryUIController.currentlySelectedItem, InventoryUIController.inventoryDictionary[_myInventory]))
            {
                InventoryUIController.currentlySelectedItem = null;
                return;
            }
        }

        if (_itemInSlot == null)
        {
            return;
        }

        InventoryUIController.currentlySelectedItem = _itemInSlot;
        _itemInSlot.onAmountChanged -= OnStackAmountChanged;
        ItemToken temp = _itemInSlot;
        ClearInventoryOfItem();
    }

    /// <summary>
    /// Sets the item in the inventory to null then sends a message to update the image and text
    /// </summary>
    public void RemoveItem()
    {
        if (_itemInSlot != null)
        {
            _itemInSlot.onAmountChanged -= OnStackAmountChanged;
        }
        _itemInSlot = null;
        ChangeSlotUIData(_itemInSlot);
    }

    /// <summary>
    /// Based on the item within the slot
    /// Start at the known home position of the item then use the items size to remove up to x and y spaces from the home position
    /// </summary>
    private void ClearInventoryOfItem()
    {
        _myInventory?.RemoveItem(InventoryUIController.inventoryDictionary[_myInventory], _itemInSlot.GetItemBase.itemSize, _slotPosition);
    }
}
