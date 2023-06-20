using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectionManager : MonoBehaviour
{
    public CollectionSystem Collection { get; protected set; }

    [SerializeField] protected InventoryItemDatabase itemDatabase;

    protected virtual void Awake()
    {
        Collection = new CollectionSystem();
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    public int AddItemToCollection(InventoryItemData itemToAdd, int amountToAdd)
    {
        return Collection.AddItemToCollection(itemToAdd, amountToAdd);
    }

    [Command]
    public int AddItemToCollection(int itemIdToAdd, int amountToAdd)
    {
        InventoryItemData itemToAdd = itemDatabase.GetItem(itemIdToAdd);

        if (itemToAdd == null)
        {
            Debug.LogWarning($"No item with ID {itemIdToAdd}");
            return 0;
        }

        return Collection.AddItemToCollection(itemToAdd, amountToAdd);
    }

    public int RemoveItemFromCollection(InventoryItemData itemToRemove, int amountToRemove)
    {
        return Collection.RemoveItemFromCollection(itemToRemove, amountToRemove);
    }

    [Command]
    public int RemoveItemFromCollection(int itemIdToRemove, int amountToRemove)
    {
        InventoryItemData itemToRemove = itemDatabase.GetItem(itemIdToRemove);

        if (itemToRemove == null)
        {
            Debug.LogWarning($"No item with ID {itemIdToRemove}");
            return 0;
        }

        return Collection.RemoveItemFromCollection(itemToRemove, amountToRemove);
    }
}

[Serializable]
public struct CollectionSaveData
{
    public CollectionSystem Collection;

    public CollectionSaveData(CollectionSystem collection)
    {
        Collection = collection;
    }
}