using EasyCharacterMovement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoadSceneOnTriggerEnter : LoadSceneOnTriggerEnter
{
    [SerializeField] private Vector3 positionToMovePlayerBeforeSave = Vector3.zero;
    [SerializeField] private ThirdPersonCharacter playerCharacter;

    protected override void OnTriggerEnter(Collider other)
    {
        playerCharacter.SetPosition(positionToMovePlayerBeforeSave);
        playerCharacter.SetRotation(Quaternion.LookRotation(Vector3.forward));

        SaveLoadSystem.SaveGameManager.SaveGame();

        base.OnTriggerEnter(other);
    }
}
