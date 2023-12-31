using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Weapons/New Weapon Data"))]
public class WeaponData : DatabaseObject
{
    public string Name;
    [TextArea(4,4)]
    public string Description;
    public Sprite Sprite;
    public GameObject Prefab;
}
