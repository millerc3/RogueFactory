using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatCollectionView : MonoBehaviour
{
    [SerializeField] private Transform collectionViewGridTransform;
    [SerializeField] private GameObject collectedItemUIPrefab;

    private CollectionManager playerCollectionManager;

    private Dictionary<InventoryItemData, CollectedItemUI> collectedItemUIDict = new Dictionary<InventoryItemData, CollectedItemUI>();

    public float TimeToShowUI = 5f;
    private float showTimer = float.MaxValue;

    private void Awake()
    {
        playerCollectionManager = FindObjectOfType<CollectionManager>();
    }

    private void Update()
    {
        if (TimeToShowUI <= 0)
        {
            HideUI();
        }
    }

    private void OnEnable()
    {
        RefreshUI();
        playerCollectionManager.Collection.OnInventorySlotChanged += RefreshSlotUI;
    }

    private void OnDisable()
    {
        playerCollectionManager.Collection.OnInventorySlotChanged -= RefreshSlotUI;
    }

    public void ShowUI()
    {
        showTimer = TimeToShowUI;
        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        if (!gameObject.activeSelf) return;

        showTimer = float.MaxValue;
        gameObject.SetActive(false);
    }

    private void RefreshUI()
    {
        foreach (InventorySlot collectionSlot in playerCollectionManager.Collection.InventorySlots)
        {
            RefreshSlotUI(collectionSlot);
        }
    }

    private void RefreshSlotUI(InventorySlot collectionSlot)
    {
        if (collectedItemUIDict.TryGetValue(collectionSlot.ItemData, out CollectedItemUI itemUI))
        {
            itemUI.UpdateUI(collectionSlot.ItemData, collectionSlot.StackSize);
        }
        else
        {
            CollectedItemUI collectedItemUI = Instantiate(collectedItemUIPrefab, collectionViewGridTransform).GetComponent<CollectedItemUI>();
            collectedItemUI.UpdateUI(collectionSlot.ItemData, collectionSlot.StackSize);
            collectedItemUIDict.Add(collectionSlot.ItemData, collectedItemUI);
        }
    }
}
