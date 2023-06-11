using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameTest : MonoBehaviour
{
    public void SaveGame()
    {
        SaveGameManager.SaveGame();
    }

    public void LoadGame()
    {
        SaveGameManager.LoadGame();
    }
}
