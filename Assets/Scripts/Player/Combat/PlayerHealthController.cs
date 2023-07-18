using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : EntityHealthController
{
    [SerializeField] private GameObject deathScreenUI;
    [SerializeField] private InputManager inputManager;

    [Command]
    public override void SetHealth(int health)
    {
        base.SetHealth(health);
    }

    public override void Die()
    {
        OnEntityDied?.Invoke();

        deathScreenUI.SetActive(true);
        inputManager.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
