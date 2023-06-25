using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Type", menuName = "StatType")]
public class StatType : ScriptableObject
{
    public new string name;
    public string description;
}
