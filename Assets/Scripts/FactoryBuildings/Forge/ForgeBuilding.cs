using SaveLoadSystem;
using Shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ForgeInventory))]
public class ForgeBuilding : FactoryBuilding
{
    private ForgeInventory inventory;
    private ForgeSaveData forgeSaveData;

    [SerializeField] private CraftingRecipeDatabase recipeDatabase;
    private CraftingRecipe currentRecipe;

    private int tickTimer;

    #region MonoBehaviour

    protected override void Awake()
    {
        base.Awake();

        inventory = GetComponent<ForgeInventory>();
        forgeSaveData = new ForgeSaveData(inventory.InputInventory, inventory.OutputInventory);
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
            ForgeInventory.OnForgeInventoryHideRequested?.Invoke(inventory);
        }
    }

    #endregion

    #region Forge

    private bool CanSmelt()
    {
        if (currentRecipe == null) return false;

        if (!inventory.OutputInventory.CanFitItem(currentRecipe.OutputItem.item, 
                                                  currentRecipe.OutputItem.amount, 
                                                  out int _)) 
            return false;
        
        return true;
    }

    private void DetermineCurrentRecipe()
    {
        currentRecipe = null;
        if (inventory.InputInventory.IsEmpty()) return;

        foreach (CraftingRecipe recipe in recipeDatabase.Recipes)
        {
            if (!recipe.CanCraft(inventory.InputInventory, BuildingData)) continue;

            currentRecipe = recipe;
            return;
        }
    }

    private void GenerateRecipeOutput()
    {
        if (!CanSmelt()) return;

        inventory.OutputInventory.AddItemToInventory(currentRecipe.OutputItem.item, currentRecipe.OutputItem.amount, out int _);
    }
    
    private void ConsumeRecipeInput()
    {
        foreach (CraftingRecipeComponent recipeInput in currentRecipe.InputItems)
        {
            inventory.InputInventory.RemoveItemFromInventory(recipeInput.item, recipeInput.amount, out _);
        }
    }

    #endregion



    #region FactoryBuilding

    protected override void OnTick()
    {
        base.OnTick();

        if (inventory.InputInventory.IsEmpty() || !CanSmelt()) return;

        tickTimer += 1;

        if (tickTimer >= currentRecipe.TicksToCraft)
        {
            GenerateRecipeOutput();
            ConsumeRecipeInput();
            DetermineCurrentRecipe();
            tickTimer = 0;
        }
    }

    public override void LocalUpdate(int depth)
    {
        if (depth > 1) return;

        CheckNeighbors();
        UpdateNeighbors(++depth);
    }

    protected override void TearDown()
    {
        foreach (InventorySlot slot in inventory.InputInventory.InventorySlots)
        {
            if (slot.ItemData == null || slot.StackSize <= 0) continue;

            // TODO Put numberUnableToAdd onto the ground or something
            playerInventoryHolder.AddToInventory(slot.ItemData, slot.StackSize, out int numberUnableToAdd);
        }

        foreach (InventorySlot slot in inventory.OutputInventory.InventorySlots)
        {
            if (slot.ItemData == null || slot.StackSize <= 0) continue;

            // TODO Put numberUnableToAdd onto the ground or something
            playerInventoryHolder.AddToInventory(slot.ItemData, slot.StackSize, out int numberUnableToAdd);
        }

        base.TearDown();
    }

    public override int AddToInput(InventoryItemData item, int count, GameObject worldObject)
    {
        bool ret = inventory.AddToInputInventory(item, count, out int numberUnableToAdd);
        Destroy(worldObject);

        DetermineCurrentRecipe();

        return count - numberUnableToAdd;
    }

    public override bool CanAddToInput(InventoryItemData item, int count, out int AmountOfItemsThatCanBeAdded)
    {
        AmountOfItemsThatCanBeAdded = 0;
        bool ret = inventory.CanAddItemsToInput(item, count, out int numberUnableToAdd);
        if (ret)
        {
            AmountOfItemsThatCanBeAdded = count - numberUnableToAdd;
        }
        return ret;
    }

    public override bool CanRemoveFromOutput(out int amountAbleToRemove)
    {
        return inventory.CanRemoveFromOutput(out amountAbleToRemove);
    }

    public override int RemoveFromOutput(int count, out InventoryItemData removedItem, out GameObject worldObject)
    {
        worldObject = null;
        bool successfullyRemoved = inventory.RemoveFromOutputInventory(count, out removedItem, out int numberOfItemsUnableToRemove);
        //print($"sucessfully removed {count - numberOfItemsUnableToRemove} of {removedItem.Name}");

        return count - numberOfItemsUnableToRemove;
    }

    #endregion

    #region InteractionSystem

    protected override void OnTapInteract()
    {
        base.OnTapInteract();

        ForgeInventory.OnForgeInventoryDisplayRequested?.Invoke(inventory);
    }

    #endregion

    #region SAVE/LOAD SYSTEM

    public override void LoadData(SaveData saveData)
    {
        if (saveData.ForgeSaveDictionary.TryGetValue(Origin, out ForgeSaveData storedSaveData))
        {
            inventory.SetInputInventory(storedSaveData.InputInventory);
            inventory.SetOutputInventory(storedSaveData.InputInventory);
        }
    }

    protected override void SaveData(SaveData saveData)
    {
        if (saveData.ForgeSaveDictionary.TryGetValue(Origin, out ForgeSaveData storedSaveData))
        {
            storedSaveData.InputInventory = inventory.InputInventory;
            storedSaveData.OutputInventory = inventory.OutputInventory;
        }
        else
        {
            saveData.ForgeSaveDictionary.TryAdd(Origin, forgeSaveData);
        }
    }

    #endregion
}

[Serializable]
public struct ForgeSaveData
{
    public InventorySystem InputInventory;
    public InventorySystem OutputInventory;

    public ForgeSaveData(InventorySystem inputInventory, InventorySystem outputInventory)
    {
        InputInventory = inputInventory;
        OutputInventory = outputInventory;
    }
}