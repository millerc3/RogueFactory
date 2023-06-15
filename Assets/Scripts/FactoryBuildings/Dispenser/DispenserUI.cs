using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DispenserUI : MonoBehaviour
{
    private MouseItemData mouseItemData;
    private DispenserBuilding dispenser;
    public DispenserBuilding Dispenser => dispenser;

    [SerializeField] private UIClickHandler itemToDispenseSlot;

    [SerializeField] private TMP_Text itemToDispenseText;
    [SerializeField] private Image itemToDispenseImage;

    private void Awake()
    {
        mouseItemData = FindObjectOfType<MouseItemData>();
    }

    public void Setup(DispenserBuilding currDispenser)
    {
        dispenser = currDispenser;
        SetUIItem(dispenser.ItemToDispense);
    }

    private void OnEnable()
    {
        itemToDispenseSlot?.onLeftClick.AddListener(OnDispenseSlotClick);
    }

    private void OnDisable()
    {
        itemToDispenseSlot?.onLeftClick.RemoveListener(OnDispenseSlotClick);
    }

    private void OnDispenseSlotClick()
    {
        InventorySlot mouseSlot = mouseItemData.AssignedInventorySlot;

        if (mouseSlot.ItemData == null)
        {
            // mouseData == null -> Clear dispensing
            dispenser.SetItemToDispense(null);
        }
        else
        {
            // mouseData == item -> Set item to dispense to the item on the mouse
            dispenser.SetItemToDispense(mouseSlot.ItemData);
        }
        SetUIItem(dispenser.ItemToDispense);
    }

    private void SetUIItem(InventoryItemData newItem)
    {
        if (newItem == null)
        {
            itemToDispenseText.text = "Not dispensing";
            itemToDispenseImage.sprite = null;
        }
        else
        {
            itemToDispenseText.text = newItem.Name.ToString();
            itemToDispenseImage.sprite = newItem.Icon;
        }
    }

}
