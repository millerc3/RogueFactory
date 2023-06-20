using SaveLoadSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInventoryHolder : InventoryHolder
{
    [SerializeField] protected int secondaryInventorySize;
    [SerializeField] protected InventorySystem secondaryInventorySystem;

    public InventorySystem SecondaryInventorySystem => secondaryInventorySystem;

    public static UnityAction<InventorySystem> OnPlayerBackpackDisplayRequested;
    public static UnityAction<InventorySystem> OnPlayerHotbarDisplayRequested;

    public static UnityAction OnPlayerBackpackChanged;
    public static UnityAction OnPlayerHotbarChanged;

    protected override void Awake()
    {
        base.Awake();

        secondaryInventorySystem = new InventorySystem(secondaryInventorySize);
    }

    protected override void Start()
    {
        base.Start();
        PlayerInventoryHolderSaveData playerInventorySaveData = new PlayerInventoryHolderSaveData(primaryInventorySystem, secondaryInventorySystem);
        SaveGameManager.CurrentSaveData.playerInventoryData = playerInventorySaveData;
    }

    protected override void Update()
    {
        base.Update();
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            OnPlayerBackpackDisplayRequested?.Invoke(secondaryInventorySystem);
        }
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

    public override bool AddToInventory(InventoryItemData data, int amount, out int numberUnableToAdd)
    {
        numberUnableToAdd = -1;
        // TODO: should this be true or false?
        if (data == null) return true;

        if (primaryInventorySystem.AddItemToInventory(data, amount, out numberUnableToAdd))
        {
            return true;
        }
        
        if (secondaryInventorySystem.AddItemToInventory(data, numberUnableToAdd, out numberUnableToAdd))
        {
            return true;
        }

        return false;
    }

    #region Save System

    protected override void LoadInventory(SaveData saveData)
    {
        if (saveData.playerInventoryData.PrimaryInventorySystem != null)
        {
            // If we don't have any data stored
            if (saveData.playerInventoryData.PrimaryInventorySystem.InventorySlots.Count == 0)
            {
                // put our current (presumably empty) inventory into the save data object
                saveData.playerInventoryData.PrimaryInventorySystem = primaryInventorySystem;
            }
            else
            {
                // otherwise, grab the stored data and put it in our inventory 
                primaryInventorySystem = saveData.playerInventoryData.PrimaryInventorySystem;
            }

            OnPlayerHotbarChanged?.Invoke();
        }
        if (saveData.playerInventoryData.SecondaryInventorySystem != null)
        {
            //secondaryInventorySystem = saveData.playerInventoryData.SecondaryInventorySystem;

            if (saveData.playerInventoryData.PrimaryInventorySystem.InventorySlots.Count == 0)
            {
                saveData.playerInventoryData.SecondaryInventorySystem = secondaryInventorySystem;
            }
            else
            {
                primaryInventorySystem = saveData.playerInventoryData.PrimaryInventorySystem;
            }

            OnPlayerBackpackChanged?.Invoke();
        }
    }

    #endregion
}


[System.Serializable]
public struct PlayerInventoryHolderSaveData
{
    public InventorySystem PrimaryInventorySystem;
    public InventorySystem SecondaryInventorySystem;

    public PlayerInventoryHolderSaveData(InventorySystem primaryInventorySystem, InventorySystem secondaryInventorySystem)
    {
        PrimaryInventorySystem = primaryInventorySystem;
        SecondaryInventorySystem = secondaryInventorySystem;
    }
}