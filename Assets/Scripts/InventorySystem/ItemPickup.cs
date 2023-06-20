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
    public int StoredItemCount = 1;

    private SphereCollider itemCollider;

    [SerializeField] private ItemPickupSaveData itemSaveData;
    public string Id;

    private bool spawnedLocalItem = false;

    private void Awake()
    {
        //Id = GetComponent<UniqueID>().Id;
        //itemSaveData = new ItemPickupSaveData(ItemData, transform.position, transform.rotation);

        itemCollider = GetComponent<SphereCollider>();
        itemCollider.isTrigger = true;
        itemCollider.radius = PickupRadius;
    }

    private void Start()
    {
        SetItem(ItemData, StoredItemCount);

        //SaveGameManager.CurrentSaveData.ActiveItems.Add(Id, itemSaveData);
    }

    //private void OnEnable()
    //{
    //    SaveGameManager.PostLoadGameEvent += LoadItemData;
    //}

    //private void OnDisable()
    //{
    //    SaveGameManager.PostLoadGameEvent -= LoadItemData;
    //}

    public void SetItem(InventoryItemData item, int amount)
    {
        if (item == null) return;
        if (spawnedLocalItem) return;

        ItemData = item;
        StoredItemCount = amount;
        Init();
    }

    public void Init()
    {
        Instantiate(ItemData.Prefab, transform);
        gameObject.name = $"World Item - {ItemData.name}";

        spawnedLocalItem = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        CollectionManager collectionManager = other.transform.GetComponentInParent<CollectionManager>();
        if (!collectionManager) return;

        collectionManager.AddItemToCollection(ItemData, 1);

        Destroy(gameObject);




        //PlayerInventoryHolder inventory = other.transform.GetComponent<PlayerInventoryHolder>();
        //if (!inventory) return;

        //if (inventory.AddToInventory(ItemData, 1, out int numberOfItemsUnableToAddToInventory))
        //{
        //    SaveGameManager.CurrentSaveData.CollectedItems.Add(Id);
        //    Destroy(gameObject);
        //}
    }

    private void LoadItemData(SaveData saveData)
    {
        if (saveData.CollectedItems.Contains(Id))
        {
            Destroy(gameObject);
        }
    }

    //private void OnDestroy()
    //{
    //    if (SaveGameManager.CurrentSaveData.ActiveItems.ContainsKey(Id))
    //    {
    //        SaveGameManager.CurrentSaveData.ActiveItems.Remove(Id);
    //    }        
    //}
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
