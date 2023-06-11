using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.UI;

[Serializable]
public class CollectionSystem
{
    [SerializeField] private List<InventorySlot> inventorySlots;
    public List<InventorySlot> InventorySlots => inventorySlots;

    public int InventorySize => InventorySlots.Count;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public CollectionSystem()
    {
        inventorySlots = new List<InventorySlot>();
    }

    /// <summary>
    /// Add <paramref name="amountToAdd"/> instances of <paramref name="itemToAdd"/> and
    /// return the number of items added (should always be equal to <paramref name="amountToAdd"/>
    /// since Collections can have infinite stack sizes
    /// </summary>
    /// <param name="itemToAdd"></param>
    /// <param name="amountToAdd"></param>
    /// <returns> amount that was successfully added </returns>
    public int AddItemToCollection(InventoryItemData itemToAdd, int amountToAdd)
    {
        if (itemToAdd == null)
        {
            Debug.LogWarning($"Added item is null");
            return 0;
        }

        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.ItemData != itemToAdd) continue;

            slot.AddToStack(amountToAdd);
            return amountToAdd;
        }

        // If we get here, the current item is not yet in the collection, so add it
        inventorySlots.Add(new InventorySlot(itemToAdd, amountToAdd));

        return amountToAdd;
    }

    /// <summary>
    /// Remove <paramref name="amountToRemove"/> instances of <paramref name="itemToRemove"/>
    /// and return the number of successfully removed items
    /// </summary>
    /// <param name="itemToRemove"></param>
    /// <param name="amountToRemove"></param>
    /// /// <returns> amount that was successfully removed </returns>
    public int RemoveItemFromCollection(InventoryItemData itemToRemove, int amountToRemove)
    {
        if (itemToRemove == null)
        {
            Debug.LogWarning($"Removed item is null");
            return 0;
        }

        for (int i = 0; i < InventorySize; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if (slot.ItemData != itemToRemove) continue;

            int amountRemoved = Mathf.Min(slot.StackSize, amountToRemove);
            slot.RemoveFromStack(amountRemoved);

            if (slot.StackSize <= 0)
            {
                // since this slot is now empty, we can remove it from our inventory slots list
                inventorySlots.RemoveAt(i);
            }
            return amountRemoved;
        }

        // If we got here, the item doesn't exist in our collection
        return 0;
    }

    /// <summary>
    /// Get the amount of instances of <paramref name="checkedItem"/> in our Collection
    /// </summary>
    /// <param name="checkedItem"></param>
    /// <returns></returns>
    public int GetAmountOfItemInCollection(InventoryItemData checkedItem)
    {
        if (checkedItem == null)
        {
            Debug.LogWarning($"Checked item is null");
            return 0;
        }

        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.ItemData != checkedItem) continue;

            // If we get here, this stack has our checkedItem (should be the only one)
            return slot.StackSize;
        }

        // If we get here, no stack contains our checkedItem
        return 0;
    }

    public bool ContainsItem(InventoryItemData checkedItem)
    {
        if (checkedItem == null)
        {
            Debug.LogWarning($"Checked item is null");
            return false;
        }

        foreach (InventorySlot slot in InventorySlots)
        {
            if (slot.ItemData != checkedItem) continue;

            // If we get here, we found a slot with our item in its
            return true;
        }

        // if we get here, we never found our item in our slots
        return false;
    }
}
