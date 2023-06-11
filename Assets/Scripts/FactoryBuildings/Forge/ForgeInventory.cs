using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ForgeInventory : MonoBehaviour
{
    [SerializeField] private int inputInventorySize;
    private InventorySystem inputInventorySystem;
    [SerializeField] private int outputInventorySize;
    private InventorySystem outputInventorySystem;

    public InventorySystem InputInventory => inputInventorySystem;
    public InventorySystem OutputInventory => outputInventorySystem;

    public static UnityAction<ForgeInventory> OnForgeInventoryDisplayRequested;
    public static UnityAction<ForgeInventory> OnForgeInventoryHideRequested;

    private void Awake()
    {
        inputInventorySystem = new InventorySystem(inputInventorySize);
        outputInventorySystem = new InventorySystem(outputInventorySize);
    }

    public bool AddToInputInventory(InventoryItemData itemToAdd, int amountToAdd, out int numberUnableToAdd)
    {
        return inputInventorySystem.AddItemToInventory(itemToAdd, amountToAdd, out numberUnableToAdd);
    }

    public bool CanAddItemsToInput(InventoryItemData itemToAdd, int amountToAdd, out int numberUnableToAdd)
    {
        return inputInventorySystem.CanFitItem(itemToAdd, amountToAdd, out numberUnableToAdd);
    }

    public bool CanRemoveFromOutput(out int numberOfItemsThatCanBeRemoved)
    {
        numberOfItemsThatCanBeRemoved = outputInventorySystem.GetNumberOfFirstItem(out InventoryItemData firstItem);
        if (numberOfItemsThatCanBeRemoved == 0) return false;

        return true;
    }

    public bool RemoveFromOutputInventory(int amount, out InventoryItemData removedItem, out int numberOfItemsUnableToRemove)
    {
        numberOfItemsUnableToRemove = amount;
        int numberOfFirstItem = outputInventorySystem.GetNumberOfFirstItem(out removedItem);
        if (numberOfFirstItem == 0) return false;

        return outputInventorySystem.RemoveItemFromInventory(removedItem, amount, out numberOfItemsUnableToRemove);

    }

    public void SetInputInventory(InventorySystem newInventorySystem)
    {
        inputInventorySystem = newInventorySystem;
    }

    public void SetOutputInventory(InventorySystem newInventorySystem)
    {
        outputInventorySystem = newInventorySystem;
    }
}
