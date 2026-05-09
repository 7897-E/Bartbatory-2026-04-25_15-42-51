using UnityEngine;
using System.Collections.Generic;

public class BatScript : MonoBehaviour
{
    public enum WeaponType { Bat, Minigun, Shotgun }
    [Header("References")]
    public Transform batPivot;     
    public Transform firePoint;    

    [Header("Rotation / Aim")]
    public float rotationSpeed = 720f;
    [Header("Bullet")]
    public Baseball bulletPrefab;
    public Baseball levelUpBulletPrefabObject;
    public Camera cam;
    public int currentLevel = 0;
    public float meleeOffsetDistance = 1.5f;   
    public LayerMask enemyLayer;
    [Header("Movement bat only")]
    public float batMoveSpeed = 10f;  
    [Header("Weapon Type Inherited")]
    public WeaponType weaponType = WeaponType.Bat;
    public int levelup = 5;
    [Header("Firing inherited")]
    public float fireRate = 0.3f;
    private float fireCooldown = 0f;
    public int meleeDamage = 1;
    public int projectileDamage = 1;
    public float meleeRange = 1f;
    [Header("Bullet inherited")]
    public float bulletSpeed = 10f;
    public int MaxHits = 1;
    public int bounces = 0;

    [Header("Shotgun Inherited")]
    public int shotgunPellets = 5;
    public float shotgunSpread = 30f;
    public float ShotgunRange = 10f;

    [Header("Flamethrower Inherited")]
    public float flamethrowerDamageMultiplier = 0.4f;
    public float flamethrowerFireRate = 0.1f;
    public float flamethrowerSpeedMultiplier = 0.8f;
    public float flamethrowerSpread = 25f;
    public float flamethrowerRange = 8f;
    public float flamethrowerFireDuration = 3f;
    public int flamethrowerFireDPS = 2;
    public int flamethrowerFlameCount = 8;
    private float flamethrowerCooldown = 0f;

    [Header("Swing (Bat only) inherited")]
    public float swingAngle = 90f;
    public float swingDuration = 0.15f;
    private bool isSwinging = false;
    private float swingTimer = 0f;
    private float startAngle;
    public int levelsUntilRanged = 5;

     

    [Header("Side Offset (relative to player)")]
    public Vector3 rightOffset = new Vector3(0f, 0f, 0f);
    public Vector3 leftOffset = new Vector3(0f, 0f, 0f);

    private Transform player;
    private EnemyScript currentTarget;
    private bool meleeHitThisSwing;

    private static Dictionary<BatScript, EnemyScript> weaponTargets = new();

    private void Awake()
    {
        player = transform.parent;
        
        if (batPivot == null)
            batPivot = transform;
    }

    private void Update()
    {
        EnemyScript nearestEnemy = FindNearestEnemy();
        
        if (nearestEnemy != currentTarget)
        {
            if (currentTarget != null && weaponTargets.ContainsKey(this) && weaponTargets[this] == currentTarget)
            {
                weaponTargets.Remove(this);
            }
            currentTarget = nearestEnemy;
            if (currentTarget != null)
            {
                weaponTargets[this] = currentTarget;
            }
        }

        if (nearestEnemy != null)
        {
            if(currentLevel < levelsUntilRanged)
                UpdateSide(nearestEnemy.transform.position);
                AimAt(nearestEnemy.transform.position);
                FireAt(nearestEnemy.transform.position);

        }
        
        else
        {
            if (weaponTargets.ContainsKey(this))
            {
                weaponTargets.Remove(this);
            }
            currentTarget = null;
        }
        if(currentLevel >= levelup){
        nearestEnemy = FindNearestEnemy();
        
        if (nearestEnemy != currentTarget)
        {
            if (currentTarget != null && weaponTargets.ContainsKey(this) && weaponTargets[this] == currentTarget)
            {
                weaponTargets.Remove(this);
            }
            currentTarget = nearestEnemy;
            if (currentTarget != null)
            {
                weaponTargets[this] = currentTarget;
            }
        }
        if (nearestEnemy != null){
        if(currentLevel < levelsUntilRanged)
                UpdateSide(nearestEnemy.transform.position);
                AimAt(nearestEnemy.transform.position);
                FireUp(nearestEnemy.transform.position);
        }
        }
        if (weaponType == WeaponType.Bat)
        {
            UpdateSwing();
        }

        
    }

    private void AimAt(Vector3 targetPos)
    {
        Vector3 dir = targetPos - batPivot.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);

