using System;
using UnityEngine;

[System.Serializable]
[ExecuteInEditMode]
public class UniqueID : MonoBehaviour
{
    [SerializeField, ReadOnly] private string id = Guid.NewGuid().ToString();

    [SerializeField] private static SerializableDictionary<string, GameObject> idDatabase = new SerializableDictionary<string, GameObject>();

    public string Id => id;

    private void OnEnable()
    {
        if (idDatabase.ContainsKey(Id))
        {
            Generate();
        }
        else
        {
            idDatabase.Add(Id, gameObject);
        }
    }

    private void OnDestroy()
    {
        if (idDatabase.ContainsKey(Id)) idDatabase.Remove(Id);
    }

    private void Generate()
    {
        id = Guid.NewGuid().ToString();
        idDatabase.Add(Id, gameObject);
    }
}
