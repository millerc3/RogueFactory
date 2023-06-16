using EasyCharacterMovement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyController : AgentCharacter
{
    private Transform targetTransform;

    protected override void OnStart()
    {
        base.OnStart();

        targetTransform = FindObjectOfType<TPSCombatCharacterController>()?.transform;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        MoveToLocation(targetTransform.position);
    }
}
