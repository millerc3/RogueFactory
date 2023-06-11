using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public PlayerInputActions playerControls { get; private set; }
    public PlayerInputActions.CombatControlsActions combatControls { get; private set; }

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        combatControls = playerControls.CombatControls;
    }

    private void OnEnable()
    {
        Enable();
    }

    private void OnDisable()
    {
        Disable();
    }

    public void Enable()
    {
        playerControls.Enable();
        combatControls.Enable();
    }

    public void Disable()
    {
        playerControls.Disable();
        combatControls.Disable();
    }



}
