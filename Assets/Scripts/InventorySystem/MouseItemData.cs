using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class MouseItemData : MonoBehaviour
{
    [field:SerializeField] public Image ItemSprite { get; private set; }
    [field: SerializeField] public TextMeshProUGUI ItemCount { get; private set; }
    [field: SerializeField] public InventorySlot AssignedInventorySlot { get; private set; }

    FactoryBuilderController buildController;

    private void Awake()
    {
        buildController = FindObjectOfType<FactoryBuilderController>();

        ItemSprite.color = Color.clear;
        ItemCount.text = "";
    }

    private void Update()
    {
        if (AssignedInventorySlot.ItemData != null)
        {
            transform.position = Mouse.current.position.ReadValue();

            //if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject())
            //{
            //    ClearSlot();
            //    // TODO: Drop the item on the ground
            //}
        }
    }

    public void ClearSlot()
    {
        AssignedInventorySlot.ClearSlot();
        ItemCount.text = "";
        ItemSprite.color = Color.clear;
        ItemSprite.sprite = null;

        buildController.StopPlacement();
        buildController.IsPlacing = false;
    }

    public void UpdateMouseSlot(InventorySlot inventorySlot)
    {
        AssignedInventorySlot.AssignItem(inventorySlot);
        RefreshMouseUI();
        StartPlacement();
    }

    public void RefreshMouseUI()
    {
        if (AssignedInventorySlot.ItemData == null)
        {
            ClearSlot();
            return;
        }

        ItemSprite.sprite = AssignedInventorySlot.ItemData.Icon;
        ItemCount.text = AssignedInventorySlot.StackSize.ToString();
        ItemSprite.color = Color.white;

        StartPlacement();
    }

    private void StartPlacement()
    {
        if (AssignedInventorySlot.ItemData.factoryBuilding != null)
        {
            buildController.StartPlacement(AssignedInventorySlot.ItemData.factoryBuilding.Id);
        }
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
