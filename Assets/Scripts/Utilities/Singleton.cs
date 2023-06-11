using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    public static T instance;

    protected virtual void Awake()
    {
        if (instance != null)
        {
            string typename = typeof(T).Name;
            Debug.LogWarning($"More that one instance of {typename} found.");
        }
        instance = this as T;
    }
}