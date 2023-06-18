using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingItemUI : MonoBehaviour
{
    [SerializeField] private GameObject craftingInputImagePrefab;
    [SerializeField] private GameObject inputHStack;
    [SerializeField] private Image outputItemImage;
    [SerializeField] private TMP_Text outputItemCountText;
    private CollectionCraftingUIController uiCraftingController;

    [SerializeField] private UIClickHandler clickHandler;
    public CraftingRecipe Recipe { get; private set; }

    private void Awake()
    {
        uiCraftingController = GetComponentInParent<CollectionCraftingUIController>();
    }

    private void OnEnable()
    {
        clickHandler?.onLeftClick.AddListener(OnCraftingItemClick);
    }

    private void OnDisable()
    {
        clickHandler?.onLeftClick.RemoveListener(OnCraftingItemClick);
    }

    public void SetRecipe(CraftingRecipe recipe)
    {
        Recipe = recipe;

        foreach (CraftingRecipeComponent inputComponent in recipe.InputItems)
        {
            CraftingInputItemUI inputItemUI = Instantiate(craftingInputImagePrefab, inputHStack.transform).GetComponent<CraftingInputItemUI>();
            inputItemUI.SetData(inputComponent.item, inputComponent.amount);
        }

        outputItemImage.sprite = recipe.OutputItem.item.Icon;
        outputItemCountText.text = recipe.OutputItem.amount.ToString();
    }

    private void OnCraftingItemClick()
    {
        uiCraftingController.PlayerCraftingController.CraftItem(Recipe.OutputItem.item.Id);
    }
}
