using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropItemsOnDeath : MonoBehaviour
{
    [field: SerializeField] public List<DropItemChance> dropItemChances;

    [SerializeField] private EntityHealthController healthController;

    [SerializeField] private GameObject worldObjectPrefab;

    private void Awake()
    {
        if (healthController == null) healthController = GetComponent<EntityHealthController>();
    }

    private void OnEnable()
    {
        healthController.OnEntityDied  += DropItems;
    }

    private void OnDisable()
    {
        healthController.OnEntityDied -= DropItems;
    }

    private void DropItems()
    {
        float f;
        foreach (var item in dropItemChances)
        {
            f = Random.Range(0f, 1f);
            if (f <= item.ChanceToDrop)
            {
                ItemPickup worldItem = Instantiate(worldObjectPrefab, transform.position + Vector3.up, Quaternion.identity).GetOrAddComponent<ItemPickup>();
                worldItem.SetItem(item.ItemToDrop, Random.Range(1, 6));
            }
        }
    }
}

[System.Serializable]
public struct DropItemChance
{
    public InventoryItemData ItemToDrop;
    [Tooltip("Percent (0-1) that this item will drop")]
    public float ChanceToDrop;

    public DropItemChance(InventoryItemData itemToDrop, float chanceToDrop)
    {
        ItemToDrop = itemToDrop;
        ChanceToDrop = chanceToDrop;
    }
}
