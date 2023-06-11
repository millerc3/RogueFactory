using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FactoryGridData
{
    Dictionary<Vector3Int, FactoryPlacementData> placedObjects = new();
    

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int Id, int placedObjectIndex, GameObject newBuildingGameObject)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        FactoryPlacementData placementData = new FactoryPlacementData(positionsToOccupy, Id, placedObjectIndex, newBuildingGameObject, gridPosition);

        foreach (var position in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(position))
            {
                throw new Exception($"Dictionary already contains this cell position: {position}");
            }
            placedObjects[position] = placementData;
        }
    }

    public void RemoveObjectAt(Vector3Int objectGridOrigin)
    {
        if (placedObjects.TryGetValue(objectGridOrigin, out var placementData))
        {
            GameObject.Destroy(placementData.BuildingGameObject);

            foreach (var position in placementData.occupiedPositions)
            {
                placedObjects.Remove(position);
            }
        }        
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x <  objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }
        }
        return true;
    }

    public FactoryPlacementData GetObjectAt(Vector3Int gridPosition)
    {
        placedObjects.TryGetValue(gridPosition, out var placementData);
        return placementData;
    }
}

public class FactoryPlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int Id { get; private set; }
    public int PlacedObjectIndex { get; private set; }
    public Vector3Int Origin { get; private set; }

    public GameObject BuildingGameObject { get; private set; }

    public FactoryPlacementData(List<Vector3Int> occupiedPositions, int id, int placedObjectIndex, GameObject buildingGameObject, Vector3Int origin)
    {
        this.occupiedPositions = occupiedPositions;
        Id = id;
        PlacedObjectIndex = placedObjectIndex;
        BuildingGameObject = buildingGameObject;
        Origin = origin;
    }
}

[Serializable]
public class FactoryPlacementSaveData
{
    public Vector3 Forward;

    public int BuildingId;

    public FactoryPlacementSaveData(Vector3 forward, int buildingId)
    {
        Forward = forward;
        BuildingId = buildingId;
    }
}
