using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class InventorySystem
{
    [SerializeField] private List<InventorySlot> inventorySlots;
    public List<InventorySlot> InventorySlots => inventorySlots;
    public int InventorySize => InventorySlots.Count;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public InventorySystem(int size)
    {
        inventorySlots = new List<InventorySlot>();
        for (int i = 0; i < size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    /// <summary>
    /// Add <paramref name="amountToAdd"/> instances of <paramref name="itemToAdd"/>, and set <paramref name="numberUnableToAdd"/> to the
    /// amount of items that couldn't fit
    /// </summary>
    /// <param name="itemToAdd"></param>
    /// <param name="amountToAdd"></param>
    /// <param name="numberUnableToAdd"></param>
    /// <returns></returns>
    public bool AddItemToInventory(InventoryItemData itemToAdd, int amountToAdd, out int numberUnableToAdd)
    {
        if (CanFitItem(itemToAdd, out List<InventorySlot> freeSlots, out int numberOfItemsThatCanFit))
        {
            //numberUnableToAdd = amountToAdd - numberOfItemsThatCanFit;
            int amountLeftToAdd = amountToAdd;

            foreach (InventorySlot slot in freeSlots)
            {
                if (amountLeftToAdd == 0) break;

                if (slot.ItemData == null) slot.UpdateInventorySlot(itemToAdd, 0);

                int freeSpaceInSlot = (slot.ItemData != null) ? itemToAdd.MaxStackSize - slot.StackSize : itemToAdd.MaxStackSize;
                if (freeSpaceInSlot >= amountLeftToAdd)
                {
                    slot.AddToStack(amountLeftToAdd);
                    amountLeftToAdd = 0;
                }
                else
                {
                    slot.AddToStack(freeSpaceInSlot);
                    amountLeftToAdd -= freeSpaceInSlot;
                }
                OnInventorySlotChanged?.Invoke(slot);
            }
            numberUnableToAdd = amountLeftToAdd;
            return true;
        }

        numberUnableToAdd = amountToAdd;
        return false;
    }

    /// <summary>
    /// Remove <paramref name="amountToRemove"/> instances of <paramref name="itemToRemove"/> from the inventory; update
    /// <paramref name="numberOfItemsUnableToRemove"/> with the number of instances that could not be removed
    /// </summary>
    /// <param name="itemToRemove"></param>
    /// <param name="amountToRemove"></param>
    /// <param name="numberOfItemsUnableToRemove"></param>
    /// <returns></returns>
    public bool RemoveItemFromInventory(InventoryItemData itemToRemove, int amountToRemove, out int numberOfItemsUnableToRemove)
    {
        numberOfItemsUnableToRemove = amountToRemove;
        if (!ContainsItem(itemToRemove, out List<InventorySlot> slotsWithTargetItem)) return false;

        int amountLeftToRemove = amountToRemove;
        foreach (InventorySlot slot in slotsWithTargetItem)
        {
            if (amountLeftToRemove == 0) break;

            // If there are more items in this slot than we need to remove
            if (slot.StackSize >= amountLeftToRemove)
            {
                // remove all the necessary amount left
                slot.RemoveFromStack(amountToRemove);
                // set amount left to 0
                amountLeftToRemove = 0;
            }
            else // If there are fewer items in this slot than we need to remove
            {
                // remove as much as we can and move to the next slot
                amountLeftToRemove -= slot.StackSize;
                slot.ClearSlot();
            }
            OnInventorySlotChanged?.Invoke(slot);
        }

        if (amountLeftToRemove == 0)
        {
            numberOfItemsUnableToRemove = 0;
        }
        else
        {
            numberOfItemsUnableToRemove = amountToRemove - amountLeftToRemove;
        }
        
        return amountLeftToRemove < amountToRemove;
    }

    /// <summary>
    /// Get the number of times <paramref name="item"/> appears in the inventory
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int GetAmountOfItemInInventory(InventoryItemData item)
    {
        int numerOfItems = 0;
        List<InventorySlot> containedSlots = InventorySlots.Where(i => i.ItemData == item).ToList();
        foreach (InventorySlot slot in containedSlots)
        {
            numerOfItems += slot.StackSize;
        }

        return numerOfItems;
    }

    // Do any of our slots have this item in them? If so, give the list of all slots with the item in it

    /// <summary>
    /// Does this inventory contain <paramref name="item"/>; Update <paramref name="invSlots"/> with the slots that contain
    /// <paramref name="item"/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="invSlots"></param>
    /// <returns></returns>
    public bool ContainsItem(InventoryItemData item, out List<InventorySlot> invSlots)
    {
        invSlots = InventorySlots.Where(i => i.ItemData == item).ToList();

        return invSlots.Count > 0;
    }

    /// <summary>
    /// Does this inventory contain <paramref name="item"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool ContainsItem(InventoryItemData item)
    {
        return InventorySlots.Where(i => i.ItemData == item).Count() > 0;
    }

    /// <summary>
    /// Does this inventory have a free slot; update <paramref name="freeSlot"/> with the first open slot
    /// </summary>
    /// <param name="freeSlot"></param>
    /// <returns></returns>
    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        // Get the first free slot
        freeSlot = InventorySlots.FirstOrDefault(i => i.ItemData == null);
        return freeSlot != null;
    }

    /// <summary>
    /// Can the inventory fit <paramref name="numberOfItemsThatCanFit"/>? If so, <paramref name="freeSlots"/> will be set to a list
    /// of all the inventory slots that the item can go into, and <paramref name="numberOfItemsThatCanFit"/> will store how many of 
    /// <paramref name="itemToFit"/> that can fit in the inventory.
    /// <param name="itemToFit"></param>
    /// <param name="freeSlots"></param>
    /// <param name="numberOfItemsThatCanFit"></param>
    /// <returns></returns>
    public bool CanFitItem(InventoryItemData itemToFit, out List<InventorySlot> freeSlots, out int numberOfItemsThatCanFit)
    {
        if (itemToFit == null) Debug.LogError("There is no itemToFit!");

        freeSlots = InventorySlots.Where(i => i.ItemData == null || (i.ItemData == itemToFit && i.StackSize < i.ItemData.MaxStackSize)).ToList();

        numberOfItemsThatCanFit = 0;
        foreach (InventorySlot slot in freeSlots)
        {
            if (slot.ItemData == null)
            {
                numberOfItemsThatCanFit += itemToFit.MaxStackSize;
            }
            else
            {
                numberOfItemsThatCanFit += itemToFit.MaxStackSize - slot.StackSize;
            }
        }   

        return numberOfItemsThatCanFit > 0;
    }

    /// <summary>
    /// Can the inventory fit <paramref name="amountToFit"/> instances of <paramref name="itemToFit"/>? Update
    /// <paramref name="numberUnableToFit"/> with the amount of items that cannot fit into the inventory
    /// </summary>
    /// <param name="itemToFit"></param>
    /// <param name="amountToFit"></param>
    /// <param name="numberUnableToFit"></param>
    /// <returns></returns>
    public bool CanFitItem(InventoryItemData itemToFit, int amountToFit, out int numberUnableToFit)
    {
        bool ret = CanFitItem(itemToFit, out List<InventorySlot> freeSlots, out int numberThatCanFit);

        // If the itemToFit CANNOT fit
        if (!ret)
        {
            numberUnableToFit = amountToFit;
            return false;
        }

        // If the itemToFit CAN fit
        numberUnableToFit = amountToFit - numberThatCanFit;
        return true;
    }

    /// <summary>
    /// Get the total number of instances of the first item in the inventory, and set
    /// <paramref name="firstItem"/> to the first InventoryItemData found
    /// </summary>
    /// <returns></returns>
    public int GetNumberOfFirstItem(out InventoryItemData firstItem)
    {
        int totalItems = 0;
        firstItem = null;
        InventorySlot firstSlot = InventorySlots.FirstOrDefault(i => i.ItemData != null);

        if (firstSlot == null) return totalItems;

        firstItem = firstSlot.ItemData;
        List<InventorySlot> matchingSlots = InventorySlots.Where(i => i.ItemData == firstSlot.ItemData).ToList();

        foreach (InventorySlot slot in matchingSlots)
        {
            totalItems += slot.StackSize;
        }

        return totalItems;
    }

    /// <summary>
    /// Is this inventory completely empty?
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        var emptySlots = InventorySlots.Where(i => i.ItemData == null).ToList();
        if (emptySlots.Count == inventorySlots.Count)
        {
            return true;
        }
        return false;
    }

    public void ClearInventory()
    {
        foreach (InventorySlot slot in InventorySlots)
        {
            slot.ClearSlot();
            OnInventorySlotChanged?.Invoke(slot);
        }
    }
}
