using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingInputItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemAmountCountText;

    public void SetData(InventoryItemData item, int amount)
    {
        itemImage.sprite = item.Icon;
        itemAmountCountText.text = amount.ToString();
    }
}
