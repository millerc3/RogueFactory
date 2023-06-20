using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField] private ImageProgressBar sceneLoadingProgressBar;

    private void Update()
    {
        sceneLoadingProgressBar.SetProgress(SceneLoader.GetLoadingProgress());
    }
}
