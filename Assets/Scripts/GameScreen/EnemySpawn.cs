using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the Enemy Spawn.
/// This includes the correct enemy spawn on the given spawnpoints,
/// and the handling of the spawn time.
/// </summary>
public class EnemySpawn : MonoBehaviour
{
    public event Action<EnemyLogic> OnEnemySpawn;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnTime = 2.5f;
    [SerializeField] private EnemySO[] enemyTypes;
    [SerializeField] private GameObject player;

    private const float MAX_SPAWNTIME = 2.25f;
    private const float SPAWNTIME_SUBTRACTION_MULTIPLIER = 0.15f;
    private const float MIN_SPAWNTIME = 1.25f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnTime);

            SpawnEnemy();

            if (spawnTime > MIN_SPAWNTIME)
            {
                spawnTime = MAX_SPAWNTIME - (EnemyPool.Instance.GetActiveEnemyCount() * SPAWNTIME_SUBTRACTION_MULTIPLIER);
            }
            else if (spawnTime < MIN_SPAWNTIME)
            {
                spawnTime = MIN_SPAWNTIME;
            }
        }
    }

    private void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        EnemySO randomEnemy = enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Length)];

        GameObject enemyObj = EnemyPool.Instance.GetEnemy(randomEnemy);
        enemyObj.transform.position = spawnPoint.position;

        var logic = enemyObj.GetComponent<EnemyLogic>();
        logic.Initialize(randomEnemy, player.transform);

        OnEnemySpawn?.Invoke(logic);
    }
}