using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyScript enemyPrefab;
    public int ZombHealth = 10;
    public EnemyScript SpawnEnemy(MapGeneration map, Transform target, Vector3 position)
    {
        EnemyScript enemyInstance = Instantiate(enemyPrefab, position, Quaternion.identity);
        enemyInstance.Init(map, target, ZombHealth);
        return enemyInstance;
    }
}
