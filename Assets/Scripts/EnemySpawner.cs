using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Zomb Settings")]
    public EnemyScript ZombPrefab;
    public int ZombHealth = 10;
    public float ZombfollowSpeed = 5f;
    public int Zombdamage;
    public float ZombCooldown;
    [Header("Cheetah Settings")]
    public EnemyScript CheetahPrefab;
    public int CheetahHealth = 10;
    public float CheetahfollowSpeed = 5f;
    public int Cheetahdamage;
    public float CheetahCooldown;
    [Header("Bulk Settings")]
    public EnemyScript BulkPrefab;
    public int BulkHealth = 10;
    public float BulKFallowSpeed = 5f;
    public int Bulkdamage;
    public float BulkCooldown;
    public EnemyScript SpawnZomb(MapGeneration map, Transform target, Vector3 position, PlayerController playerCharacter)
    {
        EnemyScript enemyInstance = Instantiate(ZombPrefab, position, Quaternion.identity);
        enemyInstance.Init(map, target, ZombHealth, playerCharacter, ZombfollowSpeed, Zombdamage, ZombCooldown);
        return enemyInstance;
    }
    public EnemyScript SpawnCheetah(MapGeneration map, Transform target, Vector3 position, PlayerController playerCharacter)
    {
        EnemyScript enemyInstance = Instantiate(CheetahPrefab, position, Quaternion.identity);
        enemyInstance.Init(map, target, CheetahHealth, playerCharacter, CheetahfollowSpeed, Cheetahdamage, CheetahCooldown);
        return enemyInstance;
    }
    public EnemyScript SpawnBulk(MapGeneration map, Transform target, Vector3 position, PlayerController playerCharacter)
    {
        EnemyScript enemyInstance = Instantiate(BulkPrefab, position, Quaternion.identity);
        enemyInstance.Init(map, target, BulkHealth, playerCharacter, BulKFallowSpeed, Bulkdamage, BulkCooldown);
        return enemyInstance;
    }
}
