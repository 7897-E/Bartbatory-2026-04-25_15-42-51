using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Zomb Settings")]
    public EnemyScript ZombPrefab;
    public int ZombHealth = 10;
    public float ZombfollowSpeed = 5f;
    public int Zombdamage;
    public float ZombCooldown;
    public int ZombXp = 10;
    [Header("Cheetah Settings")]
    public EnemyScript CheetahPrefab;
    public int CheetahHealth = 10;
    public float CheetahfollowSpeed = 5f;
    public int Cheetahdamage;
    public float CheetahCooldown;
    public int CheetahXp = 15;
    [Header("Bulk Settings")]
    public EnemyScript BulkPrefab;
    public int BulkHealth = 10;
    public float BulKFallowSpeed = 5f;
    public int Bulkdamage;
    public float BulkCooldown;
     public int BulkXp= 20;
     [Header("BossBulk Settings")]
    public EnemyScript BossBulkPrefab;
    public int BossBulkHealth = 40;
    public float BossBulKFallowSpeed = 4f;
     public int BossBulkXp= 100;
    [Header("Boss Projectile")]
    public BossBall projectile;
    public float projectileSpeed;
    public int BossBulkdamage;
    public int BossProjectileDamage;
    public float BossBulkCooldown;
    public Camera cam;
    
    
    public EnemyScript SpawnZomb(MapGeneration map, Transform target, Vector3 position, PlayerController playerCharacter, int level)
    {
        EnemyScript enemyInstance = Instantiate(ZombPrefab, position, Quaternion.identity);
        enemyInstance.Init(map, target, ZombHealth, playerCharacter, ZombfollowSpeed, Zombdamage, ZombCooldown, ZombXp,false, 0f, projectile, cam, projectileSpeed, 0, level);
        return enemyInstance;
    }
    public EnemyScript SpawnCheetah(MapGeneration map, Transform target, Vector3 position, PlayerController playerCharacter, int level)
    {
        EnemyScript enemyInstance = Instantiate(CheetahPrefab, position, Quaternion.identity);
        enemyInstance.Init(map, target, CheetahHealth, playerCharacter, CheetahfollowSpeed, Cheetahdamage, CheetahCooldown, CheetahXp,false, 0f, projectile, cam, projectileSpeed, 0, level);
        return enemyInstance;
    }
    public EnemyScript SpawnBulk(MapGeneration map, Transform target, Vector3 position, PlayerController playerCharacter, int level)
    {
        EnemyScript enemyInstance = Instantiate(BulkPrefab, position, Quaternion.identity);
        enemyInstance.Init(map, target, BulkHealth, playerCharacter, BulKFallowSpeed, Bulkdamage, BulkCooldown, BulkXp, false,0f, projectile, cam, projectileSpeed, 0, level);
        return enemyInstance;
    }
    public EnemyScript SpawnBossBulk(MapGeneration map, Transform target, Vector3 position, PlayerController playerCharacter, int level)
    {
        EnemyScript enemyInstance = Instantiate(BossBulkPrefab, position, Quaternion.identity);

        enemyInstance.target = target;
        enemyInstance.PlayerCharacter = playerCharacter;
        enemyInstance.gameManager = FindObjectOfType<GameManager>();
        enemyInstance.canShoot = true;

        enemyInstance.Init(map, target, BossBulkHealth, playerCharacter, BossBulKFallowSpeed, BossBulkdamage, BossBulkCooldown, BossBulkXp, true, .7f , projectile,cam, projectileSpeed, BossProjectileDamage, level);

        

        return enemyInstance;
    }
}
