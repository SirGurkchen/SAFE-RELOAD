using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pool of Bullet objects.
/// </summary>
public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private int poolSize = 50;

    private Queue<GameObject> bulletQueue = new Queue<GameObject>();

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

        CreatePool(poolSize);
    }

    private void CreatePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            var obj = Instantiate(bulletPrefab);
            obj.SetActive(false);
            bulletQueue.Enqueue(obj);
        }
    }

    public GameObject GetBullet()
    {
        if (bulletQueue.Count > 0)
        {
            var bullet = bulletQueue.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            return Instantiate(bulletPrefab);
        }
    }

    public void ReturnBullet(GameObject obj)
    {
        obj.SetActive(false);
        bulletQueue.Enqueue(obj);
    }
}