using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BowCrosshair))]
public class BowWeapon : Weapon
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float drawbackTime = .75f;
    [SerializeField] private BowCrosshair crosshair;

    [SerializeField] private float projectileSpeed = 50f;

    private float drawbackTimer = float.MaxValue;
    private bool isDrawingBack = false;

    protected override void Update()
    {
        base.Update();

        if (isDrawingBack)
        {
            crosshair.DrawbackProgress = drawbackTimer / drawbackTime;
            drawbackTimer -= Time.deltaTime;
        }

        if (!isDrawingBack && drawbackTimer <= 0f)
        {
            ShootArrow();
        }
    }

    public override void Init(PlayerCombatController combatController, Transform projectileStartTransform)
    {
        base.Init(combatController, projectileStartTransform);

        crosshair.Init(combatController.PlayerCamera, combatController.HUDTransform);
    }

    private void ShootArrow()
    {
        ArrowProjectile arrow = Instantiate(arrowPrefab, projectileStartPoint.position, projectileStartPoint.rotation).GetComponent<ArrowProjectile>();
        arrow.Launch(playerCombatController.TargetPoint, projectileSpeed);
        drawbackTimer = drawbackTime;
    }

    public override void OnPrimaryPressed()
    {
        crosshair.ShowCrosshair = true;
        isDrawingBack = true;
        drawbackTimer = drawbackTime;
    }

    public override void OnPrimaryReleased()
    {
        isDrawingBack = false;
        crosshair.ShowCrosshair = false;
    }

    public override void OnSecondaryPressed()
    {

    }

    public override void OnSecondaryReleased()
    {

    }
}
