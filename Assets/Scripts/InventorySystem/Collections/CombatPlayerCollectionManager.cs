using QFSW.QC;
using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPlayerCollectionManager : CollectionManager
{
    [Command]
    public void AddToSavedCollection()
    {
        CollectionSaveData storedData = SaveGameManager.CurrentSaveData.PlayerCollectionData;

        storedData.Collection.CombineWith(Collection);

        SaveGameManager.SaveGame();
    }
}
