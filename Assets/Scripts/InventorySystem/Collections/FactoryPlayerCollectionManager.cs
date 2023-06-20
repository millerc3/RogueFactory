using QFSW.QC;
using QFSW.QC.Suggestors.Tags;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using SaveLoadSystem;

public class FactoryPlayerCollectionManager : CollectionManager
{

    protected override void OnEnable()
    {
        SaveGameManager.PostLoadGameEvent += LoadCollection;
        SaveGameManager.PreSaveGameEvent += SaveCollection;
    }

    protected override void OnDisable()
    {
        SaveGameManager.PostLoadGameEvent += LoadCollection;
        SaveGameManager.PreSaveGameEvent -= SaveCollection;
    }


    #region SAVE/LOAD SYSTEM

    public void LoadCollection(SaveData saveData)
    {
        Collection = saveData.PlayerCollectionData.Collection;
    }

    public void SaveCollection(SaveData saveData)
    {
        saveData.PlayerCollectionData.Collection = Collection;
    }

    #endregion
}

