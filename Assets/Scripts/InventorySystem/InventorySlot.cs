using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static Unity.VisualScripting.Member;

[System.Serializable]
public class InventorySlot : ISerializationCallbackReceiver
{
    [NonSerialized] private InventoryItemData itemData;    // Reference to the item's data
    [SerializeField] private int itemId = -1;   // inventory slot does not save a reference to the item, but rather just the index to that item in the InventoryItemDatabase object
    [SerializeField] private int stackSize; // How many of the item do we have in this slot

    public InventoryItemData ItemData => itemData;
    public int StackSize => stackSize;

    // Constructor to make the occupied inventory slot
    public InventorySlot(InventoryItemData source, int amount)
    {
        itemData = source;
        itemId = source.Id;
        stackSize = amount;
    }

    // Constructor to make an empty inventory slot
    public InventorySlot()
    {
        ClearSlot();
    }

    public void ClearSlot()
    {
        itemData = null;
        itemId = -1;
        stackSize = -1;
    }

    // Take the item from the input slot and place its information into the target slot
    public void AssignItem(InventorySlot inventorySlot)
    {
        // If the slots both have the same item, add the incoming stack to the target slot
        if (itemData == inventorySlot.itemData) AddToStack(inventorySlot.StackSize);
        else
        {
            // Otherwise, set the target slot information to the incoming slot information
            itemData = inventorySlot.itemData;
            itemId = itemData.Id;
            stackSize = 0;
            AddToStack(inventorySlot.StackSize);
        }
    }

    public void UpdateInventorySlot(InventoryItemData data, int amount)
    {
        itemData = data;
        itemId = data.Id;
        stackSize = amount;
    }

    public bool EnoughRoomLeftInStack(int amountToAdd, out int amountRemaining)
    {
        amountRemaining = ItemData.MaxStackSize - stackSize;
        return EnoughRoomLeftInStack(amountToAdd);
    }

    public bool EnoughRoomLeftInStack(int amountToAdd)
    {
        if (itemData == null) return true;
        if (stackSize + amountToAdd <= ItemData.MaxStackSize) return true;
        return false;
    }

    public void AddToStack(int amount)
    {
        stackSize += amount;
    }

    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;
        if (stackSize == 0) ClearSlot();
    }

    public bool SplitStack(out InventorySlot splitStack)
    {
        // Is there enough to split?
        if (StackSize <= 1)
        {
            splitStack = null;
            return false;
        }
        // Get half the stack amount
        int halfStack = Mathf.RoundToInt(StackSize/ 2);

        // Remove half the stack from the current slot
        RemoveFromStack(halfStack);

        // Make a new slot for the half stack slot
        splitStack = new InventorySlot(itemData, halfStack);
        return true;
    }

    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
        if (itemId == -1) return;

        // TODO Move to a GameManager that does this once rather than each time the game desrializes
        InventoryItemDatabase db = Resources.Load("InventoryItemDatabase") as InventoryItemDatabase;
        itemData = db.GetItem(itemId);
    }

    public bool HasItem()
    {
        return itemData != null;
    }
}
