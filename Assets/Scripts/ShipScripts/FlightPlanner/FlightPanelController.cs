using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shapes;
using DG.Tweening;

public class FlightPanelController : MonoBehaviour
{
    [SerializeField] private RectTransform flightViewPanel;
    [SerializeField] private PlanetInfoViewController planetInfoViewController;

    [SerializeField] private int planetsToSpawn = 10;

    [SerializeField] private GameObject shipPrefab;
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private GameObject lineConnection;

    [SerializeField] private float maxTravelDistance = 40f;
    [SerializeField] private float minPlanetNeighborDistance = 25f;
    //[SerializeField] private float maxPlanetNeighborDistance = 80f;

    List<FlightPlannerPlanet> planets = new List<FlightPlannerPlanet>();
    List<PlanetConnection> planetConnections = new List<PlanetConnection>();

    private void Start()
    {
        EvenlyPlaceRandomPlanets();
        ConnectReachablePlanets();

        PlaceShipAtTopLeftPlanet();
    }

    private void EvenlyPlaceRandomPlanets()
    {
        int runCount = 0;
        for (int i = 0; i < planetsToSpawn; i++)
        {
            if (runCount > 1000)
            {
                Debug.LogError("Too many attempts");
                return;
            }

            runCount++;
            float x = Random.Range(0, 2f * flightViewPanel.rect.x);
            float y = Random.Range(0, 2f * flightViewPanel.rect.y);
            Vector3 spawnPos = GetBottomLeftCorner(flightViewPanel) - new Vector3(x, y, 0);

            if (IsTooCloseToAnyPlanets(spawnPos))
            {
                i--;
                continue;
            }

            if (planets.Count > 0 && !IsCloseEnoughToAtleastOnePlanet(spawnPos, maxTravelDistance))
            {
                i--;
                continue;
            }

            FlightPlannerPlanet planet = Instantiate(planetPrefab, spawnPos, Quaternion.identity).GetComponent<FlightPlannerPlanet>();
            planet.transform.SetParent(flightViewPanel);
            planets.Add(planet);

            runCount = 0;
        }
    }

    private void ConnectReachablePlanets()
    {
        foreach (FlightPlannerPlanet planet in planets)
        {
            foreach (FlightPlannerPlanet otherPlanet in planets)
            {
                if (Equals(planet, otherPlanet)) 
                    continue;

                if (Vector3.Distance(planet.transform.position, otherPlanet.transform.position) > maxTravelDistance)
                    continue;

                PlanetConnection conn = new PlanetConnection(planet, otherPlanet);

                bool found = false;
                foreach (PlanetConnection connection in planetConnections)
                {
                    if (connection.Equals(conn))    
                    {
                        found = true;
                    }
                }
                if (found) continue;

                planetConnections.Add(conn);
                
                CreateALine(planet.transform.position, otherPlanet.transform.position);

                planet.AddNeighborPlanet(otherPlanet);
                otherPlanet.AddNeighborPlanet(planet);
            }
        }
    }

    private void PlaceShipAtTopLeftPlanet()
    {
        FlightPlannerPlanet topLeftPlanet = planets[0];

        foreach (FlightPlannerPlanet planet in planets)
        {
            if (planet.transform.localPosition.x < topLeftPlanet.transform.localPosition.x)
            {
                topLeftPlanet = planet;   
            }
        }

        FlightPlannerShip ship = Instantiate(shipPrefab, topLeftPlanet.transform.position, Quaternion.identity).GetComponent<FlightPlannerShip>();
        ship.transform.SetParent(flightViewPanel, true);
        ship.CurrentPlanet = topLeftPlanet;
        planetInfoViewController.SetPlanetData(ship.CurrentPlanet.PlanetData, ship.CurrentPlanet.PlanetSprite);
    }

    bool IsTooCloseToAnyPlanets(Vector3 pos)
    {
        foreach (FlightPlannerPlanet planet in planets)
        {
            if (Vector3.Distance(pos, planet.transform.position) < 1f) continue;

            if (Vector3.Distance(planet.transform.position, pos) <= minPlanetNeighborDistance)
            {
                return true;
            }
        }

        return false;
    }

    bool IsCloseEnoughToAtleastOnePlanet(Vector3 pos, float travelDistance)
    {
        foreach (FlightPlannerPlanet planet in planets)
        {
            if (Vector3.Distance(pos, planet.transform.position) < 1f) continue;

            if (Vector3.Distance(planet.transform.position, pos) <= travelDistance)
            {
                return true;
            }
        }

        return false;
    }

    Vector3 GetBottomLeftCorner(RectTransform rt)
    {
        Vector3[] v = new Vector3[4];
        rt.GetWorldCorners(v);
        return v[0];
    }

    private void CreateALine(Vector3 objA, Vector3 objB)
    {
        /*spawn a prefab image "lineConnection" as angleBar*/
        GameObject angleBar = Instantiate(lineConnection, objB, Quaternion.identity);
        /**/
        /*calculate angle*/
        Vector2 diference = objA - objB;
        float sign = (objA.y < objB.y) ? -1.0f : 1.0f;
        float angle = Vector2.Angle(Vector2.right, diference) * sign;
        angleBar.transform.Rotate(0, 0, angle);
        /**/
        /*calculate length of bar*/
        float height = 5;
        float width = Vector2.Distance(objB, objA);
        angleBar.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        /**/
        /*calculate midpoint position*/
        float newposX = objB.x + (objA.x - objB.x) / 2;
        float newposY = objB.y + (objA.y - objB.y) / 2;
        angleBar.transform.position = new Vector3(newposX, newposY, 0);
        /***/
        /*set parent to objB*/
        angleBar.transform.SetParent(flightViewPanel, true);
    }
}

public class PlanetInfo
{
    public string Name;
    public List<PlanetConnection> connections = new List<PlanetConnection>();
}

