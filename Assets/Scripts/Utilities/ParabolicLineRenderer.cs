using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicLineRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public int numberOfPoints = 50;
    public float timeBetweenPoints = .1f;

    public LayerMask collidableLayers;

    private List<Vector3> activePoints = new List<Vector3>();

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void DrawLine(Vector3 force, Vector3 startPoint)
    {
        //Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), startPoint, Quaternion.identity);
        lineRenderer.positionCount = numberOfPoints;
        activePoints.Clear();
        for (float t = 0; t < numberOfPoints; t += timeBetweenPoints)
        {
            Vector3 newPoint = startPoint + t * force;
            newPoint.y = startPoint.y + force.y * t + Physics.gravity.y / 2f * t * t;
            activePoints.Add(newPoint);

            if (Physics.OverlapSphere(newPoint, .5f, collidableLayers).Length > 0)
            {
                lineRenderer.positionCount = activePoints.Count;
                break;
            }
        }

        lineRenderer.SetPositions(activePoints.ToArray());
    }
}
