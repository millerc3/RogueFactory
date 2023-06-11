using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorBuilding : FactoryBuilding
{
    private PlayerCollectionManager playerCollectionManager;


    #region MonoBehavior

    protected override void Start()
    {
        base.Start();

        playerCollectionManager = FindObjectOfType<PlayerCollectionManager>();

        LocalUpdate(0);
    }

    #endregion

    #region Collector

    #endregion

    #region Factory Building

    public override void LocalUpdate(int depth)
    {
        if (depth > 1) return;

        CheckNeighbors();
        UpdateNeighbors(++depth);
    }

    public override int AddToInput(InventoryItemData item, int count, GameObject worldObject)
    {
        Destroy(worldObject);
        playerCollectionManager.AddItemToCollection(item, count);
        return count;
    }

    public override bool CanAddToInput(InventoryItemData item, int count, out int AmountOfItemsThatCanBeAdded)
    {
        AmountOfItemsThatCanBeAdded = count;
        return true;
    }

    public override int RemoveFromOutput(int count, out InventoryItemData removedItem, out GameObject worldObject)
    {
        removedItem = null;
        worldObject = null;
        return 0;
    }

    public override bool CanRemoveFromOutput(out int amountAbleToRemove)
    {
        amountAbleToRemove = 0;
        return false;
    }

    #endregion

    #region Interaction System

    #endregion

    #region Save/Load System

    public override void LoadData(SaveData saveData)
    {

    }

    protected override void SaveData(SaveData saveData)
    {

    }

    #endregion
}
