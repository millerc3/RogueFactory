using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseInteractor : Interactor
{
    [SerializeField] private GameObject playerGameObject;
    [SerializeField] private Camera playerCamera;

    private PlayerInputActions input = null;
    private bool isPointerOverUI = false;

    //private bool isInteracting = false;
    private bool isHoldInteracting = false;

    private MouseItemData mouseItemData;

    private void Awake()
    {
        input = new PlayerInputActions();
        mouseItemData = FindObjectOfType<MouseItemData>();
    }

    private void OnEnable()
    {
        input.Enable();
        input.UI.RightClick.performed += HandleTapInteract;
        input.UI.RightClick.canceled += HandleTapInteract;
        input.UI.Click.performed += HandleHoldInteract;
        input.UI.Click.canceled += HandleHoldInteract;
    }

    private void OnDisable()
    {
        input.Disable();
        input.UI.RightClick.performed -= HandleTapInteract;
        input.UI.RightClick.canceled -= HandleTapInteract;
        input.UI.Click.performed -= HandleHoldInteract;
        input.UI.Click.canceled -= HandleHoldInteract;
    }

    private void Update()
    {
        Scan();

        //if (isInteracting)
        //{
        //    Interact(gameObject);
        //}

        //if (isAltInteracting)
        //{
        //    AltInteract(gameObject);
        //}
    }

    protected override void Scan()
    {
        isPointerOverUI = EventSystem.current && EventSystem.current.IsPointerOverGameObject();
        if (isPointerOverUI) return;

        if (mouseItemData.AssignedInventorySlot.ItemData != null) return;

        Vector3 mousePosition = input.UI.Point.ReadValue<Vector2>();
        mousePosition.z = playerCamera.nearClipPlane;
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
            {
                if (interactable != targetInteractable)
                {
                    EndHover(playerGameObject);
                    if (!isHoldInteracting)
                    {
                        StopHoldInteract(playerGameObject);
                    }
                    targetInteractable = interactable;
                    StartHover(playerGameObject);
                }

                if (isHoldInteracting)
                {
                    HoldInteract(playerGameObject);
                }
            }
            else
            {
                if (targetInteractable != null)
                {                    
                    EndHover(playerGameObject);
                    if (isHoldInteracting)
                    {
                        StopHoldInteract(playerGameObject);
                    }
                    targetInteractable = null;
                }
            }
        }
    }

    private void HandleTapInteract(InputAction.CallbackContext obj)
    {
        //if (mouseItemData.AssignedInventorySlot.ItemData != null) return;

        if (obj.canceled)
        {
            TapInteract(playerGameObject);
        }
    }

    private void HandleHoldInteract(InputAction.CallbackContext obj)
    {
        //if (mouseItemData.AssignedInventorySlot.ItemData != null) return;

        if (obj.performed)
        {
            isHoldInteracting = true;
        }
        else if (obj.canceled)
        {
            isHoldInteracting = false;
            if (targetInteractable != null) StopHoldInteract(playerGameObject);
        }
    }

}
