using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FlightPlannerPlanet : MonoBehaviour
{
    [SerializeField] private FlightPlannerPlanetDatabase planetsDB;
    public FlightPlannerPlanetData PlanetData;
    [HideInInspector]
    public Sprite PlanetSprite;
    [SerializeField] private Image planetImage; 

    private FlightPlannerShip ship;
    [SerializeField] private Button planetButton;
    public List<PlanetConnection> Connections= new List<PlanetConnection>();

    private PlanetInfoViewController planetInfoViewController;

    [HideInInspector]
    public RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        planetInfoViewController = FindObjectOfType<PlanetInfoViewController>();

        PlanetData = planetsDB.GetRandomPlanet();
        PlanetSprite = PlanetData.PlanetSprites[UnityEngine.Random.Range(0, PlanetData.PlanetSprites.Length - 1)];
        planetImage.sprite = PlanetSprite;
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        planetButton.onClick.AddListener(AttemptToMoveShipHere);
        planetButton.onClick.AddListener(UpdatePlanetViewWithThisPlanetData);
    }

    private void OnDisable()
    {
        planetButton.onClick.RemoveListener(AttemptToMoveShipHere);
        planetButton.onClick.RemoveListener(UpdatePlanetViewWithThisPlanetData);
    }

    private void AttemptToMoveShipHere()
    {
        if (ship == null)
        {
            ship = FindObjectOfType<FlightPlannerShip>();
        }

        ship.MoveToPlanet(this);
    }

    private void UpdatePlanetViewWithThisPlanetData()
    {
        planetInfoViewController.SetPlanetData(PlanetData, PlanetSprite);
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
