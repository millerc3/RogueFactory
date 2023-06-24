using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;


// TODO:
//  Have 3 or 5 circles around the main crosshair that
//  represent how many shots are in the gauntlet before reloading
public class GauntletCrosshair : WeaponCrosshair
{
    [SerializeField][Range(.0001f, 1f)] private float shotCountRadius = .2f;
    [SerializeField][Range(0, ShapesMath.TAU)] private float shotCountArcLength = ShapesMath.TAU/4f;
    [SerializeField][Range(0, ShapesMath.TAU)] private float shotCountArcStart = ShapesMath.TAU * 5f / 8f;
    [SerializeField][Range(.0001f, 1f)] private float shotCircleRadius = .025f;
    [SerializeField][Range(.0001f, 1f)] private float shotCircleThickness = .005f;

    private int shotCount;
    private int maxShots;

    public void SetShotCount(int _shotsRemaining, int _maxShots)
    {
        shotCount = _shotsRemaining;
        maxShots = _maxShots;

        print($"MaxShots={_maxShots}");
        print($"ShotCount={_shotsRemaining}");
    }

    protected override void OnDrawCommand()
    {
        base.OnDrawCommand();


        for (int i = 0; i < maxShots; i++)
        {
            float t = (i + 1f) / (maxShots + 1f);
            float angleRad = Mathf.Lerp(shotCountArcStart, shotCountArcStart + shotCountArcLength, t);
            Vector2 dir = ShapesMath.AngToDir(angleRad);
            Vector2 origin = dir * shotCountRadius;
            if (i >= maxShots - shotCount)
            {
                // Has Shot
                Draw.Disc(origin, shotCircleRadius, Color.white);
                Draw.Ring(origin, shotCircleRadius, shotCircleThickness, Color.cyan);
                //Draw.Ring(origin, shotCircleRadius, shotCircleThickness, Color.gray);
            }
            else
            {
                // Doesn't have shot
                Draw.Disc(origin, shotCircleRadius, Color.white);
                Draw.Ring(origin, shotCircleRadius, shotCircleThickness, Color.red);
            }
        }

        //Vector3 pos = Vector3.zero;
        //for (int i = 0; i < maxShots; i++)
        //{

        //    Draw.Ring( , shotCircleRadius, shotCircleThickness, Color.white);
        //}
    }
}
