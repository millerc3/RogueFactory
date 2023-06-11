using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(UniqueID))]
public class ItemPickup : MonoBehaviour
{
    public float PickupRadius = 1f;
    public InventoryItemData ItemData;

    private SphereCollider itemCollider;

    [SerializeField] private ItemPickupSaveData itemSaveData;
    public string Id;

    private void Awake()
    {
        if (ItemData == null) Debug.LogError($"There is no itemData set on {name}");

        Id = GetComponent<UniqueID>().Id;
        itemSaveData = new ItemPickupSaveData(ItemData, transform.position, transform.rotation);

        itemCollider = GetComponent<SphereCollider>();
        itemCollider.isTrigger = true;
        itemCollider.radius = PickupRadius;
    }

    private void Start()
    {
        Instantiate(ItemData.Prefab, transform);
        gameObject.name = $"World Item - {ItemData.name}";

        SaveGameManager.CurrentSaveData.ActiveItems.Add(Id, itemSaveData);
    }

    private void OnEnable()
    {
        SaveGameManager.PostLoadGameEvent += LoadItemData;
    }

    private void OnDisable()
    {
        SaveGameManager.PostLoadGameEvent -= LoadItemData;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventoryHolder inventory = other.transform.GetComponent<PlayerInventoryHolder>();
        if (!inventory) return;

        if (inventory.AddToInventory(ItemData, 1, out int numberOfItemsUnableToAddToInventory))
        {
            SaveGameManager.CurrentSaveData.CollectedItems.Add(Id);
            Destroy(gameObject);
        }
    }

    private void LoadItemData(SaveData saveData)
    {
        if (saveData.CollectedItems.Contains(Id))
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (SaveGameManager.CurrentSaveData.ActiveItems.ContainsKey(Id))
        {
            SaveGameManager.CurrentSaveData.ActiveItems.Remove(Id);
        }        
    }
}

[System.Serializable]
public struct ItemPickupSaveData
{
    public InventoryItemData ItemData;
    public Vector3 Position;
    public Quaternion Rotation;

    public ItemPickupSaveData(InventoryItemData itemData, Vector3 position, Quaternion rotation)
    {
        ItemData = itemData;
        Position = position;
        Rotation = rotation;
    }
}
