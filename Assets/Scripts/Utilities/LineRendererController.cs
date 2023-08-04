using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LineRendererController : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3[] points;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void SetPoints(Transform[] _points)
    {
        lr.positionCount = _points.Length;
        for (int i = 0; i < _points.Length; i++)
        {
            points[i] = _points[i].position;
        }

        UpdatePoints();
    }

    public void SetPoints(Vector3[] _points)
    {
        lr.positionCount = _points.Length;
        points = _points;

        UpdatePoints();
    }

    public void UpdatePoints()
    {
        for (int i = 0; i < points.Length; i++) 
        {
            lr.SetPosition(i, points[i]);
        }
    }
}
