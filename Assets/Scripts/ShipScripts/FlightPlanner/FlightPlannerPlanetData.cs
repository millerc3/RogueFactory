using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FlightPlanner/Planet Data")]
public class FlightPlannerPlanetData : DatabaseObject
{
    public string TypeName;
    public string Description;
    public Sprite[] PlanetSprites;
    public InventoryItemData[] PlanetResources;
}
