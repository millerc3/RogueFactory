using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is for items that were collecetd during the run - this is a readonly view for the player
/// </summary>
public class CollectedItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text itemCountText;
    [SerializeField] private Image itemImage;

    private InventoryItemData storedItem;
    private int storedAmount;

    public void UpdateUI(InventoryItemData item, int amount)
    {
        storedItem = item;
        storedAmount = amount;

        itemCountText.text =amount.ToString();
        itemImage.sprite = storedItem.Icon;
    }
}
