using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] float previewYOffset = 0.05f;

    [SerializeField] private GameObject cellIndicator;
    private Transform previewObjectMovePoint;
    private GameObject previewObject;
    private FactoryBuilding previewBuilding;

    private Renderer cellRenderer;

    private Vector3 currentFacing = Vector3.forward;

    private void Start()
    {
        cellIndicator.SetActive(false);
        cellRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size, Vector3Int initialGridPosition)
    {
        cellIndicator.SetActive(true);
        previewObject = Instantiate(prefab, initialGridPosition, Quaternion.identity);
        previewBuilding = previewObject.GetComponent<FactoryBuilding>();
        previewObjectMovePoint = previewBuilding.RotatePivot;
        previewObjectMovePoint.parent = null;
        previewBuilding.transform.parent = previewObjectMovePoint;
        SetFacing(currentFacing);
        PrepareCursor(size);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x <= 0 || size.y <= 0) return;

        cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
        cellRenderer.sharedMaterial.mainTextureScale = size;
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObjectMovePoint != null) Destroy(previewObjectMovePoint.gameObject);
        else if (previewObject != null) Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        MovePreview(position);
        MoveCursor(position);
        ApplyFeedback(validity);
    }

    private void ApplyFeedback(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = .5f;
        
        cellRenderer.sharedMaterial.color = c;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        //previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
        previewObjectMovePoint.position = new Vector3(position.x + previewBuilding.BuildingData.Size.x/2f, 
                                                      position.y + previewYOffset,
                                                      position.z + previewBuilding.BuildingData.Size.y/2f);
    }

    public void SetFacing(Vector3 forward)
    {
        if (!previewBuilding) return;

        currentFacing = forward;
        //currentFacing = forward;
        ////previewObject.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        //FactoryBuilding building = previewObject.GetComponent<FactoryBuilding>();
        //if (building == null) 
        //{
        //    Debug.LogError($"There is no FactoryBuilding component on this object: {previewObject}");
        //}
        //building.RotateTowards(forward);
        previewObjectMovePoint.rotation = Quaternion.LookRotation(currentFacing, Vector3.up);
    }
}
