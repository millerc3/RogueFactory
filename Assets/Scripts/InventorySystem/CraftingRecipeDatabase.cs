using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Crafting Recipe Databse")]
public class CraftingRecipeDatabase : ScriptableObject
{
    public List<CraftingRecipe> Recipes;

    public List<CraftingRecipe> GetRecipesForItem(InventoryItemData outputItem)
    {
        List<CraftingRecipe> recipes = new List<CraftingRecipe>();

        foreach(CraftingRecipe recipe in Recipes)
        {
            if (recipe.OutputItem.item == outputItem)
            {
                recipes.Add(recipe);
            }
        }

        return recipes;
    }
}
