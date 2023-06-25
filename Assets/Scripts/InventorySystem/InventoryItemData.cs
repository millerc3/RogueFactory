using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object to describe what an item is in our game.
///     It could be inherited from to have other versions of items, for example potions and equipment
/// </summary>

[CreateAssetMenu(menuName = "Inventory System/Inventory Item")]
public class InventoryItemData : DatabaseObject
{
    public string Name;
    [TextArea(4,4)]
    public string Description;
    public Sprite Icon;
    public int MaxStackSize;
    public int PriceValue;

    public GameObject Prefab;
    public FactoryBuildingData factoryBuilding;

    public bool CanBeUsedInDispenser = false;
}
