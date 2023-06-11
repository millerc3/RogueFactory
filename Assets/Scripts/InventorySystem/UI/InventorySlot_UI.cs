using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(UIClickHandler))]
public class InventorySlot_UI : MonoBehaviour
{
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private InventorySlot assignedInventorySlot;

    private UIClickHandler clickHandler;

    //private Button button;
    public InventorySlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryDisplay ParentDisplay {get; private set; }

    private void Awake()
    {
        ClearSlot();

        clickHandler = GetComponent<UIClickHandler>();
        clickHandler?.onLeftClick.AddListener(OnUISlotClick);
        clickHandler?.onRightClick.AddListener(OnUISlotRightClick);
        clickHandler?.onMiddleClick.AddListener(OnUISlotMiddleClick);

        ParentDisplay = transform.parent.GetComponent<InventoryDisplay>();
    }

    public void Init(InventorySlot slot)
    {
        assignedInventorySlot = slot;
        UpdateUISlot(slot);
    }

    public void UpdateUISlot(InventorySlot slot)
    {
        if (slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.Icon;
            itemSprite.color = Color.white;

            if (slot.StackSize <= 0)
            {
                ClearSlot();
            }
            else if (slot.StackSize > 1)
            {
                itemCount.text = slot.StackSize.ToString();
            }
            else
            {
                itemCount.text = "";
            }
        }
        else
        {
            slot.ClearSlot();
            ClearSlot();
        }
    }

    public void UpdateUISlot()
    {
        if (assignedInventorySlot != null)
        {
            UpdateUISlot(AssignedInventorySlot);
        }
    }
    
    public void ClearSlot()
    {
        assignedInventorySlot?.ClearSlot();

        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = "";
    }

    public void OnUISlotClick()
    {
        ParentDisplay?.SlotLeftClicked(this);
    }

    public void OnUISlotRightClick()
    {
        ParentDisplay?.SlotRightClicked(this);
    }

    public void OnUISlotMiddleClick()
    {
        Debug.Log($"I was middle clicked!");
    }
}
