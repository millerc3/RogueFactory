using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallbackController : MonoBehaviour
{
    private bool isFirstUpdate = true;

    private void Update()
    {
        if (!isFirstUpdate) return;

        isFirstUpdate = false;
        SceneLoader.LoaderCallback();
    }
}
