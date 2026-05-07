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
     [Header("Boss1 Settings")]
    public EnemyScript Boss1Prefab;
    public int Boss1Health = 40;
    public float Boss1FallowSpeed = 4f;
     public int Boss1Xp= 100;
    [Header("Boss1 Projectile")]
    public BossBall projectile;
    public float projectileSpeed;
    public int Boss1damage;
    public int BossProjectileDamage;
    public float Boss1Cooldown;
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
    public EnemyScript SpawnBoss1(MapGeneration map, Transform target, Vector3 position, PlayerController playerCharacter, int level)
    {
        EnemyScript enemyInstance = Instantiate(Boss1Prefab, position, Quaternion.identity);

        enemyInstance.target = target;
        enemyInstance.PlayerCharacter = playerCharacter;
        enemyInstance.gameManager = FindObjectOfType<GameManager>();
        enemyInstance.canShoot = true;

        enemyInstance.Init(map, target, Boss1Health, playerCharacter, Boss1FallowSpeed, Boss1damage, Boss1Cooldown, Boss1Xp, true, .7f , projectile,cam, projectileSpeed, BossProjectileDamage, level);

        

        return enemyInstance;
    }
    public EnemyScript SpawnBoss2(MapGeneration map, Transform target, Vector3 position, PlayerController playerCharacter, int level)
    {
        EnemyScript enemyInstance = Instantiate(Boss1Prefab, position, Quaternion.identity);

        enemyInstance.target = target;
        enemyInstance.PlayerCharacter = playerCharacter;
        enemyInstance.gameManager = FindObjectOfType<GameManager>();
        enemyInstance.canShoot = true;

        enemyInstance.Init(map, target, Boss1Health, playerCharacter, Boss1FallowSpeed, Boss1damage, Boss1Cooldown, Boss1Xp, true, .7f, projectile, cam, projectileSpeed, BossProjectileDamage, level);



        return enemyInstance;
    }
}
