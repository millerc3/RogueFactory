using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class FactoryBuilding : MonoBehaviour, IInteractable
{
    protected BuildingSystemManager buildingManager;

    protected List<FactoryBuilding> adjacentBuildings = new List<FactoryBuilding>();
    protected List<Vector3Int> adjacentGrids= new List<Vector3Int>();
    [HideInInspector] public FactoryPlacementData placementData;
    public FactoryBuildingData BuildingData;
    [HideInInspector] public int PlacementId;
    [HideInInspector] public Vector3Int Origin;
    public Transform RotatePivot;

    // Interaction System
    [HideInInspector] protected Transform interactorTransform = null;
    [SerializeField] protected float interactionRange = 5f;

    [SerializeField] protected float tearDownTime = .5f;
    public float TearDownTime => tearDownTime;
    private float tearDownTimer = float.MaxValue;

    private bool tearingDown = false;

    // Player Inventory System
    protected PlayerInventoryHolder playerInventoryHolder;

    public UnityAction<IInteractable> OnInteractionComplete { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    protected virtual void Awake()
    {
        buildingManager = FindObjectOfType<BuildingSystemManager>();
        if (!buildingManager)
        {
            Debug.LogError($"There is no building system manager in the scene!");
        }

        playerInventoryHolder = FindObjectOfType<PlayerInventoryHolder>();
    }

    protected virtual void Start()
    {
        SetAdjacentGrids();
        tearDownTimer = tearDownTime;
    }

    protected virtual void Update()
    {
        if (!tearingDown)
        {
            tearDownTimer = tearDownTime;
        }
        else
        {
            tearDownTimer -= Time.deltaTime;

            if (tearDownTimer < 0)
            {
                TearDown();
            }
        }
    }

    protected virtual void OnEnable()
    {
        FactoryManager.instance.OnFactoryTick += OnTick;

        SaveGameManager.PreSaveGameEvent += SaveData;
    }

    protected virtual void OnDisable()
    {
        FactoryManager.instance.OnFactoryTick -= OnTick;

        SaveGameManager.PreSaveGameEvent -= SaveData;
    }

    public void RotateTowards(Vector3 forward)
    {
        transform.RotateAround(RotatePivot.position, Vector3.up, 90f);
        //RotatePivot.parent = null;
        //transform.parent = RotatePivot;
        //RotatePivot.rotation = Quaternion.LookRotation(forward, Vector3.up);
        //transform.parent = null;
        //RotatePivot.parent = transform;
    }

    /// <summary>
    /// What should this factory do on every tick
    /// </summary>
    protected virtual void OnTick()
    {

    }

    /// <summary>
    /// If items be removed from the output, set <paramref name="amountAbleToRemove"/> equal to the number of items
    /// in the output inventory that can be removed - Note, just grab the item from the first stack and return the number
    /// of items that can be removed from that item type
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public abstract bool CanRemoveFromOutput(out int amountAbleToRemove);

    /// <summary>
    /// Check if <paramref name="count"/> instances of <paramref name="item"/> can fit in the inventory.  
    /// Update <paramref name="AmountOfItemsThatCanBeAdded"/> with the number of items that can be added.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    /// <param name="AmountOfItemsThatCanBeAdded"></param>
    /// <returns></returns>
    public abstract bool CanAddToInput(InventoryItemData item, int count, out int AmountOfItemsThatCanBeAdded);

    /// <summary>
    /// Remove <paramref name="count"/> instances of the first item in the inventory
    /// and pass the removed InventoryItem Data <paramref name="removedItem"/> and the
    /// GameObject instance to the <paramref name="worldObject"/> if one already
    /// exists.  Return the number of items that were successfully removed
    /// </summary>
    /// <param name="count"></param>
    /// <param name="removedItem"></param>
    /// <param name="worldObject"></param>
    /// <returns></returns>
    public abstract int RemoveFromOutput(int count, out InventoryItemData removedItem, out GameObject worldObject);

    /// <summary>
    /// Attempt to add <paramref name="count"/> instances of <paramref name="item"/> to the inventory system.
    /// Return the number of items that were successfully added to the inventory
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    /// <param name="worldObject"></param>
    /// <returns></returns>
    public abstract int AddToInput(InventoryItemData item, int count, GameObject worldObject);

    /// <summary>
    /// Iterate through the neighbors and update them
    /// </summary>
    /// <param name="depth"></param>
    public abstract void LocalUpdate(int depth);

    /// <summary>
    /// Iterate over all adjacent grid locations and store them
    /// </summary>
    protected virtual void SetAdjacentGrids()
    {
        if (placementData == null) return;

        List<Vector3Int> myPositions = placementData.occupiedPositions;

        Vector3Int currPos;
        for (int i = 0; i < myPositions.Count; i++)
        {
            currPos = myPositions[i];
            int z = i % BuildingData.Size.y + Origin.z;
            int x = i / BuildingData.Size.y + Origin.x;

            // add the left side
            if (x == Origin.x)
            {
                adjacentGrids.Add(new Vector3Int(currPos.x - 1,
                                                 currPos.y,
                                                 currPos.z));
            }
            // add the right side
            if (x == Origin.x + BuildingData.Size.x - 1)
            {
                adjacentGrids.Add(new Vector3Int(currPos.x + 1,
                                                 currPos.y,
                                                 currPos.z));
            }
            // add the top side
            if (z == Origin.z)
            {
                adjacentGrids.Add(new Vector3Int(currPos.x,
                                                 currPos.y,
                                                 currPos.z - 1));
            }
            // add the bottom side
            if (z == Origin.z + BuildingData.Size.y - 1)
            {
                adjacentGrids.Add(new Vector3Int(currPos.x,
                                                 currPos.y,
                                                 currPos.z + 1));
            }
        }

        //foreach (Vector3Int grid in adjacentGrids)
        //{
        //    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    go.transform.position = grid + (Vector3.one / 2f);
        //}

    }

    /// <summary>
    /// Attempt to find all adjacent FactoryBuildings and populate the adjacentBuildings list
    /// </summary>
    public virtual void CheckNeighbors()
    {
        foreach (Vector3Int adjacentGrid in adjacentGrids)
        {
            FactoryPlacementData pd = buildingManager.GetStructureAt(adjacentGrid);
            if (pd == null) continue;

            FactoryBuilding building = pd.BuildingGameObject.GetComponent<FactoryBuilding>();
            if (building == null) continue;
            if (adjacentBuildings.Contains(building)) continue;

            adjacentBuildings.Add(building);
        }
    }

    /// <summary>
    /// Remove <paramref name="neighbor"/> from the list of adjacent factory buildings
    /// </summary>
    /// <param name="neighbor"></param>
    public virtual void RemoveNeighbor(FactoryBuilding neighbor)
    {
        if (adjacentBuildings.Contains(neighbor))
        {
            adjacentBuildings.Remove(neighbor);
            LocalUpdate(1);
        }
    }

    /// <summary>
    /// Iterate over all of the adjacent buildings and update them
    /// </summary>
    /// <param name="depth"></param>
    public virtual void UpdateNeighbors(int depth)
    {
        foreach (FactoryBuilding adjacentBuilding in adjacentBuildings)
        {
            adjacentBuilding.LocalUpdate(depth);
        }
    }

    /// <summary>
    /// Iterate over all of the adjacent buildings and remove this building from their
    /// list of adjacent buildings
    /// </summary>
    public virtual void RemoveSelfFromNeighbors()
    {
        foreach (FactoryBuilding adjacentBuilding in adjacentBuildings)
        {
            adjacentBuilding.RemoveNeighbor(this);
        }
    }

    /// <summary>
    /// What should happen when this building is torn down?
    /// </summary>
    protected virtual void TearDown()
    {
        // TODO put numberUnableToAdd onto the ground or something
        RemoveSelfFromNeighbors();
        playerInventoryHolder.AddToInventory(BuildingData.InventoryItem, 1, out int _);
        tearingDown = false;
        buildingManager.RemoveStructureAt(Origin);
    }

    /// <summary>
    /// How should this building load its data
    /// </summary>
    public abstract void LoadData(SaveData saveData);

    /// <summary>
    /// How should this building save its data
    /// </summary>
    protected abstract void SaveData(SaveData saveData);

    #region Interaction

    protected virtual void OnTapInteract()
    {

    }

    protected virtual void OnHoldInteract()
    {
        tearingDown = true;
    }

    public virtual void TapInteract(GameObject interactor, out bool interactSuccessful)
    {
        if (interactor != null)
        {
            interactorTransform = interactor.transform;
            if (true || InRangeOfInteractor())
            {
                OnTapInteract();
            }
        }
        else
        {
            OnTapInteract();
        }

        interactSuccessful = true;
    }

    public virtual void StartHover(GameObject interactor)
    {

    }

    public virtual void EndHover(GameObject interactor)
    {

    }

    protected bool InRangeOfInteractor()
    {
        if (interactorTransform == null) return true;

        float d = Vector3.Distance(transform.position, interactorTransform.position);
        return d <= interactionRange;
    }

    public void HoldInteract(GameObject interactor, out bool interactSuccessful)
    {
        interactSuccessful = true;

        if (interactor != null)
        {
            interactorTransform = interactor.transform;
            if (InRangeOfInteractor())
            {
                OnHoldInteract();
            }
        }
        else
        {
            OnHoldInteract();
        }
    }

    public void StopHoldInteract(GameObject interactor)
    {
        tearingDown = false;
    }

    #endregion
}