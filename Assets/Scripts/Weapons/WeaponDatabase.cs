using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Weapons/Weapon Database")]
public class WeaponDatabase : ScriptableObject
{
    public List<WeaponData> Weapons;

    [ContextMenu("Set IDs")]
    public void SetItemIDs()
    {
        Weapons = new List<WeaponData>();

        // Get all WeaponData instances in "WeaponData" folder
        var foundWeapons = Resources.LoadAll<WeaponData>("WeaponData").OrderBy(i => i.Id).ToList();

        // Get all of the found items where the id IS NOT greater than the size of the foundWeapons list
        var hasIdInRange = foundWeapons.Where(i => i.Id != -1 && i.Id < foundWeapons.Count).OrderBy(i => i.Id).ToList();

        // Get all of the found weapons where the id IS greater than the size of the foundWeapons list
        var hasIdNotInRange = foundWeapons.Where(i => i.Id != -1 && i.Id >= foundWeapons.Count).OrderBy(i => i.Id).ToList();

        // Get all of the found weapons that do not yet have an ID set (-1)
        var hasNoId = foundWeapons.Where(i => i.Id == -1).ToList();

        var duplicates = foundWeapons.GroupBy(i => i.Id).Where(i => i.Count() > 1);
        foreach (var duplicate in duplicates)
        {
            if (duplicate.Key == -1) continue;

            Debug.LogWarning($"There are multiple items with ID {duplicate.Key}");
            var repeats = foundWeapons.Where(i => i.Id == duplicate.Key).ToList();
            foreach (var i in repeats)
            {
                Debug.LogWarning($"    * {i.Name}");
            }
        }

        int index = 0;
        for (int i = 0; i < foundWeapons.Count; i++)
        {
            WeaponData weaponToAdd;

            weaponToAdd = hasIdInRange.Find(d => d.Id == i);
            if (weaponToAdd != null)
            {
                Weapons.Add(weaponToAdd);
            }
            else if (index < hasNoId.Count)
            {
                hasNoId[index].Id = i;
                weaponToAdd = hasNoId[index];
                index++;
                Weapons.Add(weaponToAdd);
            }
#if UNITY_EDITOR
            if (weaponToAdd) EditorUtility.SetDirty(weaponToAdd);
#endif
        }
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif
    }

    public WeaponData GetWeapon(int id)
    {
        return Weapons.Find(i => i.Id == id);
    }
}
