using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBoxInventory : InventoryHolder
{
    // TODO remove the derivation from InventoryHolder and just make a new class?

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    #region SaveSystem
    protected override void LoadInventory(SaveData saveData)
    {
        // Handled in StorageBoxBuilding
    }
    #endregion
}
