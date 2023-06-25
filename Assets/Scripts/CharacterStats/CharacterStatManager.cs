using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatManager : MonoBehaviour
{
    //public Dictionary<string, CharacterStat> statDictionary = new Dictionary<string, CharacterStat>();

    public Dictionary<StatType, CharacterStat> characterStatsDictionary = new Dictionary<StatType, CharacterStat>();

    [Header("Initial Charcter Stat Values")]
    [Tooltip("Use this to define the BASE values of each of the character stats - these will not change over time!")]
    public List<CharacterStat> characterStats = new List<CharacterStat>();

    private void Awake()
    {
        foreach (CharacterStat stat in characterStats)
        {
            if (!characterStatsDictionary.ContainsKey(stat.StatType))
            {
                characterStatsDictionary.Add(stat.StatType, stat);
            }
        }
    }

    public void AddStatModifier(StatModifier statModifier)
    {
        if (characterStatsDictionary.TryGetValue(statModifier.StatType, out CharacterStat stat))
        {
            stat.AddModifier(statModifier);
        }
    }

    public void RemoveStatModifier(StatModifier statModifier)
    {
        if (characterStatsDictionary.TryGetValue(statModifier.StatType, out CharacterStat stat))
        {
            stat.RemoveModifier(statModifier);
        }
    }

    public void RemoveAllModifiers()
    {
        foreach (CharacterStat stat in characterStatsDictionary.Values)
        {
            stat.RemoveAllModifiers();
        }
    }

    public void RemoveAllModifiersOfType(StatType statType)
    {
        if (characterStatsDictionary.TryGetValue(statType, out CharacterStat stat))
        {
            stat.RemoveAllModifiers();
        }
    }

    public void RemoveAllModifiersFromSource(object source)
    {
        foreach (CharacterStat stat in characterStatsDictionary.Values)
        {
            stat.RemoveAllModifiersFromSource(source);
        }
    }

    public float GetStatValue(StatType statType)
    {
        if (characterStatsDictionary.TryGetValue(statType, out CharacterStat stat))
        {
            return stat.Value;
        }
        return 0f;
    }
}
