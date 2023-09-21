using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneOnTriggerEnter : MonoBehaviour
{
    [SerializeField] protected SceneLoader.Scene sceneToLoad;

    protected virtual void OnTriggerEnter(Collider other)
    {
        SceneLoader.LoadScene(sceneToLoad);
    }
}
