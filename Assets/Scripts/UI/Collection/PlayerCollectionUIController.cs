using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCollectionUIController : MonoBehaviour
{
    private MouseItemData mouseItemData;
    [SerializeField] private GameObject collectionItemPrefab;
    public FactoryPlayerCollectionManager PlayerCollectionManager { get; private set; }

    [SerializeField] private UIClickHandler scrollViewClickHandler;

    private Dictionary<int, CollectionItemUI> collectionUIDict = new Dictionary<int, CollectionItemUI>();

    List<int> itemsToRemoveOnRefresh = new List<int>();

    private void Awake()
    {
        PlayerCollectionManager = FindObjectOfType<FactoryPlayerCollectionManager>();
        mouseItemData = FindObjectOfType<MouseItemData>();

        scrollViewClickHandler?.onLeftClick.AddListener(OnContentWindowClick);
        scrollViewClickHandler?.onRightClick.AddListener(OnContentWindowRightClick);
        scrollViewClickHandler?.onMiddleClick.AddListener(OnContentWindowSlotMiddleClick);
    }

    private void OnContentWindowSlotMiddleClick()
    {

    }

    private void OnContentWindowRightClick()
    {
        InventorySlot mouseSlot = mouseItemData.AssignedInventorySlot;

        // mouseData == null -> Do nothing
        if (mouseSlot.ItemData == null) return;

        // mouseData == item -> Place one of item into the collection
        PlayerCollectionManager.AddItemToCollection(mouseSlot.ItemData, 1);
        mouseSlot.RemoveFromStack(1);
        mouseItemData.RefreshMouseUI();
        RefreshUI();
    }

    private void OnContentWindowClick()
    {
        InventorySlot mouseSlot = mouseItemData.AssignedInventorySlot;

        // mouseData == null -> Do nothing
        if (mouseSlot.ItemData == null) return;

        // TODO reimplement with the input actions
        bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;

        // mouseData == null -> Place items from mouse into collection
        if (isShiftPressed && mouseSlot.SplitStack(out InventorySlot halfStackSlot))
        {
            // If shifting, place half of the stack into the collection
            PlayerCollectionManager.AddItemToCollection(halfStackSlot.ItemData, halfStackSlot.StackSize);
        }
        else
        {
            // If not shifting, place the whole stack into the collection
            PlayerCollectionManager.AddItemToCollection(mouseSlot.ItemData, mouseSlot.StackSize);
            mouseSlot.ClearSlot();
        }

        mouseItemData.RefreshMouseUI();
        RefreshUI();
    }

    private void OnEnable()
    {
        RefreshUI();
        PlayerCollectionManager.Collection.OnInventorySlotChanged += UpdateCollectionUISlot;
        PlayerCollectionManager.Collection.OnCollectionChanged += RefreshUI;
    }

    private void OnDisable()
    {
        PlayerCollectionManager.Collection.OnInventorySlotChanged -= UpdateCollectionUISlot;
        PlayerCollectionManager.Collection.OnCollectionChanged -= RefreshUI;
    }

    private void Start()
    {
        if (PlayerCollectionManager == null)
        {
            Debug.LogError($"There is no player collection manager");
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        itemsToRemoveOnRefresh.Clear();

        foreach (int storedItemId in collectionUIDict.Keys) 
        {
            itemsToRemoveOnRefresh.Add(storedItemId);        
        }

        foreach (InventorySlot collectionSlot in PlayerCollectionManager.Collection.InventorySlots)
        {
            if (itemsToRemoveOnRefresh.Contains(collectionSlot.ItemData.Id))
            {
                itemsToRemoveOnRefresh.Remove(collectionSlot.ItemData.Id);
            }
            
            if (collectionUIDict.TryGetValue(collectionSlot.ItemData.Id, out CollectionItemUI existingItemUI))
            {
                existingItemUI.UpdateUI(collectionSlot.StackSize);
                continue;
            }

            CollectionItemUI itemUI = Instantiate(collectionItemPrefab, transform).GetComponent<CollectionItemUI>();

            itemUI.SetInfo(collectionSlot.ItemData, collectionSlot.StackSize);
            itemUI.UpdateUI();

            collectionUIDict.Add(collectionSlot.ItemData.Id, itemUI);
        }

        foreach (int itemId in itemsToRemoveOnRefresh)
        {
            if (collectionUIDict.TryGetValue(itemId, out CollectionItemUI itemUI))
            {
                Destroy(itemUI.gameObject);
                collectionUIDict.Remove(itemId);
            }
        }
    }

    private void UpdateCollectionUISlot(InventorySlot collectionSlot)
    {
        if (collectionSlot == null)
        {
            RefreshUI();
            return;
        }

        if (collectionSlot.ItemData == null)
        {
            RefreshUI();
            return;
        }

        if (collectionSlot.StackSize == 0)
        {
            RefreshUI();
            return;
        }

        if (collectionUIDict.TryGetValue(collectionSlot.ItemData.Id, out CollectionItemUI itemUI))
        {
            itemUI.UpdateUI(collectionSlot.StackSize);
        }
    }
}
