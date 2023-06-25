using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatModType
{
    FLAT = 100,
    PERCENT_ADD = 200,
    PERCENT_MULTIPLY = 300,
}

[System.Serializable]
public class StatModifier
{
    public StatType StatType;
    public float Value;
    public StatModType ModType;
    public int Order;
    public object Source;

    public StatModifier(StatType statType, float value, StatModType modType, int order, object source)
    {
        StatType = statType;
        ModType = modType;
        Value = value;
        Order = order;
        Source = source;
    }

    public StatModifier(StatType statType, float value, StatModType modType) : this(statType, value, modType, (int)modType, null) { }
    public StatModifier(StatType statType, float value, StatModType modType, int order) : this(statType, value, modType, order, null) { }
    public StatModifier(StatType statType, float value, StatModType modType, object source) : this(statType, value, modType, (int)modType, source) { }
}
