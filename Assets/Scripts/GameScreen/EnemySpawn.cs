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
    [SerializeField] private Transform[] shooterPoints;
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
            EnemySO randomEnemy = enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Length)];
            GameObject enemyObj = EnemyPool.Instance.GetEnemy(randomEnemy);

            if (CanSpawnEnemy(enemyObj))
            {
                SpawnEnemy(enemyObj, randomEnemy);
            }
            else
            {
                EnemyPool.Instance.ReturnEnemy(randomEnemy, enemyObj);
            }

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

    private bool CanSpawnEnemy(GameObject enemy)
    {
        if (enemy.TryGetComponent<ShooterEnemyLogic>(out ShooterEnemyLogic shooterLogic))
        {
            return FindObjectsByType<ShooterEnemyLogic>(FindObjectsSortMode.None).Length < 4;
        }

        return true;
    }

    private void SpawnEnemy(GameObject enemy, EnemySO enemyData)
    {
        if (enemy.TryGetComponent<ShooterEnemyLogic>(out ShooterEnemyLogic shooterLogic))
        {
            Transform shooterSpawnPoint = GetAvailableShooterSpawnPoint();
            if (shooterSpawnPoint != null)
            {
                enemy.transform.position = shooterSpawnPoint.position;
                shooterLogic.Initialize(enemyData, player.transform);
                OnEnemySpawn?.Invoke(shooterLogic);
            }
            else
            {
                EnemyPool.Instance.ReturnEnemy(enemyData, enemy);
            }
        }
        else
        {
            EnemyLogic logic = enemy.GetComponent<EnemyLogic>();
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            enemy.transform.position = spawnPoint.position;
            logic.Initialize(enemyData, player.transform);
            OnEnemySpawn?.Invoke(logic);
        }
    }

    private Transform GetAvailableShooterSpawnPoint()
    {
        Transform[] shuffledPoints = new Transform[shooterPoints.Length];
        shooterPoints.CopyTo(shuffledPoints, 0);

        for (int i = 0; i < shuffledPoints.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, shuffledPoints.Length);
            Transform temp = shuffledPoints[i];
            shuffledPoints[i] = shuffledPoints[randomIndex];
            shuffledPoints[randomIndex] = temp;
        }

        foreach (Transform spawnPoint in shuffledPoints)
        {
            if (!IsShooterAtPosition(spawnPoint.position))
            {
                return spawnPoint;
            }
        }

        return null;
    }

    private bool IsShooterAtPosition(Vector3 position)
    {
        float checkRadius = 1.0f;

        ShooterEnemyLogic[] activeShooters = FindObjectsByType<ShooterEnemyLogic>(FindObjectsSortMode.None);

        foreach (ShooterEnemyLogic shooter in activeShooters)
        {
            if (shooter.gameObject.activeInHierarchy &&
                Vector3.Distance(shooter.transform.position, position) < checkRadius)
            {
                return true;
            }
        }

        return false;
    }
}