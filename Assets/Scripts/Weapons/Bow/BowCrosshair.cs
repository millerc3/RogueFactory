using UnityEngine;
using Shapes;

public class BowCrosshair : WeaponCrosshair
{
    [SerializeField][Range(.0001f, 1f)] private float crosshairOuterRadius = .13f;
    [SerializeField][Range(.0001f, 1f)] private float crosshairInnerRadiusRatio = 0.1f;
    [SerializeField][Range(.0001f, 1f)] private float crosshairTickness = .005f;

    [SerializeField] private Color bowNotReadyColor = Color.red;
    [SerializeField] private Color bowReadyColor = Color.cyan;

    private float crosshairInnerRadius;
    public float DrawbackProgress;
    private float radiusDelta;
    public bool ShowCrosshair = false;

    private void Start()
    {
        crosshairInnerRadius = crosshairOuterRadius * crosshairInnerRadiusRatio;
        radiusDelta = crosshairOuterRadius - crosshairInnerRadius;
    }

    private void DrawShrinkingRing()
    {
        float currRadius = crosshairInnerRadius + radiusDelta * DrawbackProgress;
        if (currRadius <= crosshairInnerRadius)
        {
            Draw.Ring(crosshairInnerRadius, crosshairTickness, bowReadyColor);
        }
        else
        {
            Draw.Ring(currRadius, crosshairTickness, bowNotReadyColor);
        }
    }

    protected override void OnDrawCommand()
    {
        base.OnDrawCommand();

        if (ShowCrosshair)
        {
            DrawShrinkingRing();
        }
    }
}
