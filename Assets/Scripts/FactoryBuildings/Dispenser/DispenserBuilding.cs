using SaveLoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenserBuilding : FactoryBuilding
{
    private PlayerCollectionManager playerCollectionManager;

    [SerializeField] private InventoryItemData itemToDispense;
    public InventoryItemData ItemToDispense => itemToDispense;

    [SerializeField] private InventoryItemDatabase itemDatabase;

    [SerializeField] private DispenserUI sharedDispenserUI;

    private DispenserSaveData dispenserSaveData;

    #region MonoBehaviour

    protected override void Awake()
    {
        base.Awake();

        sharedDispenserUI = FindObjectOfType<DispenserUI>(true);

        dispenserSaveData = new DispenserSaveData(-1);
    }

    protected override void Start()
    {
        base.Start();

        playerCollectionManager = FindObjectOfType<PlayerCollectionManager>();

        LocalUpdate(0);
    }

    protected override void Update()
    {
        base.Update();

        if (interactorTransform != null)
        {
            if (!InRangeOfInteractor())
            {
                HideUI();
            }
        }
    }



    #endregion

    #region Dispenser

    public void SetItemToDispense(InventoryItemData item)
    {
        itemToDispense = item;
    }

    private void ShowUI()
    {
        sharedDispenserUI.Setup(this);
        sharedDispenserUI.gameObject.SetActive(true);
    }

    private void HideUI()
    {
        if (sharedDispenserUI.Dispenser == this)
        {
            sharedDispenserUI.gameObject.SetActive(false);
        }
    }

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
        return 0;
    }

    public override bool CanAddToInput(InventoryItemData item, int count, out int AmountOfItemsThatCanBeAdded)
    {
        AmountOfItemsThatCanBeAdded = 0;
        return false;
    }

    public override bool CanRemoveFromOutput(out int amountAbleToRemove)
    {
        amountAbleToRemove = 0;
        if (itemToDispense == null) return false;

        if (playerCollectionManager.Collection.ContainsItem(itemToDispense))
        {
            amountAbleToRemove = playerCollectionManager.Collection.GetAmountOfItemInCollection(itemToDispense);
            return true;
        }

        return false;
    }

    public override int RemoveFromOutput(int count, out InventoryItemData removedItem, out GameObject worldObject)
    {
        worldObject = null;
        removedItem = null;

        if (itemToDispense == null) return 0;

        int amountRemoved = 0;

        int amountInCollection = playerCollectionManager.Collection.GetAmountOfItemInCollection(itemToDispense);
        // If we have this item in the collection
        if (amountInCollection > 0)
        {
            // set the remoevd item to the item were currently dispensing
            removedItem = itemToDispense;
            // return whatever is lower, the count requested, or the amount of the items that are have left
            //   in this collection
            amountRemoved = Mathf.Min(amountInCollection, count);

            // remove that amount from the collection
            playerCollectionManager.RemoveItemFromCollection(itemToDispense, amountRemoved);
        }

        return amountRemoved;

    }

    #endregion

    #region Interaction System

    protected override void OnTapInteract()
    {
        base.OnTapInteract();

        print("Ontapinteract");
        ShowUI();
    }

    #endregion

    #region Save/Load System
    public override void LoadData(SaveData saveData)
    {
        if (saveData.DispenserSaveDictionary.TryGetValue(Origin, out DispenserSaveData storedSaveData))
        {
            itemToDispense = itemDatabase.GetItem(storedSaveData.ItemToDispenseId);
        }
    }

    protected override void SaveData(SaveData saveData)
    {
        if (itemToDispense == null) return;

        if (saveData.DispenserSaveDictionary.TryGetValue(Origin, out DispenserSaveData storedSaveData))
        {
            storedSaveData.ItemToDispenseId = itemToDispense.Id;
        }
        else
        {
            dispenserSaveData.ItemToDispenseId = itemToDispense.Id;
            saveData.DispenserSaveDictionary.TryAdd(Origin, dispenserSaveData);
        }
    }

    #endregion
}

[Serializable]
public struct DispenserSaveData
{
    public int ItemToDispenseId;

    public DispenserSaveData(int itemToDispenseId)
    {
        ItemToDispenseId = itemToDispenseId;
    }
}

