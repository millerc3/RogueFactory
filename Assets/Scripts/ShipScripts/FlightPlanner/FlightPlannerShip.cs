using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightPlannerShip : MonoBehaviour
{
    [HideInInspector]
    public FlightPlannerPlanet CurrentPlanet;

    [SerializeField] private float travelTime = 1f;

    [SerializeField]
    private bool isMoving = false;

    public void MoveToPlanet(FlightPlannerPlanet planet)
    {
        print($"Attempting to move to planet @ {planet.transform.position}");
        if (isMoving) return;

        foreach (PlanetConnection connection in CurrentPlanet.Connections)
        {
            if (connection.Contains(planet))
            {
                transform.DOMove(planet.transform.position, travelTime).OnStart(() => isMoving = true).OnComplete(() => isMoving = false);
                CurrentPlanet = planet;
                return;
            }
        }
    }
}
