using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(EntityHealthController))]
public class DamageNumberSpawner : MonoBehaviour
{
    private EntityHealthController healthController;
    [SerializeField] private Transform defaultSpawnPoint;

    [SerializeField] private GameObject damageNumbersPrefab;

    private void Awake()
    {
        healthController = GetComponent<EntityHealthController>();
    }

    private void OnEnable()
    {
        healthController.OnDamageAtPoint += SpawnDamageNumbers;
    }

    private void OnDisable()
    {
        healthController.OnDamageAtPoint -= SpawnDamageNumbers;
    }

    private void SpawnDamageNumbers(int amount, Vector3? position)
    {
        Vector3 spawnPos = defaultSpawnPoint.position;

        if (position != null)
        {
            spawnPos = position.Value;
        }

        DamageNumbers dn = Instantiate(damageNumbersPrefab, spawnPos, Quaternion.identity).GetComponent<DamageNumbers>();
        dn.SetValue(amount);
    }
}
