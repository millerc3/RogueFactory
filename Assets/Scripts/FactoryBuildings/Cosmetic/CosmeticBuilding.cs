using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticBuilding : FactoryBuilding
{
    public override int AddToInput(InventoryItemData item, int count, GameObject worldObject)
    {
        throw new System.NotImplementedException();
    }

    public override bool CanAddToInput(InventoryItemData item, int count, out int AmountOfItemsThatCanBeAdded)
    {
        AmountOfItemsThatCanBeAdded = 0;
        return false;
    }

    public override bool CanRemoveFromOutput(out int amountAbleToRemove)
    {
        amountAbleToRemove = 0;
        return false;
    }

    public override void LoadData(SaveData saveData)
    {

    }

    public override void LocalUpdate(int depth)
    {

    }

    public override int RemoveFromOutput(int count, out InventoryItemData removedItem, out GameObject worldObject)
    {
        throw new System.NotImplementedException();
    }

    protected override void SaveData(SaveData saveData)
    {

    }
}
