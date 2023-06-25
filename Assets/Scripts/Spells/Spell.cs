using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    [HideInInspector] public SpellData spellData;

    protected Transform castPoint;
    protected PlayerSpellController playerSpellController;

    #region MonoBehaviour
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
    #endregion

    #region Spell

    public virtual void Init(PlayerSpellController _playerSpellController, Transform _castPoint)
    {
        playerSpellController = _playerSpellController;
        castPoint = _castPoint;
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

    #endregion
}
