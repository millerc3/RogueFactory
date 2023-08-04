using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlightPlannerStationTrigger : MonoBehaviour
{
    [SerializeField] private GameObject flightPlannerView;

    private InputManager currInputManager;

    private void OnTriggerEnter(Collider other)
    {
        Transform playerRoot = other.transform.root;
        currInputManager = playerRoot.GetComponentInChildren<InputManager>();

        if (currInputManager == null)
        {
            Debug.LogError($"No InputManager found on player!");
        }

        currInputManager.playerControls.PlayerControls.Disable();
        OpenFlightPlannerView();

        currInputManager.playerControls.UI.Cancel.performed += ctx => CloseFlightPlannerView();
    }

    private void OnTriggerExit(Collider other)
    {
        if (currInputManager != null)
        {
            currInputManager.playerControls.PlayerControls.Enable();

            currInputManager.playerControls.UI.Cancel.performed -= ctx => CloseFlightPlannerView();
        }

        currInputManager = null;
    }

    private void OpenFlightPlannerView()
    {
        flightPlannerView.SetActive(true);
    }

    public void CloseFlightPlannerView()
    {
        flightPlannerView.SetActive(false);
        currInputManager.playerControls.PlayerControls.Enable();
    }
}
