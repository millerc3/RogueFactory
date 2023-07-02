using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityHealthController))]
public class ShieldWithHealth : MonoBehaviour
{
    private EntityHealthController healthController;
    [SerializeField] private ResetParticleEffectOnAction resetParticleEffectComponent;

    [SerializeField] private GameObject shieldHitPrefab;

    private void Awake()
    {
        healthController = GetComponent<EntityHealthController>();
    }

    private void OnEnable()
    {
        healthController.OnDamageAtPoint += OnHitAtPoint;
    }

    private void OnDisable()
    {
        healthController.OnDamageAtPoint -= OnHitAtPoint;
    }

    private void OnHitAtPoint(int hitAmount, Vector3? hitPos)
    {
        resetParticleEffectComponent.ResetParticleSystem();
        if (hitPos != null)
        {
            Instantiate(shieldHitPrefab, hitPos.Value, Quaternion.identity);
        }
    }
}
