using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] MouseItemData mouseItemData;
    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlot_UI, InventorySlot> slotDictionary;

    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlot_UI, InventorySlot> SlotDictionary => slotDictionary;

    protected virtual void Start()
    {

    }

    public abstract void AssignSlot(InventorySystem inventoryToDisplay);

    protected virtual void UpdateSlots(InventorySlot updatedSlot)
    {
        foreach (var slot in SlotDictionary)
        {
            if (slot.Value == updatedSlot)  // slot value - the "under the hood" inventory slot
            {
                slot.Key.UpdateUISlot(updatedSlot); // slot key - the UI representation of the value
            }
        }
    }

    public void SlotRightClicked(InventorySlot_UI clickedUISlot)
    {
        // mouseData == null --> select item (if possible)
        // mouseData == item --> attempt to place a single instance of the item into the slot


        InventoryItemData slotData = clickedUISlot.AssignedInventorySlot.ItemData;
        InventoryItemData mouseData = mouseItemData.AssignedInventorySlot.ItemData;

        if (slotData == null)
        {
            // slot = null, mouse = null  -->  return
            if (mouseData == null) return;

            // slot = null, mouse = item  -->  place one item from mouse into slot
            clickedUISlot.AssignedInventorySlot.UpdateInventorySlot(mouseData, 1);
            mouseItemData.AssignedInventorySlot.RemoveFromStack(1);
            clickedUISlot.UpdateUISlot();
            mouseItemData.RefreshMouseUI();
        }
        else
        {
            // slot = item, mouse = null  -->  grab just one item
            if (mouseData == null)
            {
                mouseItemData.AssignedInventorySlot.UpdateInventorySlot(slotData, 1);
                clickedUISlot.AssignedInventorySlot.RemoveFromStack(1);
                clickedUISlot.UpdateUISlot();
                mouseItemData.RefreshMouseUI();
                return;
            }

            if (mouseData != slotData) return;

            // slot = item, mouse = item  -->  attempt to place just one item from mouse into slot
            if (clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(1))
            {
                clickedUISlot.AssignedInventorySlot.AddToStack(1);
                mouseItemData.AssignedInventorySlot.RemoveFromStack(1);
                clickedUISlot.UpdateUISlot();
                mouseItemData.RefreshMouseUI();
            }
        }
    }

    public void SlotLeftClicked(InventorySlot_UI clickedUISlot)
    {
        // TODO reimplement with the input actions
        bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;

        // clicked slot DOES have item, and mouse DOES NOT have an item, pick up item
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseItemData.AssignedInventorySlot.ItemData == null)
        {
            // If the player is holding shift, then split the stack
            if (isShiftPressed && clickedUISlot.AssignedInventorySlot.SplitStack(out InventorySlot halfStackSlot))
            {
                mouseItemData.UpdateMouseSlot(halfStackSlot);
                clickedUISlot.UpdateUISlot();
            }
            else
            {
                mouseItemData.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
                clickedUISlot.ClearSlot();
            }
            return;
        }

        // clicked slot DOES NOT have an item, and mouse DOES have an item, place the item in this slot
        if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseItemData.AssignedInventorySlot.ItemData != null)
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(mouseItemData.AssignedInventorySlot);
            clickedUISlot.UpdateUISlot();

            mouseItemData.ClearSlot();
            return;
        }

        // clicked slot DOES have an item, and the mouse DOES have an item, decide what to do...
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseItemData.AssignedInventorySlot.ItemData != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseItemData.AssignedInventorySlot.ItemData;

            // if the items are different
            if (!isSameItem)
            {
                SwapSlots(clickedUISlot);
                return;
            }

            // if the items are the same
            if (isSameItem)
            {
                // if adding the items from the mouse to the clicked UI would go over the max stack size
                if (!clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseItemData.AssignedInventorySlot.StackSize, out int leftInStack))
                {
                    // if stack is full, swap the items, otherwise take what is possible from the mouse's inventory
                    if (leftInStack < 1)
                    {
                        SwapSlots(clickedUISlot);
                    }
                    else
                    {
                        int remainingOnMouse = mouseItemData.AssignedInventorySlot.StackSize - leftInStack;
                        clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                        clickedUISlot.UpdateUISlot();

                        InventorySlot newItem = new InventorySlot(mouseItemData.AssignedInventorySlot.ItemData, remainingOnMouse);
                        mouseItemData.ClearSlot();
                        mouseItemData.UpdateMouseSlot(newItem);
                    }
                }
                else // if adding items from the mouse will NOT go over the max stack size
                {
                    clickedUISlot.AssignedInventorySlot.AssignItem(mouseItemData.AssignedInventorySlot);
                    clickedUISlot.UpdateUISlot();

                    mouseItemData.ClearSlot();
                }
            }
        }
    }

    private void SwapSlots(InventorySlot_UI clickedUISlot)
    {
        InventorySlot clonedSlot = new InventorySlot(mouseItemData.AssignedInventorySlot.ItemData, mouseItemData.AssignedInventorySlot.StackSize);
        mouseItemData.ClearSlot();

        mouseItemData.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);

        clickedUISlot.ClearSlot();

        clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
        clickedUISlot.UpdateUISlot();
    }
}