        batPivot.rotation = Quaternion.RotateTowards(
            batPivot.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    private EnemyScript FindNearestEnemy()
    {
        EnemyScript[] enemies = FindObjectsOfType<EnemyScript>();
        if (enemies.Length == 0) return null;

        EnemyScript nearest = null;
        float nearestDistSqr = float.MaxValue;
        Vector3 currentPos = batPivot.position;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            if (weaponTargets.ContainsValue(enemy)) continue;

            float distSqr = (enemy.transform.position - currentPos).sqrMagnitude;
            if (distSqr < nearestDistSqr)
            {
                nearestDistSqr = distSqr;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private void FireAt(Vector3 targetPos)

    {
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;       
            return; 
        }        
        switch (weaponType)
        {
            case WeaponType.Bat:
                if ( currentLevel >= levelsUntilRanged)
                {
                        StartSwing(targetPos);
                       FireBullet(targetPos);
                }

                if (!isSwinging && IsEnemyInMeleeRange())
                {
                    StartSwing(targetPos);
                }
                break;
            case WeaponType.Minigun:
                FireBullet(targetPos);
                break;
            case WeaponType.Shotgun:
                if(currentLevel >= levelup){
                    FireShotgun(targetPos);
                }else{
                    FireShotgun(targetPos);
                }
                break;
        }
        
        fireCooldown = fireRate;
    }

    private void FireUp(Vector3 targetPos)
    {
        if (flamethrowerCooldown > 0f) {flamethrowerCooldown-= Time.deltaTime;return;}

        switch (weaponType)
        {
            case WeaponType.Bat:
                break;
            case WeaponType.Minigun:
                break;
            case WeaponType.Shotgun:
                FireFlames(targetPos);
                break;
        }

        flamethrowerCooldown = flamethrowerFireRate;
    }
    private void FireBullet(Vector3 targetPos)
    {
        Transform spawnTransform = firePoint != null ? firePoint : batPivot;
        Vector3 dir = (targetPos - spawnTransform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, dir);

        Baseball bullet = Instantiate(
            bulletPrefab,
            spawnTransform.position,
            rot
        );

        bullet.Init(dir, projectileDamage, bulletSpeed, MaxHits, bounces, cam, false, 0);
    }
 
        

    private void FireShotgun(Vector3 targetPos)
    {
        Transform spawnTransform = firePoint != null ? firePoint : batPivot;
        Vector3 baseDir = (targetPos - spawnTransform.position).normalized;

        for (int i = 0; i < shotgunPellets; i++)
        {
            float angleOffset = Random.Range(-shotgunSpread / 2f, shotgunSpread / 2f) * Mathf.Deg2Rad;
            Vector3 dir = Quaternion.Euler(0, 0, angleOffset * Mathf.Rad2Deg) * baseDir;

            Baseball bullet = Instantiate(
                bulletPrefab,
                spawnTransform.position,
                Quaternion.LookRotation(Vector3.forward, dir)
            );

            bullet.Init(dir, projectileDamage, bulletSpeed, MaxHits, bounces, cam, true, ShotgunRange);
        }
    }
    private void FireFlames(Vector3 targetPos)
    {
        Transform spawnTransform = firePoint != null ? firePoint : batPivot;
        Vector3 baseDir = (targetPos - spawnTransform.position).normalized;
        Baseball prefabToUse = levelUpBulletPrefabObject != null ? levelUpBulletPrefabObject : bulletPrefab;

        if (prefabToUse == null)
        {
            Debug.LogWarning($"No bullet prefab assigned for weapon type {weaponType}");
            return;
        }

        int flamethrowerDamage = Mathf.Max(1, Mathf.RoundToInt(projectileDamage * flamethrowerDamageMultiplier));
        float flamethrowerSpeed = bulletSpeed * flamethrowerSpeedMultiplier;

        for (int i = 0; i < flamethrowerFlameCount; i++)
        {
            float angleOffset = Random.Range(-flamethrowerSpread / 2f, flamethrowerSpread / 2f) * Mathf.Deg2Rad;
            Vector3 dir = Quaternion.Euler(0, 0, angleOffset * Mathf.Rad2Deg) * baseDir;

            Baseball bullet = Instantiate(
                prefabToUse,
                spawnTransform.position,
                Quaternion.LookRotation(Vector3.forward, dir)
            ).GetComponent<Baseball>();

            if (bullet == null)
            {
                Debug.LogError($"Prefab {prefabToUse.name} does not have a Baseball component");
                continue;
            }

            bullet.Init(dir, flamethrowerDamage, flamethrowerSpeed, MaxHits, bounces, cam, true, flamethrowerRange, true, true, flamethrowerFireDuration, flamethrowerFireDPS);
        }
    }

    

    private void UpdateSide(Vector3 targetPos)
    {
    if (player == null || isSwinging) return;

    Vector3 dir = (targetPos - player.position).normalized;
    Vector3 targetLocalPos = dir * meleeOffsetDistance;

    batPivot.localPosition = Vector3.MoveTowards(
        batPivot.localPosition,
        targetLocalPos,
        batMoveSpeed * Time.deltaTime
    );
    }
private void StartSwing(Vector3 targetPos)
{
    isSwinging = true;
    swingTimer = 0f;
    meleeHitThisSwing = false;

    Vector3 dir = (targetPos - batPivot.position).normalized;
    float angleToTarget = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    startAngle = angleToTarget;
}

private void UpdateSwing()
{
    if (!isSwinging) return;

    swingTimer += Time.deltaTime;
    float t = swingTimer / swingDuration;

    if (t >= 1f)
    {
        isSwinging = false;
        meleeHitThisSwing = false;
        return;
    }

    if (!meleeHitThisSwing && t >= 0.4f)
        PerformMeleeHit();

    float curve = Mathf.Sin(t * Mathf.PI);

    float currentAngle = startAngle + swingAngle * (curve - 0.5f);

    batPivot.rotation = Quaternion.Euler(0f, 0f, currentAngle);
}
private bool IsEnemyInMeleeRange()
{
    int mask = enemyLayer.value;
    if (mask == 0)
        mask = Physics2D.DefaultRaycastLayers;

    Collider2D hit = Physics2D.OverlapCircle(batPivot.position, meleeRange, mask);
    return hit != null && hit.GetComponent<EnemyScript>() != null;
}
    private void PerformMeleeHit()
    {
        meleeHitThisSwing = true;

        int layerMask = enemyLayer.value;
        if (layerMask == 0)
        {
            layerMask = Physics2D.DefaultRaycastLayers;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(batPivot.position, meleeRange, layerMask);
        foreach (var hit in hits)
        {
            EnemyScript enemy = hit.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.TakeDamage(meleeDamage);
            }
        }
    }

    private void OnDestroy()
    {
        if (weaponTargets.ContainsKey(this))
        {
            weaponTargets.Remove(this);
        }
    }
}
