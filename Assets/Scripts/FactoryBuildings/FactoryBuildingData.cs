using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Factory Building Data")]
public class FactoryBuildingData : DatabaseObject
{
    public string Name;
    public Vector2Int Size;
    public GameObject Prefab;
    public Sprite Sprite;
    public GameObject previewPrefab;
    public InventoryItemData InventoryItem;
}
