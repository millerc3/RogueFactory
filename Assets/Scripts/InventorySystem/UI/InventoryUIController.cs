using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIController : MonoBehaviour
{
    public DynamicInventoryDisplay ChestPanel;
    public DynamicInventoryDisplay PlayerBackpackPanel;
    public GameObject PlayerCollectionUI;

    public StaticInventoryDisplay PlayerHotBar;

    private void Awake()
    {
        ChestPanel.gameObject.SetActive(false);
        PlayerBackpackPanel.gameObject.SetActive(false);

    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            PlayerCollectionUI.SetActive(!PlayerCollectionUI.activeSelf);
        }

        if (ChestPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
            ChestPanel.gameObject.SetActive(false);

        if (PlayerBackpackPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
            PlayerBackpackPanel.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
        InventoryHolder.OnDynamicInventoryHideRequested += HideInventory;

        PlayerInventoryHolder.OnPlayerBackpackDisplayRequested += DisplayPlayerBackpack;
    }

    private void OnDisable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
        InventoryHolder.OnDynamicInventoryHideRequested -= HideInventory;

        PlayerInventoryHolder.OnPlayerBackpackDisplayRequested -= DisplayPlayerBackpack;
    }

    private void DisplayInventory(InventorySystem inventorySystemToDisplay)
    {
        ChestPanel.gameObject.SetActive(true);
        ChestPanel.RefreshDynamicInventory(inventorySystemToDisplay);
    }

    private void HideInventory(InventorySystem inventorySystemToDisplay)
    {
        ChestPanel.gameObject.SetActive(false);
    }
    
    private void DisplayPlayerBackpack(InventorySystem inventorySystemToDisplay)
    {
        PlayerBackpackPanel.gameObject.SetActive(true);
        PlayerBackpackPanel.RefreshDynamicInventory(inventorySystemToDisplay);
    }

}
