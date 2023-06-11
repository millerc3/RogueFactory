using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveLoadSystem
{
    [System.Serializable]
    public class SaveData
    {
        // Inventory
        public List<string> CollectedItems;
        public SerializableDictionary<string, ItemPickupSaveData> ActiveItems;
        public SerializableDictionary<string, InventoryHolderSaveData> ChestDictionary;
        public PlayerInventoryHolderSaveData playerInventoryData;

        // Collection
        public CollectionSaveData PlayerCollectionData;
        public PlayerData PlayerData;

        // Factory Buildings
        public SerializableDictionary<Vector3Int, FactoryPlacementSaveData> FactoryPlacementSaveData;
        public SerializableDictionary<Vector3Int, StorageBoxSaveData> StorageBoxSaveDictionary;
        public SerializableDictionary<Vector3Int, BeltSaveData> BeltSaveDictionary;
        public SerializableDictionary<Vector3Int, ArmSaveData> ArmSaveDictionary;
        public SerializableDictionary<Vector3Int, ForgeSaveData> ForgeSaveDictionary;
        public SerializableDictionary<Vector3Int, DispenserSaveData> DispenserSaveDictionary;

        public SaveData()
        {
            // Inventory
            CollectedItems = new List<string>();
            ActiveItems = new SerializableDictionary<string, ItemPickupSaveData>();
            ChestDictionary = new SerializableDictionary<string, InventoryHolderSaveData>();

            // Player Data
            PlayerData = new PlayerData();

            // Factory Builings
            FactoryPlacementSaveData = new SerializableDictionary<Vector3Int, FactoryPlacementSaveData>();
            StorageBoxSaveDictionary = new SerializableDictionary<Vector3Int, StorageBoxSaveData>();
            BeltSaveDictionary = new SerializableDictionary<Vector3Int, BeltSaveData>();
            ArmSaveDictionary = new SerializableDictionary<Vector3Int, ArmSaveData>();
            ForgeSaveDictionary = new SerializableDictionary<Vector3Int, ForgeSaveData>();
            DispenserSaveDictionary = new SerializableDictionary<Vector3Int, DispenserSaveData>();
        }
    }
}

