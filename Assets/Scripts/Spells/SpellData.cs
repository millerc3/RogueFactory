using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/New Spell Data")]
public class SpellData : DatabaseObject
{
    public string Name;
    [TextArea(4, 4)]
    public string Description;
    public Sprite Sprite;
    public GameObject Prefab; // Unsure
}
