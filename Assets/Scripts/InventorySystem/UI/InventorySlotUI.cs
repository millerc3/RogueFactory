using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(UIClickHandler))]
public class InventorySlotUI : MonoBehaviour
{
    protected MouseItemData mouseItemData;
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TMP_Text itemAmount;
    [SerializeField] protected InventorySlot inventorySlot;

    protected InventorySystem inventorySystem;
    [HideInInspector] public UIClickHandler ClickHandler;

    public InventorySlot InventorySlot => inventorySlot;

    public UnityAction OnPlayerInteraction;

    public bool PlayerCanAddItems = true;
    public bool PlayerCanRemoveItems = true;

    private void Awake()
    {
        mouseItemData = FindObjectOfType<MouseItemData>();
        ClickHandler = GetComponent<UIClickHandler>();
    }

    public virtual void Init(InventorySlot newSlotData)
    {
        inventorySlot = newSlotData;
        UpdateUISlot(inventorySlot);
    }

    private void OnEnable()
    {
        ClickHandler?.onLeftClick.AddListener(SlotLeftClicked);
    }

    private void OnDisable()
    {
        ClickHandler?.onLeftClick.RemoveListener(SlotLeftClicked);
    }

    public virtual void UpdateUISlot(InventorySlot newSlotData)
    {
        if (newSlotData.ItemData == null)
        {
            ClearSlot();
            return;
        }

        itemImage.sprite = newSlotData.ItemData.Icon;
        itemImage.color = Color.white;

        if (newSlotData.StackSize <= 0)
        {
            ClearSlot();
        }
        else if (newSlotData.StackSize > 1)
        {
            itemAmount.text = newSlotData.StackSize.ToString();
        }
        else
        {
            itemAmount.text = "";
        }
    }

    public virtual void ClearSlot()
    {
        InventorySlot?.ClearSlot();

        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemAmount.text = "";
    }

    public void UpdateUISlot()
    {
        if (inventorySlot != null)
        {
            UpdateUISlot(inventorySlot);
        }
    }

    public void SlotLeftClicked()
    {
        // TODO reimplement with the input actions
        bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;

        // clicked slot DOES have item, and mouse DOES NOT have an item, pick up item
        if (InventorySlot.ItemData != null && mouseItemData.AssignedInventorySlot.ItemData == null)
        {
            if (!PlayerCanRemoveItems)
            {
                return;
            }

            // If the player is holding shift, then split the stack
            if (isShiftPressed && InventorySlot.SplitStack(out InventorySlot halfStackSlot))
            {
                mouseItemData.UpdateMouseSlot(halfStackSlot);
                UpdateUISlot();
            }
            else
            {
                mouseItemData.UpdateMouseSlot(InventorySlot);
                ClearSlot();
            }

            OnPlayerInteraction?.Invoke();
            return;
        }

        // clicked slot DOES NOT have an item, and mouse DOES have an item, place the item in this slot
        if (InventorySlot.ItemData == null && mouseItemData.AssignedInventorySlot.ItemData != null)
        {
            if (!PlayerCanAddItems)
            {
                return;
            }

            InventorySlot.AssignItem(mouseItemData.AssignedInventorySlot);
            UpdateUISlot();

            mouseItemData.ClearSlot();
            OnPlayerInteraction?.Invoke();
            return;
        }

        // clicked slot DOES have an item, and the mouse DOES have an item, decide what to do...
        if (InventorySlot.ItemData != null && mouseItemData.AssignedInventorySlot.ItemData != null)
        {
            bool isSameItem = InventorySlot.ItemData == mouseItemData.AssignedInventorySlot.ItemData;

            // if the items are different
            if (!isSameItem)
            {
                SwapSlotsWithMouse();
                OnPlayerInteraction?.Invoke();
                return;
            }

            // if the items are the same
            if (isSameItem)
            {
                // if adding the items from the mouse to the clicked UI would go over the max stack size
                if (!InventorySlot.EnoughRoomLeftInStack(mouseItemData.AssignedInventorySlot.StackSize, out int leftInStack))
                {
                    // if stack is full, swap the items, otherwise take what is possible from the mouse's inventory
                    if (leftInStack < 1)
                    {
                        SwapSlotsWithMouse();
                    }
                    else
                    {
                        int remainingOnMouse = mouseItemData.AssignedInventorySlot.StackSize - leftInStack;
                        InventorySlot.AddToStack(leftInStack);
                        UpdateUISlot();

                        InventorySlot newItem = new InventorySlot(mouseItemData.AssignedInventorySlot.ItemData, remainingOnMouse);
                        mouseItemData.ClearSlot();
                        mouseItemData.UpdateMouseSlot(newItem);
                    }
                }
                else // if adding items from the mouse will NOT go over the max stack size
                {
                    InventorySlot.AssignItem(mouseItemData.AssignedInventorySlot);
                    UpdateUISlot();

                    mouseItemData.ClearSlot();
                }
            }
            OnPlayerInteraction?.Invoke();
        }
    }

    private void SwapSlotsWithMouse()
    {
        InventorySlot clonedSlot = new InventorySlot(mouseItemData.AssignedInventorySlot.ItemData, mouseItemData.AssignedInventorySlot.StackSize);
        mouseItemData.ClearSlot();

        mouseItemData.UpdateMouseSlot(InventorySlot);

        ClearSlot();

        InventorySlot.AssignItem(clonedSlot);
        UpdateUISlot();
    }
}
