using SaveLoadSystem;
using System;
using UnityEngine;

public class ArmBuilding : FactoryBuilding
{
    private InventorySlot inventorySlot;
    private int maxInventoryStackSize = 1;

    private GameObject heldWorldObject;

    private FactoryBuilding fromBuilding;
    private Vector3Int backwardGrid;
    private FactoryBuilding toBuilding;
    private Vector3Int forwardGrid;

    [SerializeField] private int ticksToProcess = 20;
    private float processingTimer, timeToProcess;
    private int tickTimer = 0;

    [SerializeField] private Transform armClawGrabPoint;
    [SerializeField] private Transform modelTransform;

    [SerializeField] private Animator animator;

    private ArmSaveData armSaveData;

    protected override void Awake()
    {
        base.Awake();

        inventorySlot = new InventorySlot();
        armSaveData = new ArmSaveData(inventorySlot);
    }

    protected override void Start()
    {
        base.Start();

        timeToProcess = (ticksToProcess - 1) * FactoryManager.instance.GetTimePerTick();

        Vector3Int transformForwardInt = new Vector3Int((int)transform.forward.x, (int)transform.forward.y, (int)transform.forward.z);
        forwardGrid = Origin + transformForwardInt;
        backwardGrid = Origin - transformForwardInt;

        LocalUpdate(0);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTick()
    {
        base.OnTick();

        if (fromBuilding == null && toBuilding == null) return;

        if (IsHoldingItem())
        {
            if (tickTimer >= ticksToProcess)
            {
                PlaceItemIntoOutputBuilding();
            }
        }
        else
        {
            PickupItemFromInputBuilding();
            tickTimer = 0;
        }

        tickTimer += 1;
    }

    private void PickupItemFromInputBuilding()
    {
        if (fromBuilding == null || !fromBuilding.CanRemoveFromOutput(out int amountThatCanBeRemoved)) return;

        int removedCount = fromBuilding.RemoveFromOutput(maxInventoryStackSize, out InventoryItemData removedItem, out GameObject removedWorldObject);
        if (removedCount <= 0) return;        

        AddToInput(removedItem, removedCount, removedWorldObject);
        animator.SetTrigger("trRotateTowardsToBuilding");
    }

    private void PlaceItemIntoOutputBuilding()
    {
        if (toBuilding == null || !toBuilding.CanAddToInput(inventorySlot.ItemData, inventorySlot.StackSize, out int amountOfItemsThatCanBeAdded)) return;

        int addedItems = toBuilding.AddToInput(inventorySlot.ItemData, inventorySlot.StackSize, heldWorldObject);
        if (addedItems <= 0) return;

        inventorySlot.RemoveFromStack(addedItems);

        if (inventorySlot.ItemData == null || inventorySlot.StackSize == 0)
        {
            heldWorldObject = null;
        }
        animator.SetTrigger("trRotateTowardsFromBuilding");
    }

    public override int AddToInput(InventoryItemData item, int count, GameObject worldObject)
    {
        inventorySlot.UpdateInventorySlot(item, maxInventoryStackSize);
        heldWorldObject = worldObject;
        
        if (heldWorldObject == null)
        {
            heldWorldObject = Instantiate(item.Prefab);
        }
        heldWorldObject.transform.position = armClawGrabPoint.position;
        heldWorldObject.transform.SetParent(armClawGrabPoint);

        return maxInventoryStackSize;
    }

    public override bool CanAddToInput(InventoryItemData item, int count, out int AmountOfItemsThatCanBeAdded)
    {
        // Nothing can add items to the arm
        AmountOfItemsThatCanBeAdded = 0;
        return false;
        //if (inventorySlot.ItemData == null || inventorySlot.StackSize < maxInventoryStackSize)
        //{
        //    AmountOfItemsThatCanBeAdded = maxInventoryStackSize - inventorySlot.StackSize;
        //    return true;
        //}
        //AmountOfItemsThatCanBeAdded = 0;
        //return false;
    }

    public override bool CanRemoveFromOutput(out int count)
    {
        // Nothing can remove the items from the arm
        count = -1;
        return false;
        //return count <= inventorySlot.StackSize;
    }

    public override void CheckNeighbors()
    {
        adjacentBuildings.Clear();

        FactoryPlacementData pd = buildingManager.GetStructureAt(forwardGrid);
        if (pd != null)
        {
            toBuilding = pd.BuildingGameObject.GetComponent<FactoryBuilding>();
            if (toBuilding != null) adjacentBuildings.Add(toBuilding);
        }

        pd = buildingManager.GetStructureAt(backwardGrid);
        if (pd != null)
        {
            fromBuilding = pd.BuildingGameObject.GetComponent<FactoryBuilding>();
            if (fromBuilding != null) adjacentBuildings.Add(fromBuilding);
        }
    }

    public override void LocalUpdate(int depth)
    {
        if (depth > 1) return;

        CheckNeighbors();

        UpdateNeighbors(++depth);
    }

    public override int RemoveFromOutput(int count, out InventoryItemData removedItem, out GameObject worldObject)
    {
        int removedItems = count;
        removedItem = null;
        worldObject = heldWorldObject;
        if (count >= inventorySlot.StackSize)
        {
            removedItems = inventorySlot.StackSize;
            removedItem = inventorySlot.ItemData;
        }
        inventorySlot.RemoveFromStack(removedItems);

        return removedItems;
    }

    protected override void TearDown()
    {
        // TODO put numberUnableToAdd onto ground or something
        playerInventoryHolder.AddToInventory(inventorySlot.ItemData, inventorySlot.StackSize, out int numberUnableToAdd);

        base.TearDown();
    }

    private bool IsHoldingItem()
    {
        return inventorySlot.ItemData != null && inventorySlot.StackSize > 0;
    }

    public override void LoadData(SaveData saveData)
    {
        if (saveData.ArmSaveDictionary.TryGetValue(Origin, out ArmSaveData storedSaveData))
        {
            if (storedSaveData.InventorySlot.ItemData == null || storedSaveData.InventorySlot.StackSize == 0) return;

            AddToInput(storedSaveData.InventorySlot.ItemData, storedSaveData.InventorySlot.StackSize, null);
        }
    }

    protected override void SaveData(SaveData saveData)
    {
        if (saveData.ArmSaveDictionary.TryGetValue(Origin, out ArmSaveData storedSaveData))
        {
            if (inventorySlot.ItemData == null || inventorySlot.StackSize == 0)
            {
                storedSaveData.InventorySlot.ClearSlot();
            }
            else
            {
                storedSaveData.InventorySlot.UpdateInventorySlot(inventorySlot.ItemData, inventorySlot.StackSize);
            }
        }
        else
        {
            saveData.ArmSaveDictionary.TryAdd(Origin, armSaveData);
        }
    }
}

[Serializable]
public struct ArmSaveData
{
    public InventorySlot InventorySlot;

    public ArmSaveData(InventorySlot inventorySlot)
    {
        InventorySlot = inventorySlot;
    }
}