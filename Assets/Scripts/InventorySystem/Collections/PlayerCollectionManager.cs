using QFSW.QC;
using QFSW.QC.Suggestors.Tags;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using SaveLoadSystem;

public class PlayerCollectionManager : MonoBehaviour
{
    public CollectionSystem Collection { get; private set; }

    [SerializeField] private InventoryItemDatabase itemDatabase;

    private void Awake()
    {
        Collection = new CollectionSystem();
    }

    private void OnEnable()
    {
        SaveGameManager.PostLoadGameEvent += LoadCollection;
        SaveGameManager.PreSaveGameEvent += SaveCollection;
    }

    private void OnDisable()
    {
        SaveGameManager.PostLoadGameEvent += LoadCollection;
        SaveGameManager.PreSaveGameEvent -= SaveCollection;
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

    #region SAVE/LOAD SYSTEM

    public void LoadCollection(SaveData saveData)
    {
        Collection = saveData.PlayerCollectionData.Collection;
    }

    public void SaveCollection(SaveData saveData)
    {
        saveData.PlayerCollectionData.Collection = Collection;
    }

    #endregion
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