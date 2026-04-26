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
}
