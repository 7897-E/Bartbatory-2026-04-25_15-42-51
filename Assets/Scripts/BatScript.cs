using UnityEngine;
using System.Collections.Generic;

public class BatScript : MonoBehaviour
{
    public enum WeaponType { Bat, Railgun, Shotgun }

    [Header("Weapon Type")]
    public WeaponType weaponType = WeaponType.Bat;

    [Header("References")]
    public Transform batPivot;     
    public Transform firePoint;    

    [Header("Rotation / Aim")]
    public float rotationSpeed = 720f;

    [Header("Firing")]
    public float fireRate = 0.3f;
    private float fireCooldown = 0f;
    public int damage;
    [Header("Bullet")]
    public float bulletSpeed = 10f;
    public int MaxHits = 1;
    public int bounces = 0;
    public Baseball bulletPrefab;
    public Camera cam;

    [Header("Railgun")]
    public float railgunRange = 20f;
    public LayerMask enemyLayer;

    [Header("Shotgun")]
    public int shotgunPellets = 5;
    public float shotgunSpread = 30f;

    [Header("Swing (Bat only)")]
    public float swingAngle = 90f;
    public float swingDuration = 0.15f;
    private bool isSwinging = false;
    private float swingTimer = 0f;
    private float startAngle;

    [Header("Side Offset (relative to player)")]
    public Vector3 rightOffset = new Vector3(0f, 0f, 0f);
    public Vector3 leftOffset = new Vector3(0f, 0f, 0f);

    private Transform player;
    private EnemyScript currentTarget;

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
            if (!isSwinging || weaponType != WeaponType.Bat)
            {
                AimAt(nearestEnemy.transform.position);
                
            }

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

        fireCooldown = fireRate;

        switch (weaponType)
        {
            case WeaponType.Bat:
                FireBullet(targetPos);
                if (fireCooldown >= .175f)
                {
                    StartSwing(targetPos);
                }
                break;
            case WeaponType.Railgun:
                FireRailgun(targetPos);
                break;
            case WeaponType.Shotgun:
                FireShotgun(targetPos);
                break;
        }
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

        bullet.Init(dir, damage, bulletSpeed, MaxHits, bounces, cam);
    }

    private void FireRailgun(Vector3 targetPos)
    {
        Transform spawnTransform = firePoint != null ? firePoint : batPivot;
        Vector3 dir = (targetPos - spawnTransform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(spawnTransform.position, dir, railgunRange, enemyLayer);
        if (hit.collider != null)
        {
            EnemyScript enemy = hit.collider.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
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

            bullet.Init(dir, damage, bulletSpeed, MaxHits, bounces, cam);
        }
    }

    private void UpdateSide(Vector3 targetPos)
    {
        if (player == null) return;
        if (isSwinging) return;
        if (targetPos.x >= player.position.x)
        {
            batPivot.localPosition = rightOffset;
        }
        else
        {
            batPivot.localPosition = leftOffset;
        }
    }

    private void StartSwing(Vector3 targetPos)
    {
        isSwinging = true;
        swingTimer = 0f;
        startAngle = batPivot.eulerAngles.z;
    }

    private void UpdateSwing()
    {
        if (!isSwinging) return;

        swingTimer += Time.deltaTime;
        float t = swingTimer / swingDuration;

        if (t >= 1f)
        {
            isSwinging = false;
            return;
        }

        float curve = Mathf.Sin(t * Mathf.PI);

        float direction = (batPivot.localPosition.x >= 0) ? 1f : -1f;

        float currentAngle = startAngle
                           - direction * (swingAngle * 0.5f)
                           + direction * swingAngle * curve;

        batPivot.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    private void OnDestroy()
    {
        if (weaponTargets.ContainsKey(this))
        {
            weaponTargets.Remove(this);
        }
    }
}
