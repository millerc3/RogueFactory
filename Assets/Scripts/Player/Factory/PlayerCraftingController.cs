using QFSW.QC;
using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCraftingController : MonoBehaviour
{
    private FactoryPlayerCollectionManager collection;
    [SerializeField] private CraftingRecipeDatabase recipeDatabse;
    [SerializeField] private InventoryItemDatabase itemDatabase;

    public bool CanCraft { get; private set; }

    private void Awake()
    {
        CanCraft = true;
    }

    private void Start()
    {
        collection = GetComponent<FactoryPlayerCollectionManager>();

        if (collection == null) Debug.LogError($"There is no PlayerItemCollection found in this scene!");
    }

    [Command]
    public void CraftItem(int itemToCraftID)
    {
        InventoryItemData itemToCraft = itemDatabase.GetItem(itemToCraftID);

        if (itemToCraft == null)
        {
            Debug.LogWarning($"There is no item with ID {itemToCraft}; Cannot craft!");
            return;
        }

        CraftItem(itemToCraft);
    }

    public void CraftItem(InventoryItemData itemToCraft)
    {
        List<CraftingRecipe> possibleRecipes = recipeDatabse.GetRecipesForItem(itemToCraft);

        if (possibleRecipes.Count == 0) return;

        // TODO: Figure out a better solution for this since this doesn't let the player
        //   pick between potential recipe
        foreach (CraftingRecipe recipe in possibleRecipes)
        {
            if (recipe.CanCraft(collection.Collection))
            {
                GenerateRecipeOutput(recipe);
                ConsumeRecipeInput(recipe);
            }
        }
    }

    private void GenerateRecipeOutput(CraftingRecipe recipe)
    {
        collection.Collection.AddItemToCollection(recipe.OutputItem.item, recipe.OutputItem.amount);
    }

    private void ConsumeRecipeInput(CraftingRecipe recipe)
    {
        foreach (CraftingRecipeComponent inputComponent in recipe.InputItems)
        {
            collection.Collection.RemoveItemFromCollection(inputComponent.item, inputComponent.amount);
        }
    }
}
