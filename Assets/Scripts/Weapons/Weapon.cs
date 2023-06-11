using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [HideInInspector] public WeaponData WeaponData;

    protected Transform projectileStartPoint;
    protected PlayerCombatController playerCombatController;

    //protected InputManager playerInputManager;

    protected virtual void Awake()
    {

    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    public virtual void Init(PlayerCombatController combatController, Transform projectileStartTransform)
    {
        playerCombatController = combatController;
        projectileStartPoint = projectileStartTransform;
    }

    public abstract void OnPrimaryPressed();
    public abstract void OnPrimaryReleased();

    public abstract void OnSecondaryPressed();
    public abstract void OnSecondaryReleased();

    public void OnPrimaryAction(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            OnPrimaryPressed();
        }
        else if (obj.canceled)
        {
            OnPrimaryReleased();
        }
    }

    public void OnSecondaryAction(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            OnSecondaryPressed();
        }
        else if (obj.canceled)
        {
            OnSecondaryReleased();
        }
    }
}
