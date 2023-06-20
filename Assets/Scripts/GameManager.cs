using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();

        if (gameObject == null) return;

        DontDestroyOnLoad(gameObject);

        if (!SaveLoadSystem.SaveGameManager.LoadGame())
        {
            SaveLoadSystem.SaveGameManager.SaveGame();
        }
    }

    protected override void OnAnotherInstanceFound()
    {
        base.OnAnotherInstanceFound();

        Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SaveGameManager.LoadGame();
    }
}
