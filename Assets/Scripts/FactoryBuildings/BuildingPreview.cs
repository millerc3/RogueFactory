using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : FactoryBuilding
{
    public override int AddToInput(InventoryItemData item, int count, GameObject worldObject)
    {
        throw new System.NotImplementedException();
    }

    public override bool CanAddToInput(InventoryItemData item, int count, out int AmountOfItemsThatCanBeAdded)
    {
        throw new System.NotImplementedException();
    }

    public override bool CanRemoveFromOutput(out int count)
    {
        throw new System.NotImplementedException();
    }

    public override void CheckNeighbors()
    {
        throw new System.NotImplementedException();
    }

    public override void LoadData(SaveData saveData)
    {
        throw new System.NotImplementedException();
    }

    public override void LocalUpdate(int depth)
    {
        throw new System.NotImplementedException();
    }

    public override int RemoveFromOutput(int count, out InventoryItemData removedItem, out GameObject worldObject)
    {
        throw new System.NotImplementedException();
    }

    protected override void SaveData(SaveData saveData)
    {
        throw new System.NotImplementedException();
    }
}
