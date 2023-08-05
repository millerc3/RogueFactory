using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FlightPlanner/Planet Database")]
public class FlightPlannerPlanetDatabase : ObjectDatabase
{
    public FlightPlannerPlanetData GetPlanet(int id)
    {
        return Objects.Find(i => i.Id == id) as FlightPlannerPlanetData;
    }

    public FlightPlannerPlanetData GetRandomPlanet()
    {
        return Objects[Random.Range(0, Objects.Count)] as FlightPlannerPlanetData;
    }
}
