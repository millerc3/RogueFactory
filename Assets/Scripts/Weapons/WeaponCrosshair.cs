using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WeaponCrosshair : ImmediateModeShapeDrawer
{
    protected Camera cam;
    protected Transform HUDTransform;

    public void Init(Camera targetCam, Transform hudTransform)
    {
        cam = targetCam;
        HUDTransform = hudTransform;
    }

    protected virtual void OnDrawCommand()
    {
        // Draw on top of everything else
        Draw.ZTest = CompareFunction.Always;
        Draw.BlendMode = ShapesBlendMode.Transparent;
        Draw.LineGeometry = LineGeometry.Flat2D;
        Draw.Matrix = HUDTransform.localToWorldMatrix;
    }

    public override void DrawShapes(Camera cam)
    {
        if (HUDTransform == null) return;
        //if (cam != this.cam) return; // Can only draw in the player's camera

        using (Draw.Command(cam))
        {
            OnDrawCommand();
        }
    }
}
