using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


/// <summary>
/// This is for items in the collection that can be accessed and used by the factory player
/// </summary>
[RequireComponent(typeof(UIClickHandler))]
public class CollectionItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text itemCountText;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private Image itemImage;

    private UIClickHandler clickHandler;
    private MouseItemData mouseItemData;
    private PlayerCollectionUIController collectionUIController;

    private InventoryItemData storedItem;
    private int storedAmount;

    private void Awake()
    {
        clickHandler = GetComponent<UIClickHandler>();
        clickHandler?.onLeftClick.AddListener(OnUISlotClick);
        clickHandler?.onRightClick.AddListener(OnUISlotRightClick);
        clickHandler?.onMiddleClick.AddListener(OnUISlotMiddleClick);

        mouseItemData = FindObjectOfType<MouseItemData>();
        collectionUIController = transform.parent.GetComponent<PlayerCollectionUIController>();
    }

    private void OnUISlotMiddleClick()
    {

    }

    private void OnUISlotRightClick()
    {
        // mouseData == null -> Pick up 1 of item and place it into mouse
        // mouseData == item -> Place a single instance of the item into the collection
    }

    private void OnUISlotClick()
    {
        InventorySlot mouseSlot = mouseItemData.AssignedInventorySlot;

        // TODO reimplement with the input actions
        bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;

        if (mouseSlot.ItemData == null)
        {
            // mouseData == null -> Pick up (up to MaxStackSize) of item and place it into mouse

            int amountAddedToMouse = 0;
            if (isShiftPressed && storedAmount > 1)
            {
                int halfCollectionStackSize = Mathf.RoundToInt(storedAmount / 2);
                int halfMaxMouseHoldStackSize = Mathf.RoundToInt(storedItem.MaxStackSize / 2);
                amountAddedToMouse = Mathf.Min(halfCollectionStackSize, halfMaxMouseHoldStackSize);
                mouseSlot.AssignItem(new InventorySlot(storedItem, amountAddedToMouse));
                collectionUIController.PlayerCollectionManager.RemoveItemFromCollection(storedItem, amountAddedToMouse);
                storedAmount -= amountAddedToMouse;
            }
            else
            {
                amountAddedToMouse = Mathf.Min(storedAmount, storedItem.MaxStackSize);
                mouseSlot.AssignItem(new InventorySlot(storedItem, amountAddedToMouse));
                collectionUIController.PlayerCollectionManager.RemoveItemFromCollection(storedItem, amountAddedToMouse);
                storedAmount -= amountAddedToMouse;
            }            
        }
        else
        {
            // mouseData == item -> Add item to collection
            if (isShiftPressed && mouseSlot.SplitStack(out InventorySlot halfStackSlot))
            {
                // if shift is pressed, add half of the mouse's stack to the collection
                collectionUIController.PlayerCollectionManager.AddItemToCollection(halfStackSlot.ItemData, halfStackSlot.StackSize);
            }
            else
            {
                // Otherwise, add the whole stack to the collection
                collectionUIController.PlayerCollectionManager.AddItemToCollection(mouseSlot.ItemData, mouseSlot.StackSize);
                mouseSlot.ClearSlot();
            }
        }

        mouseItemData.RefreshMouseUI();
        collectionUIController.RefreshUI();
    }

    public void SetInfo(InventoryItemData item, int itemCount)
    {
        storedItem= item;
        storedAmount= itemCount;

    }

    public void UpdateUI(int itemCount)
    {
        storedAmount = itemCount;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (storedItem == null)
        {
            Debug.LogError($"There is no stored item to update information... Did you forget to call SetInfo()?");
            return;
        }

        itemCountText.text = storedAmount.ToString();
        itemNameText.text = storedItem.Name;
        itemImage.sprite = storedItem.Icon;
    }
}
