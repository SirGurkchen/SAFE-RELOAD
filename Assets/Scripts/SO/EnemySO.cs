using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/Enemy Data")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public GameObject prefab;
    public int maxHealth;
    public float moveSpeed;
    public int scoreToGive;
    public int damageToGive;
}
