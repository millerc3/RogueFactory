using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FlightPlannerPlanet : MonoBehaviour
{
    private FlightPlannerShip ship;
    [SerializeField] private Button planetButton;
    public List<PlanetConnection> Connections= new List<PlanetConnection>();

    [HideInInspector]
    public RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        planetButton.onClick.AddListener(AttemptToMoveShipHere);
    }

    private void OnDisable()
    {
        planetButton.onClick.RemoveListener(AttemptToMoveShipHere);
    }

    private void AttemptToMoveShipHere()
    {
        if (ship == null)
        {
            ship = FindObjectOfType<FlightPlannerShip>();
        }

        ship.MoveToPlanet(this);
    }

    public void AddNeighborPlanet(FlightPlannerPlanet planet)
    {
        Connections.Add(new PlanetConnection(this, planet));
    }
}

[Serializable]
public struct PlanetConnection
{
    public FlightPlannerPlanet p1;
    public FlightPlannerPlanet p2;

    public PlanetConnection(FlightPlannerPlanet _p1, FlightPlannerPlanet _p2)
    {
        p1 = _p1;
        p2 = _p2;
    }

    public bool Equals(PlanetConnection other)
    {
        if (p1 == other.p1) return p2 == other.p2;
        if (p1 == other.p2) return p2 == other.p1;

        return false;
    }

    public bool Contains(FlightPlannerPlanet other)
    {
        return p1 == other || p2 == other;
    }
}
