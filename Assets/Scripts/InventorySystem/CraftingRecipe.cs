using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public List<CraftingRecipeComponent> InputItems;

    public CraftingRecipeComponent OutputItem;

    public int TicksToCraft;

    // Which factory buildings can process this recipe
    public List<FactoryBuildingData> processingBuildings;
    public bool CanCraftByHand = false;

    public int GetRequiredInputAmount(InventoryItemData item)
    {
        if (item == null) return 0;

        foreach (CraftingRecipeComponent recipeInput in InputItems)
        {
            if (recipeInput.item != item) continue;
            return recipeInput.amount;
        }

        return 0;
    }

    public bool EnoughItemsForRecipe(InventoryItemData item, int amount)
    {
        foreach (CraftingRecipeComponent recipeInput in InputItems)
        {
            if (recipeInput.item != item) continue;
            return (recipeInput.amount <= amount);
        }
        return false;
    }

    // Collection crafting is only for when the player wants to craft things themselves
    //   instead of using a factory machine
    public bool CanCraft(CollectionSystem collectionSystem)
    {
        if (!CanCraftByHand) return false;

        foreach (CraftingRecipeComponent inputComponent in InputItems)
        {
            // if the collectionSystem doesn't contain this item, it cannot be crafted
            if (!collectionSystem.ContainsItem(inputComponent.item)) return false;

            // get the number of instances of this item that the collectionSysetm has
            int instancesInCollection = collectionSystem.GetAmountOfItemInCollection(inputComponent.item); ;

            // If the number of instances of this item is less than what the recipe needs,
            //   it cannot be crafted
            if (instancesInCollection < inputComponent.amount)
            {
                return false;
            }
        }

        // If we got here, the collectionSystem has to have enough of each of the input items
        //  to successfully craft the recipe

        return true;
    }

    
    // TODO make another that iterates through multiple inventory systems
    //      ^- this will allow to check if crafting can be done if you combine both the player's
    //         hotbar AND their inventory since theyre two different system
    public bool CanCraft(InventorySystem inputInventory, FactoryBuildingData building)
    {
        // If the requested building can't even process it, return false
        if (!processingBuildings.Contains(building)) return false;

        // Create a running list of "missing items" this list will denote the items missing
        //  from the <inputInventory> at the end of the function
        List<InventoryItemData> missingItems = new List<InventoryItemData>();
        foreach (CraftingRecipeComponent inputItem in InputItems)
        {
            missingItems.Add(inputItem.item);
        }

        // iterate over all of the items in the inputInventory and determine if 
        //   the item can be crafted or not
        foreach(InventorySlot itemSlot in inputInventory.InventorySlots)
        {
            // if this slot is empty, move to the next one
            if (itemSlot.ItemData == null) continue;
            
            // if this slot contains an item we already know we have enough of, move to the next one
            if (!missingItems.Contains(itemSlot.ItemData)) continue;

            // if the recipe does not even need this item, we can move to the net one
            if (GetRequiredInputAmount(itemSlot.ItemData) == 0) continue;

            // get the total number of this item in the inputInventory
            int totalItems = inputInventory.GetAmountOfItemInInventory(itemSlot.ItemData);

            // if the inventory has enough of this item for the recipe
            if (EnoughItemsForRecipe(itemSlot.ItemData, totalItems))
            {
                // remove this item from the misisng items list, since it is no longer missing
                missingItems.Remove(itemSlot.ItemData);
            }
            else
            {
                // we found an item we need, but we don't have enough for it, so we CANNOT craft
                return false;
            }
        }

        // If we got here and there are no more missing items, the inputInventory has all
        //   required items to craft this
        return missingItems.Count == 0;
    }
}

[Serializable]
public struct CraftingRecipeComponent
{
    public InventoryItemData item;
    public int amount;
}