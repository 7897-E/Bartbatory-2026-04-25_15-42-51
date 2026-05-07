using UnityEngine;

public class EnemyScript : MonoBehaviour
{   public GameManager gameManager;
    private MapGeneration m_Board;
    [Header("References Inherited by Spawner")]
    public Transform target;
    public PlayerController PlayerCharacter;

    public Vector3 offset = new Vector3(0f, 0f, 0f);

    public float followSpeed = 5f;

    public XPOrb XPOrbPrefab;
    public Camera cam;
    [Header("XP Settings Inherited by Spawner")]
    public int XP = 10;

    [Header("Damage Settings Inherited by Spawner")]
    public int Damage;
    public int projectileDamage;
    public float cooldown;

    [Header("Health Settings Inherited by Spawner")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("Sprites")]
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private float activeFollowSpeed;

    [Header("Shooting (Boss Only)")]
    public bool canShoot = false;
    public BossBall projectilePrefab;
    public float shootCooldown = 2f;
    private float shootTimer;
    public float projectileSpeed;
    public float projectileRadius = 5f;
    [Header("Fan Settings")]
    public int bulletsPerShot = 5;          
    public float fanAngle = 60f;

    public int level =0;
    public void Init(MapGeneration mapGeneration, Transform targetTransform, int h, PlayerController playerCharacter, float followSpeed, int Zombdamage, float ZombCooldown, int xp, bool canShoot, float shootCooldown, BossBall Projectile, Camera camera, float speed, int projectileDamage, int level = 1)
    {
        this.followSpeed = followSpeed * (1 + 0.3f * level);
        this.Damage = (int)(Zombdamage * (1 + .5f * level));
        this.cooldown = ZombCooldown * Mathf.Max(0.5f, 1 - 0.05f * level);

        m_Board = mapGeneration;
        target = targetTransform;
        maxHealth = h* level;
        currentHealth = maxHealth;
        PlayerCharacter = playerCharacter;
        XP = xp * level;
        this.canShoot = canShoot;
        projectilePrefab= Projectile;
        this.shootCooldown = shootCooldown;
        projectileSpeed = speed;
        this.cam = camera;
        this.projectileDamage = projectileDamage;
}

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        activeFollowSpeed = followSpeed;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 previousPosition = rb != null ? (Vector3)rb.position : transform.position;

        Vector3 desiredPosition = target.position + offset;
        Vector2 desiredPosition2D = new Vector2(desiredPosition.x, desiredPosition.y);
        Vector2 currentPosition = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 direction = desiredPosition2D - currentPosition;
        Vector2 velocity = Vector2.zero;

        if (direction.sqrMagnitude > 0.0001f)
        {
            Vector2 moveInput = direction.normalized;
            activeFollowSpeed = Mathf.Lerp(activeFollowSpeed, followSpeed, Time.deltaTime * 10f);
            velocity = moveInput * activeFollowSpeed;
        }

        if (rb != null)
        {
            rb.velocity = velocity;
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                desiredPosition,
                followSpeed * Time.deltaTime
            );
        }

  
        Vector2 moveDir = (transform.position - previousPosition);
        if (canShoot)
{
         shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            ShootAtPlayer();
            shootTimer = shootCooldown;
        }   
}
        UpdateSprite(moveDir);
    }
    void ShootAtPlayer()
    {
        if (projectilePrefab == null || PlayerCharacter == null) return;

        Vector3 basePos = transform.position;
        Vector3 playerPos = PlayerCharacter.transform.position;

        Vector3 toPlayer = (playerPos - basePos).normalized;

        Quaternion baseRot = Quaternion.LookRotation(Vector3.forward, toPlayer);


        int count = Mathf.Max(1, bulletsPerShot);
        float totalAngle = fanAngle;

        if (count == 1)
        {
            Vector3 spawnPos = basePos + toPlayer * projectileRadius;
            BossBall proj = Instantiate(projectilePrefab, spawnPos, baseRot);
            proj.Init(toPlayer, projectileDamage, projectileSpeed, 0, 0, cam, true);
            return;
        }

        float step = totalAngle / (count - 1); 

        float startAngle = -totalAngle * 0.5f; 

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + step * i;    

            Quaternion rot = baseRot * Quaternion.AngleAxis(angle, Vector3.forward);
            Vector3 dir = rot * Vector3.up;        

            Vector3 spawnPos = basePos + dir * projectileRadius;

            BossBall proj = Instantiate(projectilePrefab, spawnPos, rot);
            proj.Init(dir, Damage, projectileSpeed, 0, 0, cam, true);
        }
    }

    private void UpdateSprite(Vector2 moveDir)
    {
        if (moveDir == Vector2.zero) return;

        if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
        {
            sr.sprite = moveDir.x > 0 ? rightSprite : leftSprite;
        }
        else
        {
            sr.sprite = moveDir.y > 0 ? upSprite : downSprite;
        }
    }

    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
{   
    if (XPOrbPrefab != null)
    {
        XPOrb xpOrb = Instantiate(XPOrbPrefab, transform.position, Quaternion.identity);
        xpOrb.Init(XP,target);
    }
    if (gameManager != null)
    {
        gameManager.OnBossDefeated();
    }
    Destroy(gameObject);
}
}