using EasyCharacterMovement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCombatCharacterController : ThirdPersonCharacter
{
    [SerializeField] private InputManager inputManager;

    protected override void InitPlayerInput()
    {
        // Attempts to cache Character InputActions (if any)

        if (inputManager == null)
            return;

        #region CHARACTER

        // Movement input action (no handler, this is polled, e.g. GetMovementInput())

        movementInputAction = inputManager.playerControls.FindAction("Movement");
        movementInputAction?.Enable();

        // Setup Sprint input action handlers

        sprintInputAction = inputManager.playerControls.FindAction("Sprint");
        if (sprintInputAction != null)
        {
            sprintInputAction.started += OnSprint;
            sprintInputAction.performed += OnSprint;
            sprintInputAction.canceled += OnSprint;

            sprintInputAction.Enable();
        }

        // Setup Crouch input action handlers

        crouchInputAction = inputManager.playerControls.FindAction("Crouch");
        if (crouchInputAction != null)
        {
            crouchInputAction.started += OnCrouch;
            crouchInputAction.performed += OnCrouch;
            crouchInputAction.canceled += OnCrouch;

            crouchInputAction.Enable();
        }

        // Setup Jump input action handlers

        jumpInputAction = inputManager.playerControls.FindAction("Jump");
        if (jumpInputAction != null)
        {
            jumpInputAction.started += OnJump;
            jumpInputAction.performed += OnJump;
            jumpInputAction.canceled += OnJump;

            jumpInputAction.Enable();
        }

        #endregion

        #region THIRDPERSONCHARACTER

        mouseLookInputAction = inputManager.playerControls.FindAction("Mouse Look");
        mouseLookInputAction?.Enable();

        mouseScrollInputAction = inputManager.playerControls.FindAction("Mouse Scroll");
        mouseScrollInputAction?.Enable();

        controllerLookInputAction = inputManager.playerControls.FindAction("Controller Look");
        controllerLookInputAction?.Enable();

        cursorLockInputAction = inputManager.playerControls.FindAction("Cursor Lock");
        if (cursorLockInputAction != null)
        {
            cursorLockInputAction.started += OnCursorLock;
            cursorLockInputAction.Enable();
        }

        cursorUnlockInputAction = inputActions.FindAction("Cursor Unlock");
        if (cursorUnlockInputAction != null)
        {
            cursorUnlockInputAction.started += OnCursorUnlock;
            cursorUnlockInputAction.Enable();
        }

        #endregion
    }
}
