using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FactoryBuildingData : ScriptableObject
{
    public string Name;
    public int Id = -1;
    public Vector2Int Size;
    public GameObject Prefab;
    public Sprite Sprite;
    public GameObject previewPrefab;
    public InventoryItemData InventoryItem;
}
