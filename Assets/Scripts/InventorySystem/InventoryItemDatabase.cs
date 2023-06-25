using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Item Database")]
public class InventoryItemDatabase : ObjectDatabase
{
    public InventoryItemData GetItem(int id)
    {
        return Objects.Find(i => i.Id == id) as InventoryItemData;
    }
}
