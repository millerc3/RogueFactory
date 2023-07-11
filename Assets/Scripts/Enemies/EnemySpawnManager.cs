using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnManager : MonoBehaviour
{
    private Entity playerEntity;

    [SerializeField] private int enemiesPerMinute = 15;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float maxRadiusToSpawnFromPlayer = 30f;
    [SerializeField] private float minRadiusToSpawnFromPlayer = 10f;
    private float timeBetweenEnemySpawns;
    private float spawnTimer = 0f;

    private void Awake()
    {
        GetPlayerEntity();
        timeBetweenEnemySpawns = 60f / enemiesPerMinute;
    }

    private void Update()
    {
        if (spawnTimer >=timeBetweenEnemySpawns)
        {
            SpawnEnemyNearPlayer();

            spawnTimer = 0f;
        }

        spawnTimer += Time.deltaTime;
    }

    private void GetPlayerEntity()
    {
        Entity[] entities = GameObject.FindObjectsOfType<Entity>();
        foreach (Entity entity in entities)
        {
            if (entity.EntityData.Team == EntityTeam_t.PLAYER)
            {
                playerEntity = entity;
                break;
            }
        }

        if (playerEntity == null)
        {
            Debug.LogError($"There is no player entity found");
        }
    }

    private void SpawnEnemyNearPlayer()
    {
        Vector3 randomVector = Random.insideUnitSphere;
        randomVector.z = Mathf.Abs(randomVector.z);
        float dist = Random.Range(minRadiusToSpawnFromPlayer, maxRadiusToSpawnFromPlayer);
        randomVector *= dist;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomVector + playerEntity.transform.position, out hit, maxRadiusToSpawnFromPlayer, 1);
        Instantiate(enemyPrefab, hit.position, playerEntity.transform.rotation);
    }
}
