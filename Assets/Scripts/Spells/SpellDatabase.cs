using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Spell Database")]
public class SpellDatabase : ObjectDatabase
{
    public SpellData GetSpell(int id)
    {
        return Objects.Find(i => i.Id == id) as SpellData;
    }
}
