using EasyCharacterMovement;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTargetInteractor : Interactor
{
    [SerializeField] private GameObject playerGameObject; 
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InputManager inputManager;

    private PlayerInputActions input;
    private bool isHoldingInteract = false;

    private void Awake()
    {
        input = inputManager.playerControls;
    }

    private void OnEnable()
    {
        input.CombatControls.Interact.performed += HandleTapInteract;
        input.CombatControls.Interact.canceled += HandleTapInteract;
        input.CombatControls.Interact.performed += HandleHoldInteract;
        input.CombatControls.Interact.canceled += HandleHoldInteract;
    }

    private void OnDisable()
    {
        input.CombatControls.Interact.performed -= HandleTapInteract;
        input.CombatControls.Interact.canceled -= HandleTapInteract;
        input.CombatControls.Interact.performed -= HandleHoldInteract;
        input.CombatControls.Interact.canceled -= HandleHoldInteract;
    }

    private void Update()
    {
        Scan();
    }

    protected override void Scan()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 15f))
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
            {
                if (interactable != targetInteractable)
                {
                    EndHover(playerGameObject);
                    if (!isHoldingInteract)
                    {
                        StopHoldInteract(playerGameObject);
                    }
                    targetInteractable = interactable;
                    StartHover(playerGameObject);
                }

                if (isHoldingInteract)
                {
                    HoldInteract(playerGameObject);
                }
            }
            else
            {
                if (targetInteractable != null)
                {
                    EndHover(playerGameObject);
                    if (isHoldingInteract)
                    {
                        StopHoldInteract(playerGameObject);
                    }
                    targetInteractable = null;
                }
            }
        }
        else
        {
            EndHover(playerGameObject);
            StopHoldInteract(playerGameObject);
        }
    }

    private void HandleTapInteract(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
        {
            TapInteract(playerGameObject);
        }
    }

    private void HandleHoldInteract(InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            isHoldingInteract = true;
        }
        else if (obj.canceled)
        {
            isHoldingInteract = false;
            if (targetInteractable != null) StopHoldInteract(playerGameObject);
        }
    }
}
