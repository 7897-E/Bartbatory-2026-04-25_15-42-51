using UnityEngine;

public class BatScript : MonoBehaviour
{
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
    public BulletScript bulletPrefab;
    public Camera cam;

    [Header("Swing")]
    public float swingAngle = 90f;
    public float swingDuration = 0.15f;
    private bool isSwinging = false;
    private float swingTimer = 0f;
    private float startAngle;

    [Header("Side Offset (relative to player)")]
    public Vector3 rightOffset = new Vector3(0f, 0f, 0f);
    public Vector3 leftOffset = new Vector3(0f, 0f, 0f);

    private Transform player;

    private void Awake()
    {
        player = transform.parent;

        if (batPivot == null)
            batPivot = transform;
    }

    private void Update()
    {
        EnemyScript nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            if (!isSwinging)
            {
                AimAt(nearestEnemy.transform.position);
                UpdateSide(nearestEnemy.transform.position);
            }

            AutoFire(nearestEnemy.transform.position);
        }

        UpdateSwing();
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

            float distSqr = (enemy.transform.position - currentPos).sqrMagnitude;
            if (distSqr < nearestDistSqr)
            {
                nearestDistSqr = distSqr;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private void AutoFire(Vector3 targetPos)
    {
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
            return;
        }

        fireCooldown = fireRate;

        Transform spawnTransform = firePoint != null ? firePoint : batPivot;

        Vector3 dir = (targetPos - spawnTransform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, dir);

        BulletScript bullet = Instantiate(
            bulletPrefab,
            spawnTransform.position,
            rot
        );

        bullet.Init(dir, damage, bulletSpeed, MaxHits, cam);
        if (fireCooldown >= .175)
        {
            StartSwing(targetPos);
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

        float curve = Mathf.Sin(t * Mathf.PI); // 0 -> 1 -> 0

        float direction = (batPivot.localPosition.x >= 0) ? 1f : -1f;

        float currentAngle = startAngle
                           - direction * (swingAngle * 0.5f)
                           + direction * swingAngle * curve;

        batPivot.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
}
