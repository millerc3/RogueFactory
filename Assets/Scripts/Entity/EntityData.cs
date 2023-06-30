using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Entities/EntityData")]
public class EntityData : ScriptableObject
{
    public string Name;
    [TextArea(4,4)]
    public string Description;
    public EntityTeam_t Team;
}

[Flags]
public enum EntityTeam_t
{
    NULL = 0,
    PLAYER = 1,
    ROBOT = 2,
}
