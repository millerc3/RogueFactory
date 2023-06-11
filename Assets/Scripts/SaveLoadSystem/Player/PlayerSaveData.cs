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
        saveData.PlayerData = playerData;
    }

    private void LoadPositionRotation(SaveData saveData)
    {
        playerData = saveData.PlayerData;
        playerCharacter.SetPosition(playerData.PlayerFactoryPosition);
        playerCharacter.SetRotation(playerData.PlayerFactoryRotation);
    }
}

[System.Serializable]
public struct PlayerData
{
    public Vector3 PlayerFactoryPosition;
    public Quaternion PlayerFactoryRotation;

    public PlayerData(Vector3 factoryPosition, Quaternion factoryRotation)
    {
        PlayerFactoryPosition = factoryPosition;
        PlayerFactoryRotation= factoryRotation;
    }
}