using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ForgeBuildingUI : MonoBehaviour
{
    private ForgeInventory forgeInventory;
    private ForgeBuilding forge;
    public ForgeInventory ForgeInventory => forgeInventory;

    [SerializeField] private InventorySlotUI inputInventorySlotUI;
    [SerializeField] private InventorySlotUI outputInventorySlotUI;

    private void Update()
    {
        // TODO : Replace with input system
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            gameObject.SetActive(false);
        }
    }

    public void Setup(ForgeBuilding forgeBuilding)
    {
        forge = forgeBuilding;
        forgeInventory = forge.Inventory;

        inputInventorySlotUI.Init(forgeInventory.InputInventory.InventorySlots[0]);
        outputInventorySlotUI.Init(forgeInventory.OutputInventory.InventorySlots[0]);

        inputInventorySlotUI.OnPlayerInteraction += forge.DetermineCurrentRecipe;

        ForgeInventory.InputInventory.OnInventorySlotChanged += UpdateInputSlotUI;
        ForgeInventory.OutputInventory.OnInventorySlotChanged += UpdateOutputSlotUI;
    }

    private void OnEnable()
    {
        //inputInventorySlotUI.ClickHandler?.onLeftClick.AddListener(OnInputSlotClick);
        //inputInventorySlotUI.ClickHandler?.onRightClick.AddListener(OnInputSlotRightClick);
        //inputInventorySlotUI.ClickHandler?.onMiddleClick.AddListener(OnInputSlotMiddleClick);

        //outputInventorySlotUI.ClickHandler?.onLeftClick.AddListener(OnOutputSlotClick);
        //outputInventorySlotUI.ClickHandler?.onRightClick.AddListener(OnOutputSlotRightClick);
        //outputInventorySlotUI.ClickHandler?.onMiddleClick.AddListener(OnOutputSlotMiddleClick);
    }

    private void OnDisable()
    {
        ForgeInventory.InputInventory.OnInventorySlotChanged -= UpdateInputSlotUI;
        ForgeInventory.OutputInventory.OnInventorySlotChanged -= UpdateOutputSlotUI;

        inputInventorySlotUI.OnPlayerInteraction -= forge.DetermineCurrentRecipe;

        //inputInventorySlotUI.ClickHandler?.onLeftClick.RemoveListener(OnInputSlotClick);
        //inputInventorySlotUI.ClickHandler?.onRightClick.RemoveListener(OnInputSlotRightClick);
        //inputInventorySlotUI.ClickHandler?.onMiddleClick.RemoveListener(OnInputSlotMiddleClick);

        //outputInventorySlotUI.ClickHandler?.onLeftClick.RemoveListener(OnOutputSlotClick);
        //outputInventorySlotUI.ClickHandler?.onRightClick.RemoveListener(OnOutputSlotRightClick);
        //outputInventorySlotUI.ClickHandler?.onMiddleClick.RemoveListener(OnOutputSlotMiddleClick);
    }

    private void UpdateInputSlotUI(InventorySlot updatedSlot)
    {
        if (updatedSlot != inputInventorySlotUI.InventorySlot) return;

        inputInventorySlotUI.UpdateUISlot(updatedSlot);
    }

    private void UpdateOutputSlotUI(InventorySlot updatedSlot)
    {
        if (updatedSlot != outputInventorySlotUI.InventorySlot) return;

        outputInventorySlotUI.UpdateUISlot(updatedSlot);
    }

    //private void OnInputSlotClick()
    //{

    //}

    //private void OnInputSlotRightClick()
    //{

    //}

    //private void OnInputSlotMiddleClick()
    //{

    //}

    //private void OnOutputSlotClick()
    //{

    //}

    //private void OnOutputSlotRightClick()
    //{

    //}

    //private void OnOutputSlotMiddleClick()
    //{

    //}
}
