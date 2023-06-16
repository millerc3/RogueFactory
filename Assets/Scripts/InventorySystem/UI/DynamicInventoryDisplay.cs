using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an inventory display that can be used by various different inventory systems.
/// </summary>

public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField] protected InventorySlot_UI slotPrefab;

    public void RefreshDynamicInventory(InventorySystem inventorySystemToDisplay)
    {
        ClearSlots();
        inventorySystem = inventorySystemToDisplay;
        if (inventorySystem != null) inventorySystem.OnInventorySlotChanged += UpdateSlots;
        AssignSlot(InventorySystem);
    }

    private void OnDisable()
    {
        if (inventorySystem != null) inventorySystem.OnInventorySlotChanged -= UpdateSlots;
    }

    public override void AssignSlot(InventorySystem inventoryToDisplay)
    {
        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();

        if (inventoryToDisplay == null) return;

        for (int i = 0; i < inventoryToDisplay.InventorySize; i++)
        {
            var uiSlot = Instantiate(slotPrefab, transform);
            slotDictionary.Add(uiSlot, inventoryToDisplay.InventorySlots[i]);
            uiSlot.Init(inventoryToDisplay.InventorySlots[i]);
            uiSlot.UpdateUISlot();
        }
    }

    private void ClearSlots()
    {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }

        if (slotDictionary != null)
        {
            slotDictionary.Clear();
        }
    }
}
