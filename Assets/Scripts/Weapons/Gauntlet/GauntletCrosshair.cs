using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;


// TODO:
//  Have 3 or 5 circles around the main crosshair that
//  represent how many shots are in the gauntlet before reloading
public class GauntletCrosshair : WeaponCrosshair
{
    [SerializeField][Range(.0001f, 1f)] private float shotCircleRadius = .1f;
    [SerializeField][Range(.0001f, 1f)] private float shotCircleThickness = .005f;

    private int shotCount;
    private int maxShots;

    public void SetShotCount(int _shotsRemaining, int _maxShots)
    {
        shotCount = _shotsRemaining;
        maxShots = _maxShots;
    }

    protected override void OnDrawCommand()
    {
        base.OnDrawCommand();

        //Vector3 pos = Vector3.zero;
        //for (int i = 0; i < maxShots; i++)
        //{

        //    Draw.Ring( , shotCircleRadius, shotCircleThickness, Color.white);
        //}
    }
}
