using SaveLoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Belt : FactoryBuilding
{
    [SerializeField] private InventoryItemData itemToAdd;

    private InventorySlot inventorySlot;
    private int maxInventoryStackSize = 1;

    private Belt forwardBelt;
    private GameObject heldWorldObject = null;

    private List<Vector3Int> neighborGrids = new List<Vector3Int>();

    [Tooltip("The number of ticks to move the item across the belt")]
    [SerializeField] private int ticksToProcess = 5;
    private float processingTimer, timeToProcess;
    private int tickTimer = 0;
    private int testTickTimer = 0;

    private BeltSaveData beltSaveData;

    protected override void Awake()
    {
        base.Awake();

        inventorySlot = new InventorySlot();
        beltSaveData = new BeltSaveData(inventorySlot);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void Start()
    {
        base.Start();

        timeToProcess = (ticksToProcess - 1) * FactoryManager.instance.GetTimePerTick();

        neighborGrids.Add(Origin + Vector3Int.forward);
        neighborGrids.Add(Origin - Vector3Int.forward);
        neighborGrids.Add(Origin + Vector3Int.right);
        neighborGrids.Add(Origin - Vector3Int.right);

        LocalUpdate(0);
    }

    protected override void Update()
    {
        base.Update();

        if (inventorySlot.ItemData == null || inventorySlot.StackSize <= 0) return;
        if (forwardBelt == null || !forwardBelt.CanAddToInput(inventorySlot.ItemData, maxInventoryStackSize, out _)) return;

        heldWorldObject.transform.position = Vector3.Lerp(RotatePivot.position + Vector3.up,
                                                          forwardBelt.RotatePivot.position + Vector3.up,
                                                          processingTimer / timeToProcess);
        processingTimer += Time.deltaTime;
    }

    protected override void OnTick()
    {
        testTickTimer += 1;
        base.OnTick();

        if (forwardBelt == null || !forwardBelt.CanAddToInput(inventorySlot.ItemData, maxInventoryStackSize, out int _)) return;

        if (itemToAdd != null && testTickTimer % 15 == 0)
        {
            TestAddToInventory();
        }

        if (inventorySlot.ItemData == null) return;

        tickTimer += 1;

        if (tickTimer >= ticksToProcess)
        {
            PushToNextBelt();
            tickTimer = 0;
        }
    }

    protected override void TearDown()
    {
        // TODO Put numberUnableToAdd onto the ground or something
        playerInventoryHolder.AddToInventory(inventorySlot.ItemData, inventorySlot.StackSize, out int numberUnableToAdd);

        base.TearDown();
    }

    private void TestAddToInventory()
    {
        if (inventorySlot.ItemData != null) return;
        AddToInput(itemToAdd, maxInventoryStackSize, null);
    }

    public override void LocalUpdate(int depth)
    {
        if (depth > 1) return;

        CheckNeighbors();
        SetForwardBelt();

        UpdateNeighbors(++depth);
    }

    private void PushToNextBelt()
    {
        forwardBelt.AddToInput(inventorySlot.ItemData, maxInventoryStackSize, heldWorldObject);
        RemoveFromOutput(maxInventoryStackSize, out _, out _);
    }

    public override int AddToInput(InventoryItemData item, int count, GameObject worldObject)
    {
        inventorySlot.UpdateInventorySlot(item, maxInventoryStackSize);
        heldWorldObject = worldObject;

        if (heldWorldObject == null)
        {
            heldWorldObject = Instantiate(item.Prefab);
        }
        heldWorldObject.transform.position = RotatePivot.position + Vector3.up;
        heldWorldObject.transform.SetParent(transform);

        processingTimer = 0;

        return maxInventoryStackSize;
    }

    public override bool CanAddToInput(InventoryItemData item, int count, out int AmountOfItemsThatCanBeAdded)
    {
        if (inventorySlot.ItemData == null || inventorySlot.StackSize < maxInventoryStackSize)
        {
            AmountOfItemsThatCanBeAdded = maxInventoryStackSize - inventorySlot.StackSize;
            return true;
        }
        AmountOfItemsThatCanBeAdded = 0;
        return false;
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

    public override bool CanRemoveFromOutput(out int amountThatCanBeRemoved)
    {
        amountThatCanBeRemoved = inventorySlot.StackSize;
        return amountThatCanBeRemoved > 0;
    }

    public override void CheckNeighbors()
    {
        adjacentBuildings.Clear();
        foreach (Vector3Int grid in neighborGrids)
        {
            FactoryPlacementData neighborPlacementData = buildingManager.GetStructureAt(grid);
            if (neighborPlacementData == null) continue;

            FactoryBuilding neighborBuilding = neighborPlacementData.BuildingGameObject.GetComponent<FactoryBuilding>();
            if (neighborBuilding == null) Debug.LogError($"There is no game object for this stored placement data");

            adjacentBuildings.Add(neighborBuilding);
        }
    }

    private void SetForwardBelt()
    {
        Vector3Int localForwardGrid = Origin + new Vector3Int((int)transform.forward.x,
                                                              (int)transform.forward.y,
                                                              (int)transform.forward.z);

        FactoryPlacementData forwardPlacementData = buildingManager.GetStructureAt(localForwardGrid);
        if (forwardPlacementData == null) return;

        forwardBelt = forwardPlacementData.BuildingGameObject.GetComponent<Belt>();
    }

    public override void LoadData(SaveData saveData)
    {
        if (saveData.BeltSaveDictionary.TryGetValue(Origin, out BeltSaveData storedSaveData))
        {
            if (storedSaveData.InventorySlot.ItemData == null || storedSaveData.InventorySlot.StackSize == 0) return;

            AddToInput(storedSaveData.InventorySlot.ItemData, storedSaveData.InventorySlot.StackSize, null);
        }
    }

    protected override void SaveData(SaveData saveData)
    {
        if (saveData.BeltSaveDictionary.TryGetValue(Origin, out BeltSaveData storedSaveData))
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
            saveData.BeltSaveDictionary.TryAdd(Origin, beltSaveData);
        }
    }
}

[Serializable]
public struct BeltSaveData
{
    public InventorySlot InventorySlot;

    public BeltSaveData(InventorySlot inventorySlot)
    {
        InventorySlot = inventorySlot;
    }
}