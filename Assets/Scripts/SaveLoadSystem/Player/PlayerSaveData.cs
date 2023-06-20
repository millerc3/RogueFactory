using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveLoadSystem;
using EasyCharacterMovement;

public class PlayerSaveData : MonoBehaviour
{
    [SerializeField] private EasyCharacterMovement.Character playerCharacter;
    private PlayerData playerData = new PlayerData();

    private void OnEnable()
    {
        SaveGameManager.PreSaveGameEvent += StorePositionRotation;
        SaveGameManager.PostLoadGameEvent += LoadPositionRotation;
    }

    private void OnDisable()
    {
        SaveGameManager.PreSaveGameEvent -= StorePositionRotation;
        SaveGameManager.PostLoadGameEvent -= LoadPositionRotation;
    }

    private void StorePositionRotation(SaveData saveData)
    {
        playerData.PlayerFactoryPosition = playerCharacter.GetPosition();
        playerData.PlayerFactoryRotation = playerCharacter.GetRotation();
        playerData.ValidData = true;
        saveData.PlayerData = playerData;
    }

    private void LoadPositionRotation(SaveData saveData)
    {
        playerData = saveData.PlayerData;

        if (!playerData.ValidData) return;

        playerCharacter.SetPosition(playerData.PlayerFactoryPosition);
        playerCharacter.SetRotation(playerData.PlayerFactoryRotation);
    }
}

[System.Serializable]
public struct PlayerData
{
    public bool ValidData;
    public Vector3 PlayerFactoryPosition;
    public Quaternion PlayerFactoryRotation;

    public PlayerData(bool validData, Vector3 factoryPosition, Quaternion factoryRotation)
    {
        ValidData = validData;
        PlayerFactoryPosition = factoryPosition;
        PlayerFactoryRotation= factoryRotation;
    }
}