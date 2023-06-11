using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Item Database")]
public class InventoryItemDatabase : ScriptableObject
{
    [SerializeField] private List<InventoryItemData> itemDatabase;

    public List<string> ItemsAsStringList= new List<string>();

    [ContextMenu("Set IDs")]
    public void SetItemIDs()
    {
        itemDatabase = new List<InventoryItemData>();

        // Get all InventoryItemData instances in "InventoryItemData" folder
        var foundItems = Resources.LoadAll<InventoryItemData>("InventoryItemData").OrderBy(i => i.Id).ToList();

        // Get all of the found items where the id IS NOT greater than the size of the foundItemsList
        var hasIdInRange = foundItems.Where(i => i.Id != -1 && i.Id < foundItems.Count).OrderBy(i => i.Id).ToList();

        // Get all of the found items where the id IS greater than the size of the foundItemsList
        var hasIdNotInRange = foundItems.Where(i => i.Id != -1 && i.Id >= foundItems.Count).OrderBy(i => i.Id).ToList();

        // Get all of the found items that do not yet have an ID set (-1)
        var hasNoId = foundItems.Where(i => i.Id == -1).ToList();

        var duplicates = foundItems.GroupBy(i => i.Id).Where(i => i.Count() > 1);
        foreach (var duplicate in duplicates)
        {
            if (duplicate.Key == -1) continue;

            Debug.LogWarning($"There multiple items with ID: {duplicate.Key}");
            var repeats = foundItems.Where(i => i.Id == duplicate.Key).ToList();
            foreach (var i in repeats)
            {
                Debug.LogWarning($"    * {i.Name}");
            }
        }

        int index = 0;
        for (int i = 0; i < foundItems.Count; i++)
        {
            InventoryItemData itemToAdd;

            itemToAdd = hasIdInRange.Find(d => d.Id == i);
            if (itemToAdd != null)
            {
                itemDatabase.Add(itemToAdd);
            }
            else if (index < hasNoId.Count)
            {
                hasNoId[index].Id = i;
                itemToAdd = hasNoId[index];
                index++;
                itemDatabase.Add(itemToAdd);
            }

            ItemsAsStringList.Add($"{i}: {itemToAdd.name}");

#if UNITY_EDITOR
            if (itemToAdd) EditorUtility.SetDirty(itemToAdd);
#endif
        }

        foreach (InventoryItemData item in hasIdNotInRange)
        {
            itemDatabase.Add(item);
            ItemsAsStringList.Add($"{item.Id}: {item.name}");

#if UNITY_EDITOR
            if (item) EditorUtility.SetDirty(item);
#endif
        }

#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif
    }

    public InventoryItemData GetItem(int id)
    {
        return itemDatabase.Find(i => i.Id == id);
    }
}
