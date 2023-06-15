using SaveLoadSystem;
using System;
using UnityEngine;

[RequireComponent(typeof(StorageBoxInventory))]
public class StorageBoxBuilding : FactoryBuilding
{
    private StorageBoxInventory inventory;
    private StorageBoxSaveData storageBoxSaveData;

    protected override void Awake()
    {
        base.Awake();

        inventory = GetComponent<StorageBoxInventory>();
        storageBoxSaveData = new StorageBoxSaveData(inventory.PrimaryInventorySystem);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void Start()
    {
        base.Start();

        LocalUpdate(0);
    }

    protected override void Update()
    {
        base.Update();

        if (interactorTransform != null && !Interactor.InRangeOfInteractor(interactorTransform, transform, interactionRange))
        {
            InventoryHolder.OnDynamicInventoryHideRequested?.Invoke(inventory.PrimaryInventorySystem);
            interactorTransform = null;
        }
    }

    protected override void TearDown()
    {
        foreach (InventorySlot slot in inventory.PrimaryInventorySystem.InventorySlots)
        {
            if (slot.ItemData == null || slot.StackSize <= 0) continue;

            // TODO Put numberUnableToAdd onto the ground or something
            playerInventoryHolder.AddToInventory(slot.ItemData, slot.StackSize, out int numberUnableToAdd);
        }

        base.TearDown();
    }

    public override int AddToInput(InventoryItemData item, int count, GameObject worldObject)
    {
        inventory.AddToInventory(item, count, out int numberUnableToAdd);

        Destroy(worldObject);

        return count - numberUnableToAdd;
    }

    public override bool CanAddToInput(InventoryItemData item, int count, out int AmountOfItemsThatCanBeAdded)
    {
        AmountOfItemsThatCanBeAdded = 0;
        bool ret = inventory.CanAddItems(item, count, out int numberUnableToAdd);
        if (ret)
        {
            AmountOfItemsThatCanBeAdded = count - numberUnableToAdd;
        }

        return ret;
    }

    public override bool CanRemoveFromOutput(out int amountAbleToRemove)
    {
        return inventory.CanRemoveItems(out amountAbleToRemove);
    }

    public override void LocalUpdate(int depth)
    {
        if (depth > 1) return;

        CheckNeighbors();
        UpdateNeighbors(++depth);
    }

    public override int RemoveFromOutput(int count, out InventoryItemData removedItem, out GameObject worldObject)
    {
        worldObject = null;
        bool successfullyRemoved = inventory.RemoveFromInventory(count, out removedItem, out int numberOfItemsUnableToRemove);
        //print($"sucessfully removed {count - numberOfItemsUnableToRemove} of {removedItem.Name}");

        return count - numberOfItemsUnableToRemove;
    }

    protected override void OnTapInteract()
    {
        base.OnTapInteract();

        InventoryHolder.OnDynamicInventoryDisplayRequested?.Invoke(inventory.PrimaryInventorySystem);
    }

    public override void LoadData(SaveData saveData)
    {
        if (saveData.StorageBoxSaveDictionary.TryGetValue(Origin, out StorageBoxSaveData storedSaveData))
        {
            inventory.SetInventory(storedSaveData.InventorySystem);
        }
    }

    protected override void SaveData(SaveData saveData)
    {
        if (saveData.StorageBoxSaveDictionary.TryGetValue(Origin, out StorageBoxSaveData storedSaveData))
        {
            storedSaveData.InventorySystem = inventory.PrimaryInventorySystem;
        }
        else
        {
            saveData.StorageBoxSaveDictionary.TryAdd(Origin, storageBoxSaveData);
        }
    }
}

[Serializable]
public struct StorageBoxSaveData
{
    public InventorySystem InventorySystem;

    public StorageBoxSaveData(InventorySystem inventorySystem)
    {
        InventorySystem = inventorySystem;
    }
}