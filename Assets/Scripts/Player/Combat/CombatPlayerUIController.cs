using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CombatPlayerUIController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    private GameObject QCWindow;

    private void Start()
    {
        QCWindow = FindObjectOfType<QuantumConsole>().gameObject;
        HideQC();
    }

    private void Update()
    {
        if (Keyboard.current.backquoteKey.wasPressedThisFrame && QCWindow != null)
        {
            if (QCWindow.activeSelf)
            {
                HideQC();
            }
            else
            {
                ShowQC();
            }
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame && QCWindow != null)
        {
            if (QCWindow.activeSelf)
                HideQC();
        }
    }

    private void ShowQC()
    {
        if (QCWindow == null) return;

        QCWindow.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        inputManager.Disable();
    }

    private void HideQC()
    {
        if (QCWindow == null) return;

        QCWindow.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inputManager.Enable();
    }
}
