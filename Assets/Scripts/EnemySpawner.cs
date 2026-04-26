using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyScript enemyPrefab;
    public int ZombHealth = 10;
    public EnemyScript SpawnEnemy(MapGeneration map, Transform target, Vector3 position, PlayerController playerCharacter)
    {
        EnemyScript enemyInstance = Instantiate(enemyPrefab, position, Quaternion.identity);
        enemyInstance.Init(map, target, ZombHealth, playerCharacter);
        return enemyInstance;
    }
}
