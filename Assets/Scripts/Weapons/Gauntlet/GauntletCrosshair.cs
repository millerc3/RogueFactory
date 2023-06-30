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

    [SerializeField][Range(.0001f, 1f)] private float reloadArcRadius = .0125f;
    [SerializeField][Range(.0001f, 1f)] private float reloadArcThickness = .0125f;

    private GauntletWeapon gauntlet;

    private int shotCount;
    private int maxShots;
    private float reloadingTimer = 100f;

    public void SetShotCount(int _shotsRemaining, int _maxShots)
    {
        shotCount = _shotsRemaining;
        maxShots = _maxShots;
    }

    public void SetGauntlet(GauntletWeapon _gauntlet)
    {
        gauntlet = _gauntlet;

        gauntlet.OnGauntletStartReload += () => reloadingTimer = 0f;
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        gauntlet.OnGauntletStartReload += () => reloadingTimer = 0f;
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

        if (gauntlet.IsReloading)
        {
            float t = reloadingTimer / gauntlet.TimeToReload / 2f;
            float angleRad = Mathf.Lerp(shotCountArcStart, shotCountArcStart + shotCountArcLength, t);
            Draw.Arc(reloadArcRadius, reloadArcThickness, shotCountArcStart, angleRad, ArcEndCap.Round, Color.white);

            reloadingTimer += Time.deltaTime;
        }

        //Vector3 pos = Vector3.zero;
        //for (int i = 0; i < maxShots; i++)
        //{

        //    Draw.Ring( , shotCircleRadius, shotCircleThickness, Color.white);
        //}
    }
}
