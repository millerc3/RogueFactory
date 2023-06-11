using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingSystemManager : MonoBehaviour
{
    [SerializeField] private FactoryBuildingsDatabase factoryBuildingsDB;
    private List<FactoryBuildingData> placedBuildings = new List<FactoryBuildingData>();
    SerializableDictionary<Vector3Int, FactoryPlacementSaveData> placementSaveData = new();

    [SerializeField] private Transform floorParent;

    private FactoryGridData factoryGridData;

    private Grid grid;
    private Vector3Int currSize = Vector3Int.one * 2;

    private void Awake()
    {
        factoryGridData = new FactoryGridData();
    }

    private void OnEnable()
    {
        SaveGameManager.PreSaveGameEvent += StorePlacementData;
        SaveGameManager.PostLoadGameEvent += LoadPlacementData;
    }

    private void OnDisable()
    {
        SaveGameManager.PreSaveGameEvent -= StorePlacementData;
        SaveGameManager.PostLoadGameEvent -= LoadPlacementData;
    }

    private void Start()
    {
        grid = FindObjectOfType<Grid>();
        if (grid == null)
        {
            Debug.LogError("No Grid!");
        }
    }

    // Note: This needs to be of EVEN NUMBERS ONLY - odd numbers wont have the grid aligned properly
    public void UpdateSize(Vector3Int size)
    {
        floorParent.localScale = (Vector3)size / 10f;
    }

    public bool CheckPlacementValidity(Vector3Int gridPosition, int selectedBuildingIndex)
    {
        return factoryGridData.CanPlaceObjectAt(gridPosition, factoryBuildingsDB.BuildingDatabase[selectedBuildingIndex].Size);
    }

    public bool IsGridPositionOpen(Vector3Int gridPosition)
    {
        return factoryGridData.CanPlaceObjectAt(gridPosition, Vector2Int.one);
    }

    public bool PlaceStructureAt(Vector3Int gridPosition, int structureId, Vector3 forward)
    {
        return PlaceStructureAt(gridPosition, factoryBuildingsDB.BuildingDatabase[structureId], forward);
    }

    public bool PlaceStructureAt(Vector3Int gridPosition, FactoryBuildingData factoryBuilding, Vector3 forward)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, factoryBuilding.Id);
        if (!placementValidity)
        {
            return false;
        }

        GameObject newObject = Instantiate(factoryBuilding.Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);

        placedBuildings.Add(factoryBuilding);
        FactoryBuilding newBuilding = newObject.GetComponent<FactoryBuilding>();
        RotateFactoryBuilding(newBuilding, forward);
        newBuilding.BuildingData = factoryBuilding;
        newBuilding.PlacementId = placedBuildings.Count;
        newBuilding.Origin = gridPosition;
        factoryGridData.AddObjectAt(gridPosition, factoryBuilding.Size, factoryBuilding.Id, placedBuildings.Count, newObject);
        newBuilding.placementData = factoryGridData.GetObjectAt(gridPosition);

        placementSaveData.TryAdd(newBuilding.Origin, new FactoryPlacementSaveData(forward, factoryBuilding.Id));

        return true;
    }

    public void RemoveStructureAt(Vector3Int gridPosition)
    {
        bool gridEmpty = IsGridPositionOpen(gridPosition);
        if (gridEmpty)
        {
            return;
        }

        FactoryPlacementData pd = factoryGridData.GetObjectAt(gridPosition);
        placementSaveData.Remove(pd.Origin);

        factoryGridData.RemoveObjectAt(gridPosition);
    }

    public FactoryPlacementData GetStructureAt(Vector3Int gridPosition)
    {
        return factoryGridData.GetObjectAt(gridPosition);
    }

    private void RotateFactoryBuilding(FactoryBuilding building, Vector3 forward)
    {
        building.RotatePivot.parent = null;
        building.transform.parent = building.RotatePivot;
        building.RotatePivot.rotation = Quaternion.LookRotation(forward, Vector3.up);
        building.transform.parent = null;
        building.RotatePivot.parent = building.transform;
    }

    private void StorePlacementData(SaveData saveData)
    {
        saveData.FactoryPlacementSaveData = placementSaveData;
    }

    private void LoadPlacementData(SaveData saveData)
    {
        var tmPlacementSaveData = saveData.FactoryPlacementSaveData;

        foreach (KeyValuePair<Vector3Int, FactoryPlacementSaveData> pdData in tmPlacementSaveData)
        {
            PlaceStructureAt(pdData.Key, pdData.Value.BuildingId, pdData.Value.Forward);

            GetStructureAt(pdData.Key).BuildingGameObject.GetComponent<FactoryBuilding>().LoadData(saveData);
        }


    }
}
