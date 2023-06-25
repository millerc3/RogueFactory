using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class ObjectDatabase : ScriptableObject
{
    public string ObjectResourceFolderName = "NULL";
    public List<DatabaseObject> Objects;

    [ContextMenu("Validate")]
    public bool Validate()
    {
        var foundObjects = Resources.LoadAll<DatabaseObject>(ObjectResourceFolderName).OrderBy(i => i.Id).ToList();

        if (foundObjects.Count == 0)
        {
            Debug.LogError($"Invalid Folder Structure for ObjectDatabse!");
            Debug.LogError($"There were no DatabaseObjects found in Resources/{ObjectResourceFolderName}");
            Debug.LogError($"  - Please ensure a folder named {ObjectResourceFolderName} inside of a Resources folder exists.");
            Debug.LogError($"  - This folder should contain all of the DatabaseObject to be contained by this database.");

            return false;
        }

        Debug.Log($"Found {foundObjects.Count} objects in Resources/{ObjectResourceFolderName} folder");
        return true;
    }

    [ContextMenu("Set IDs")]
    public void SetObjectIDs()
    {
        if (!Validate())
        {
            Debug.LogError("Unable to set IDs - Please fix validation errors");
        }

        Objects = new List<DatabaseObject>();

        // Get all object instances in the Resources folder named after this object
        var foundObjects = Resources.LoadAll<DatabaseObject>(ObjectResourceFolderName).OrderBy(i => i.Id).ToList();

        // Get all of the found objects where the ID is NOT greater than the size of the foundObjects list
        var hasIdInRange = foundObjects.Where(i => i.Id != -1 && i.Id < foundObjects.Count).OrderBy(i => i.Id).ToList();

        // Get all of the found objects where the ID IS greater than the size of the foundObjects list
        var hasIdNotInRange = foundObjects.Where(i => i.Id != -1 && i.Id >= foundObjects.Count).OrderBy(i => i.Id).ToList();

        // Get all of the found objects that do not yet have an ID set (ID == -1)
        var hasNoId = foundObjects.Where(i => i.Id == -1).ToList();

        var duplicates = foundObjects.GroupBy(i => i.Id).Where(i => i.Count() > 1);
        foreach (var duplicate in duplicates)
        {
            if (duplicate.Key == -1) continue;

            Debug.LogWarning($"There are multiple items with ID {duplicate.Key}");
        }

        int index = 0;
        for (int i = 0; i < foundObjects.Count; i++)
        {
            DatabaseObject objecToAdd;

            objecToAdd = hasIdInRange.Find(d => d.Id == i);
            if (objecToAdd != null)
            {
                Objects.Add(objecToAdd);
            }
            else if (index < hasNoId.Count)
            {
                hasNoId[index].Id = i;
                objecToAdd = hasNoId[index];
                index++;
                Objects.Add(objecToAdd);
            }
#if UNITY_EDITOR
            if (objecToAdd) EditorUtility.SetDirty(objecToAdd);
#endif
        }
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif
    }
}

public abstract class DatabaseObject : ScriptableObject
{
    public int Id = -1;
}