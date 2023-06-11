using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

/// <summary>
/// This is an inventory display that can be used by one single inventory system,
///   The inventory system is owned by the inventoryHolder
/// </summary>

public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField] private InventoryHolder inventoryHolder;
    [SerializeField] private InventorySlot_UI[] slots;

    protected override void Start()
    {
        base.Start();

        RefreshStaticDisplay();
    }

    private void OnEnable()
    {
        PlayerInventoryHolder.OnPlayerHotbarChanged += RefreshStaticDisplay;
    }

    private void OnDisable()
    {
        PlayerInventoryHolder.OnPlayerHotbarChanged -= RefreshStaticDisplay;
    }

    public override void AssignSlot(InventorySystem inventoryToDisplay)
    {
        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();

        if (slots.Length != inventorySystem.InventorySize)
        {
            Debug.LogError($"Inventory slots out of sync on {gameObject}");
        }

        for (int i = 0; i < inventorySystem.InventorySize; i++)
        {
            SlotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
        }
    }

    public void RefreshStaticDisplay()
    {
        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.PrimaryInventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlots;
        }
        else
        {
            Debug.LogWarning($"No inventory assigned to {gameObject})");
        }

        for (int i = 0; i < inventorySystem.InventorySize; i++)
        {
            slots[i].ClearSlot();
        }

        AssignSlot(inventorySystem);
    }
}
