using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightPlannerShip : MonoBehaviour
{
    [HideInInspector]
    public FlightPlannerPlanet CurrentPlanet;

    [SerializeField] private Transform model;
    [SerializeField] private Transform modelHolder;
    [SerializeField] private float orbitRadius = 20f;

    [SerializeField] private float travelTime = 1f;

    [SerializeField]
    private bool isMoving = false;
    private Transform targetPlanet = null;

    public void MoveToPlanet(FlightPlannerPlanet planet)
    {
        print($"Attempting to move to planet @ {planet.transform.position}");
        //if (isMoving) return;

        foreach (PlanetConnection connection in CurrentPlanet.Connections)
        {
            if (connection.Contains(planet))
            {
                //transform.DOMove(planet.transform.position, travelTime);
                transform.DOMove(planet.transform.position, travelTime).OnStart(() => isMoving = true).OnComplete(() => isMoving = false);
                //transform.position = planet.transform.position;
                CurrentPlanet = planet;
                targetPlanet = CurrentPlanet.transform;
                return;
            }
        }
    }

    private void Update()
    {
        if (!isMoving)
        {
            model.localPosition = Vector3.right * orbitRadius;
            modelHolder.RotateAround(transform.position, Vector3.forward, 20 * Time.deltaTime);
        }
        //else
        //{
        //    model.localPosition = Vector3.zero;
        //    model.LookAt(targetPlanet);
        //}
    }
}
