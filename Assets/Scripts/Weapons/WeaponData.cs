using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Weapons/New Weapon Data"))]
public class WeaponData : ScriptableObject
{
    public string Name;
    [TextArea(4,4)]
    public string Description;
    public int Id = -1;
    public Sprite Sprite;
    public GameObject Prefab;
}
