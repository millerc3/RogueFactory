using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private SceneLoader.Scene sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        SceneLoader.LoadScene(sceneToLoad);
    }
}
