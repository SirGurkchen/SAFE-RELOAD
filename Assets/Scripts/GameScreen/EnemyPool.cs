using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

/// <summary>
/// Pool of enemies and their types.
/// </summary>
public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private GameObject strongEnemyPrefab;

    [SerializeField] private GameObject fastEnemyPrefab;

    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

    private List<GameObject> activeEnemyList;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        activeEnemyList = new List<GameObject>();
        pools["Enemy"] = CreatePool(enemyPrefab, 30);
        pools["StrongEnemy"] = CreatePool(strongEnemyPrefab, 30);
        pools["FastEnemy"] = CreatePool(fastEnemyPrefab, 30);
    }

    private Queue<GameObject> CreatePool(GameObject enemyPrefab, int poolSize)
    {
        var queue = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(enemyPrefab);
            obj.SetActive(false);
            queue.Enqueue(obj);
        }
        return queue;
    }

    public GameObject GetEnemy(string type)
    {
        var pool = pools[type];
        if (pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(type == "Enemy" ? enemyPrefab : strongEnemyPrefab);
    }

    public void ReturnEnemy(string type, GameObject obj)
    {
        obj.SetActive(false);
        pools[type].Enqueue(obj);
    }

    public void AddEnemyToList(GameObject enemy)
    {
        activeEnemyList.Add(enemy);
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        activeEnemyList.Remove(enemy);
    }

    public int GetActiveEnemyCount()
    {
        int count = 0;
        foreach (GameObject enemy in activeEnemyList)
        {
            count++;
        }
        return count;
    }
}