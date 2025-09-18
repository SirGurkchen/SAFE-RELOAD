using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

/// <summary>
/// Pool of enemies and their types.
/// </summary>
public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    [SerializeField] private List<EnemySO> enemyTypes;

    private Dictionary<EnemySO, Queue<GameObject>> pools = new Dictionary<EnemySO, Queue<GameObject>>();

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
        
        foreach(EnemySO enemy in enemyTypes)
        {
            pools[enemy] = CreatePool(enemy, 105);
        }
    }

    private Queue<GameObject> CreatePool(EnemySO data, int poolSize)
    {
        var queue = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(data.prefab);
            obj.SetActive(false);
            queue.Enqueue(obj);
        }
        return queue;
    }

    public GameObject GetEnemy(EnemySO data)
    {
        if (pools[data].Count > 0)
        {
            var obj = pools[data].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(data.prefab);
    }

    public void ReturnEnemy(EnemySO data, GameObject obj)
    {
        obj.SetActive(false);
        pools[data].Enqueue(obj);
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