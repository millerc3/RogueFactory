using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace SaveLoadSystem
{
    public static class SaveGameManager
    {
        public static SaveData CurrentSaveData = new SaveData();

        public const string SaveDirectory = "/SaveData/";
        public const string Filename = "SaveGame.sav";

        public static UnityAction<SaveData> PreSaveGameEvent;
        public static UnityAction PostSaveGameEvent;

        public static UnityAction PreLoadGameEvent;
        public static UnityAction<SaveData> PostLoadGameEvent;

        [Command("Save")]
        public static bool SaveGame()
        {
            PreSaveGameEvent?.Invoke(CurrentSaveData);

            var dir = Application.persistentDataPath + SaveDirectory;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string json = JsonUtility.ToJson(CurrentSaveData, true);
            File.WriteAllText(dir + Filename, json);

            GUIUtility.systemCopyBuffer = dir + Filename;

            PostSaveGameEvent?.Invoke();

            return true;
        }

        [Command("Load")]
        public static bool LoadGame()
        {
            PreLoadGameEvent?.Invoke();

            string fullPath = Application.persistentDataPath + SaveDirectory + Filename;
            SaveData tempData = new SaveData();
            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                tempData = JsonUtility.FromJson<SaveData>(json);
            }
            else
            {
                Debug.LogError($"Save file does not exist");
                return false;
            }

            CurrentSaveData = tempData;
            PostLoadGameEvent?.Invoke(tempData);
            return true;
        }

        public static void DeleteSaveData()
        {
            string fullPath = Application.persistentDataPath + SaveDirectory + Filename;
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
