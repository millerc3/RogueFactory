using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Factory Buildings/Building Database")]
public class FactoryBuildingsDatabase : ObjectDatabase
{
    public FactoryBuildingData GetBuliding(int id)
    {
        return Objects.Find(i => i.Id == id) as FactoryBuildingData;
    }
}

