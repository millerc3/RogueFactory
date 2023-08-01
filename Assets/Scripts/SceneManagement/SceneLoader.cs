using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public class LoadingMonoBehaviour : MonoBehaviour { }

    public enum Scene
    {
        FactoryTest,
        MainMenu,
        TPSTesting,
        LoadingScreen,
        LargeWorldTest,
        ShipScene,
    }

    private static Action OnLoaderCallback;
    private static AsyncOperation loadingAasyncOperation;

    [Command]
    public static void LoadScene(Scene scene)
    {
        // Setup the loader callback that will be called by a the LoaderCallbackController 
        //  which is owned by a game object in the LoadingScreen Scene
        OnLoaderCallback = () =>
        {
            GameObject loadingGameObject = new GameObject("Loading Game Object");
            loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));
        };

        // Load the LoadingScreen scene
        SceneManager.LoadScene(Scene.LoadingScreen.ToString());
    }

    private static IEnumerator LoadSceneAsync(Scene scene)
    {
        yield return null;

        loadingAasyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

        while (!loadingAasyncOperation.isDone)
        {
            yield return null;
        }
    }

    public static float GetLoadingProgress()
    {
        if (loadingAasyncOperation != null)
        {
            return loadingAasyncOperation.progress;
        }

        return 0f;
    }

    public static void LoaderCallback()
    {
        if (OnLoaderCallback == null) return;

        OnLoaderCallback();
        OnLoaderCallback = null;
    }
}
