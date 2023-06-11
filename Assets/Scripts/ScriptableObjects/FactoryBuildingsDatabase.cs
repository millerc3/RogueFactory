using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Factory Buildings/Building Database")]
public class FactoryBuildingsDatabase : ScriptableObject
{
    //public List<FactoryBuildingData> factoryBuildings;
    public List<FactoryBuildingData> BuildingDatabase;

    [ContextMenu("Set IDs")]
    public void SetItemIDs()
    {
        BuildingDatabase = new List<FactoryBuildingData>();

        // Get all FactoryBuildingData instances in "FactoryBuildingData" folder
        var foundBuildings = Resources.LoadAll<FactoryBuildingData>("FactoryBulidingData").OrderBy(i => i.Id).ToList();

        // Get all of the found items where the id IS NOT greater than the size of the foundBuildings list
        var hasIdInRange = foundBuildings.Where(i => i.Id != -1 && i.Id < foundBuildings.Count).OrderBy(i => i.Id).ToList();

        // Get all of the found buildings where the id IS greater than the size of the foundBuildings list
        var hasIdNotInRange = foundBuildings.Where(i => i.Id != -1 && i.Id >= foundBuildings.Count).OrderBy(i => i.Id).ToList();

        // Get all of the found buildings that do not yet have an ID set (-1)
        var hasNoId = foundBuildings.Where(i => i.Id == -1).ToList();

        var duplicates = foundBuildings.GroupBy(i => i.Id).Where(i => i.Count() > 1);
        foreach (var duplicate in duplicates)
        {
            if (duplicate.Key == -1) continue;

            Debug.LogWarning($"There are multiple items with ID {duplicate.Key}");
            var repeats = foundBuildings.Where(i => i.Id == duplicate.Key).ToList();
            foreach (var i in repeats)
            {
                Debug.LogWarning($"    * {i.Name}");
            }
        }

        int index = 0;
        for (int i = 0; i < foundBuildings.Count; i++)
        {
            FactoryBuildingData buildingToAdd;

            buildingToAdd = hasIdInRange.Find(d => d.Id == i);
            if (buildingToAdd != null)
            {
                BuildingDatabase.Add(buildingToAdd);
            }
            else if (index < hasNoId.Count)
            {
                hasNoId[index].Id = i;
                buildingToAdd = hasNoId[index];
                index++;
                BuildingDatabase.Add(buildingToAdd);
            }
#if UNITY_EDITOR
            if (buildingToAdd) EditorUtility.SetDirty(buildingToAdd);
#endif
        }

#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif
    }

    public FactoryBuildingData GetBuliding(int id)
    {
        return BuildingDatabase.Find(i => i.Id == id);
    }
}

