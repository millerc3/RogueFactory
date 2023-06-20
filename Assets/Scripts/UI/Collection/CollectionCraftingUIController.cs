using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CollectionCraftingUIController : MonoBehaviour
{
    [SerializeField] private GameObject craftingItemPrefab;
    [SerializeField] private CraftingRecipeDatabase recipeDatabase;
    public FactoryPlayerCollectionManager PlayerCollectionManager { get; private set; }
    public PlayerCraftingController PlayerCraftingController { get; private set; }

    private Dictionary<int, CraftingItemUI> craftingItemDict = new Dictionary<int, CraftingItemUI>();

    List<int> itemsToRemoveOnRefresh = new List<int>();

    private void Awake()
    {
        PlayerCollectionManager = FindObjectOfType<FactoryPlayerCollectionManager>();
        PlayerCraftingController = FindObjectOfType<PlayerCraftingController>();
    }

    private void OnEnable()
    {
        RefreshUI();
        PlayerCollectionManager.Collection.OnCollectionChanged += RefreshUI;
    }

    private void OnDisable()
    {
        PlayerCollectionManager.Collection.OnCollectionChanged -= RefreshUI;
    }

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        itemsToRemoveOnRefresh.Clear();
        InventoryItemData outputItem;

        foreach (int storedItemId in craftingItemDict.Keys)
        {
            itemsToRemoveOnRefresh.Add(storedItemId);
        }

        foreach (CraftingRecipe recipe in recipeDatabase.Recipes)
        {
            if (!recipe.CanCraftByHand) continue;
            if (!recipe.CanCraft(PlayerCollectionManager.Collection)) continue;

            outputItem = recipe.OutputItem.item;

            if (itemsToRemoveOnRefresh.Contains(outputItem.Id))
            {
                itemsToRemoveOnRefresh.Remove(outputItem.Id);
            }

            if (craftingItemDict.TryGetValue(outputItem.Id, out CraftingItemUI existingItemUI))
            {
                //existingItemUI.UpdateUI();
                continue;
            }

            CraftingItemUI itemUI = Instantiate(craftingItemPrefab, transform).GetComponent<CraftingItemUI>();

            itemUI.SetRecipe(recipe);
            craftingItemDict.Add(outputItem.Id, itemUI);
        }

        foreach (int itemId in itemsToRemoveOnRefresh)
        {
            if (craftingItemDict.TryGetValue(itemId, out CraftingItemUI itemUI))
            {
                Destroy(itemUI.gameObject);
                craftingItemDict.Remove(itemId);
            }
        }
    }
}
