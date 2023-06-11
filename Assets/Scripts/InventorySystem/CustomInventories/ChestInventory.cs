using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChestInventory : InventoryHolder, IInteractable
{
    [SerializeField] protected float interactionRange = 5f;
    private Transform interactorTransform;
    public UnityAction<IInteractable> OnInteractionComplete { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    protected override void Start()
    {
        InventoryHolderSaveData chestSaveData = new InventoryHolderSaveData(primaryInventorySystem, transform.position, transform.rotation);
        SaveGameManager.CurrentSaveData.ChestDictionary.Add(GetComponent<UniqueID>().Id, chestSaveData);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SaveGameManager.PostLoadGameEvent += LoadInventory;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SaveGameManager.PostLoadGameEvent -= LoadInventory;
    }

    protected override void Update()
    {
        base.Update();
        if (interactorTransform != null && !Interactor.InRangeOfInteractor(interactorTransform, transform, interactionRange)) {
            OnDynamicInventoryHideRequested?.Invoke(PrimaryInventorySystem);
        }
    }

    #region Interation System

    public void EndHover(GameObject interactor)
    {
        
    }

    public void TapInteract(GameObject interactor, out bool interactSuccessful)
    {
        interactorTransform = interactor.transform;
        OnDynamicInventoryDisplayRequested?.Invoke(PrimaryInventorySystem);

        interactSuccessful = true;
    }

    public void StartHover(GameObject interactor)
    {

    }

    #endregion

    #region Save System

    protected override void LoadInventory(SaveData saveData)
    {
        if (saveData.ChestDictionary.TryGetValue(GetComponent<UniqueID>().Id, out InventoryHolderSaveData chestSaveData))
        {
            primaryInventorySystem = chestSaveData.InventorySystem;
            transform.position = chestSaveData.Position;
            transform.rotation = chestSaveData.Rotation;
        }
    }

    public void HoldInteract(GameObject interactor, out bool interactSuccessful)
    {
        interactSuccessful = true;
    }

    public void StopHoldInteract(GameObject interactor)
    {

    }

    #endregion
}

