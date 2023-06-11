using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class InventoryHolder : MonoBehaviour
{
    [SerializeField] private int inventorySize;
    [SerializeField] protected InventorySystem primaryInventorySystem;

    public InventorySystem PrimaryInventorySystem => primaryInventorySystem;

    public static UnityAction<InventorySystem> OnDynamicInventoryDisplayRequested;
    public static UnityAction<InventorySystem> OnDynamicInventoryHideRequested;

    protected virtual void Awake()
    {
        primaryInventorySystem = new InventorySystem(inventorySize);
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {
        
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    public virtual bool IsEmpty()
    {
        return PrimaryInventorySystem.IsEmpty();
    }

    public virtual bool CanRemoveItems(out int numberOfItemsThatCanBeRemoved)
    {
        numberOfItemsThatCanBeRemoved = primaryInventorySystem.GetNumberOfFirstItem(out InventoryItemData firstItem);
        if (numberOfItemsThatCanBeRemoved == 0) return false;

        return true;
    }

    public virtual bool CanRemoveItems(int count)
    {
        int numberOfItemsThatCanBeRemoved = primaryInventorySystem.GetNumberOfFirstItem(out InventoryItemData _);
        return count <= numberOfItemsThatCanBeRemoved;
    }

    public virtual bool RemoveFromInventory(InventoryItemData item, int amount, out int numberUnableToRemove)
    {
        return primaryInventorySystem.RemoveItemFromInventory(item, amount, out numberUnableToRemove);
    }

    public virtual bool RemoveFromInventory(int amount, out InventoryItemData removedItem, out int numberOfItemsUnableToRemove)
    {
        numberOfItemsUnableToRemove = amount;
        int numberOfFirstItem = primaryInventorySystem.GetNumberOfFirstItem(out removedItem);
        if (numberOfFirstItem == 0) return false;

        return primaryInventorySystem.RemoveItemFromInventory(removedItem, amount, out numberOfItemsUnableToRemove);
    }

    public virtual bool CanAddItems(InventoryItemData itemToAdd, int amountToAdd, out int numberUnableToAdd)
    {
        return primaryInventorySystem.CanFitItem(itemToAdd, amountToAdd, out numberUnableToAdd);
    }

    public virtual bool AddToInventory(InventoryItemData data, int amount, out int numberUnableToAdd)
    {
        return primaryInventorySystem.AddItemToInventory(data, amount, out numberUnableToAdd);
    }

    public virtual void SetInventory(InventorySystem newSystem)
    {
        primaryInventorySystem = newSystem;
    }

    protected abstract void LoadInventory(SaveData saveData);
}

[System.Serializable]
public struct InventoryHolderSaveData
{
    public InventorySystem InventorySystem;
    public Vector3 Position;
    public Quaternion Rotation;

    public InventoryHolderSaveData(InventorySystem inventorySystem, Vector3 position, Quaternion rotation)
    {
        InventorySystem = inventorySystem;
        Position = position;
        Rotation = rotation;
    }

    public InventoryHolderSaveData(InventorySystem inventorySystem)
    {
        InventorySystem = inventorySystem;
        Position = Vector3.zero;
        Rotation = Quaternion.identity;
    }
}