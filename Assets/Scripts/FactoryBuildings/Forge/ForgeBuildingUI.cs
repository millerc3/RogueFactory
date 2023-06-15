using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeBuildingUI : InventoryDisplay
{
    private ForgeBuilding forge;
    public ForgeBuilding Forge => forge;

    [SerializeField] InventorySlot_UI inputUISlot;
    [SerializeField] InventorySlot_UI outputUISlot;

    public override void AssignSlot(InventorySystem inventoryToDisplay)
    {
        throw new System.NotImplementedException();
    }
}
