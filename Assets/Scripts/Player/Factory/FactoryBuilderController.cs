using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FactoryBuilderController : MonoBehaviour
{
    private PlayerInputActions inputActions = null;

    [SerializeField] private PreviewSystem previewSystem;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask placementLayerMask;
    private Vector3 lastPositionOfCursor;
    private Vector3Int lastGridPositionOfCursor;
    private Vector3 lastPositionForPreview = Vector3.zero;
    private Vector3 lastPositionForEdit = Vector3.zero;

    private Grid grid;

    private int selectedBuildingIndex = -1;
    [SerializeField] private BuildingSystemManager buildingManager;
    [SerializeField] private FactoryBuildingsDatabase factoryBuildingsDB;
    [SerializeField] private GameObject gridEffect;

    private int buildingFacingIndex = 0;
    private Vector3[] buildingFacings = { Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.right };

    public bool IsPlacing = false, IsRemoving = false;

    private bool isPointerOverUI = false;

    private MouseItemData mouseItemData;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        mouseItemData = FindObjectOfType<MouseItemData>();
    }

    private void Start()
    {
        grid = FindObjectOfType<Grid>();
        if (grid == null)
        {
            Debug.LogError("No Grid!");
        }

        StopPlacement();
    }


    private void Update()
    {
        Vector3 mousePos = GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePos);

        if (gridPosition != lastGridPositionOfCursor)
        {
            lastGridPositionOfCursor = gridPosition;
        }

        if (selectedBuildingIndex < 0)
        {
            return;
        }

        if (!IsSelectedPositionOnGrid())
        {
            return;
        }

        isPointerOverUI = EventSystem.current && EventSystem.current.IsPointerOverGameObject();
        if (isPointerOverUI)
        {
            return;
        }

        //Vector3 mousePos = GetSelectedMapPosition();
        //Vector3Int gridPosition = grid.WorldToCell(mousePos);

        if (IsPlacing && lastPositionForEdit != gridPosition)
        {
            if (buildingManager.PlaceStructureAt(gridPosition, selectedBuildingIndex, buildingFacings[buildingFacingIndex]))
            {
                mouseItemData.AssignedInventorySlot.RemoveFromStack(1);
                mouseItemData.RefreshMouseUI();
            }

            lastPositionForEdit = gridPosition;

            if (mouseItemData.AssignedInventorySlot.StackSize <= 0) return;
        }

        if (IsRemoving && lastPositionForEdit != gridPosition)
        {
            buildingManager.RemoveStructureAt(gridPosition);
            lastPositionForEdit = gridPosition;
        }

        if (lastPositionForPreview != gridPosition)
        {
            bool placementValidity = buildingManager.CheckPlacementValidity(gridPosition, selectedBuildingIndex);
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
            lastPositionForPreview = gridPosition;
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
        //inputActions.UI.Cancel.performed += StopPlacementAction;
        inputActions.PlayerControls.RotateBuilding.performed += RotateBulidingAction; 
    }

    private void OnDisable()
    {
        inputActions.Disable();
        //inputActions.UI.Cancel.performed -= StopPlacementAction;
        inputActions.PlayerControls.RotateBuilding.performed -= RotateBulidingAction;
    }

    private bool IsSelectedPositionOnGrid()
    {
        Vector3 mousePos = inputActions.UI.Point.ReadValue<Vector2>();
        mousePos.z = playerCamera.nearClipPlane;
        Ray ray = playerCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            return true;
        }
        return false;
    }

    private Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = inputActions.UI.Point.ReadValue<Vector2>();
        mousePos.z = playerCamera.nearClipPlane;
        Ray ray = playerCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            lastPositionOfCursor = hit.point;
        }
        return lastPositionOfCursor;
    }

    public void StartPlacement(int Id)
    {
        StopPlacement();
        selectedBuildingIndex = factoryBuildingsDB.BuildingDatabase.FindIndex(data => data.Id == Id);
        FactoryBuildingData selectedBuilding = factoryBuildingsDB.BuildingDatabase[selectedBuildingIndex];
        if (selectedBuildingIndex < 0)
        {
            Debug.LogError($"No ID found: {Id}");
            return;
        }
        //gridEffect.SetActive(true);
        previewSystem.StartShowingPlacementPreview(selectedBuilding.previewPrefab, selectedBuilding.Size, lastGridPositionOfCursor);
        //Vector3 mousePos = GetSelectedMapPosition();
        //Vector3Int gridPosition = grid.WorldToCell(mousePos);
        //previewSystem.UpdatePosition(lastPositionForEdit, buildingManager.CheckPlacementValidity(gridPosition, selectedBuildingIndex));

        inputActions.UI.Click.performed += ClickPressedAction;
        inputActions.UI.Click.canceled += ClickReleasedAction;

        //inputActions.UI.RightClick.performed += RightClickPressedAction;
        //inputActions.UI.RightClick.canceled += RightClickReleasedAction;

        inputActions.UI.Cancel.performed += StopPlacementAction;
    }

    public void StopPlacement()
    {
        selectedBuildingIndex = -1;
        //gridEffect.SetActive(false);
        previewSystem.StopShowingPreview();

        inputActions.UI.Click.performed -= ClickPressedAction;
        inputActions.UI.Click.canceled -= ClickReleasedAction;

        //inputActions.UI.RightClick.performed -= RightClickPressedAction;
        //inputActions.UI.RightClick.canceled -= RightClickReleasedAction;

        inputActions.UI.Cancel.performed -= StopPlacementAction;
    }

    private void StopPlacementAction(InputAction.CallbackContext value)
    {
        StopPlacement();
    }

    private void RotateBulidingFacing()
    {
        if (selectedBuildingIndex < 0) return;

        buildingFacingIndex += 1;
        buildingFacingIndex %= buildingFacings.Length;

        previewSystem.SetFacing(buildingFacings[buildingFacingIndex]);
    }

    #region InputActions
    private void ClickPressedAction(InputAction.CallbackContext value)
    {
        IsPlacing = true;
    }

    private void ClickReleasedAction(InputAction.CallbackContext value)
    {
        IsPlacing = false;
    }

    //private void RightClickPressedAction(InputAction.CallbackContext value)
    //{
    //    isRemoving = true;
    //}

    //private void RightClickReleasedAction(InputAction.CallbackContext value)
    //{
    //    isRemoving = false;
    //}

    private void RotateBulidingAction(InputAction.CallbackContext value)
    {
        RotateBulidingFacing();
    }
    #endregion

}