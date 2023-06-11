using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyCharacterMovement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FactoryCharacterController : ThirdPersonCharacter
{
    [SerializeField] private InputManager inputManager;

    protected InputAction cameraRotateLeftInputAction { get; set; }
    protected InputAction cameraRotateRightInputAction { get; set; }

    protected bool isPointerOverUI = false;


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

        #region THIRD PERSON CHARACTER

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

        #region FACTORY CHARACTER

        cameraRotateLeftInputAction = inputManager.playerControls.FindAction("RotateLeft");
        if (cameraRotateLeftInputAction != null)
        {
            cameraRotateLeftInputAction.performed += OnCameraRotateLeft;
            cameraRotateLeftInputAction.Enable();
        }


        cameraRotateRightInputAction = inputManager.playerControls.FindAction("RotateRight");
        if (cameraRotateRightInputAction != null)
        {
            cameraRotateRightInputAction.performed += OnCameraRotateRight;
            cameraRotateRightInputAction.Enable();
        }

        #endregion
    }


    /// <summary>
    /// Initialize player InputActions (if any).
    /// E.g. Subscribe to input action events and enable input actions here.
    /// </summary>
    //protected override void InitPlayerInput()
    //{
    //    base.InitPlayerInput();

    //    cameraRotateLeftInputAction = inputActions.FindAction("RotateLeft");
    //    if (cameraRotateLeftInputAction != null)
    //    {
    //        cameraRotateLeftInputAction.performed += OnCameraRotateLeft;
    //        cameraRotateLeftInputAction.Enable();
    //    }


    //    cameraRotateRightInputAction = inputActions.FindAction("RotateRight");
    //    if (cameraRotateRightInputAction != null)
    //    {
    //        cameraRotateRightInputAction.performed += OnCameraRotateRight;
    //        cameraRotateRightInputAction.Enable();
    //    }
    //}

    /// <summary>
    /// Perform camera related input actions, eg: Look Up / Down, Turn, etc.
    /// </summary>
    protected override void HandleCameraInput()
    {
        // Mouse scroll input
        Vector2 mouseScrollInput = GetMouseScrollInput();

        if (mouseScrollInput.y != 0.0f)
            cameraController.ZoomAtRate(mouseScrollInput.y);
    }

    protected virtual void OnCameraRotateLeft(InputAction.CallbackContext context)
    {
        cameraController.Turn(90f);
    }

    protected virtual void OnCameraRotateRight(InputAction.CallbackContext context)
    {
        cameraController.Turn(-90f);
    }

    protected override void OnCursorLock(InputAction.CallbackContext context)
    {
        // Do not allow to lock cursor if using UI
        if (isPointerOverUI)
            return;

        if (context.started)
            cameraController.LockCursor();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        isPointerOverUI = EventSystem.current && EventSystem.current.IsPointerOverGameObject();
    }
}
