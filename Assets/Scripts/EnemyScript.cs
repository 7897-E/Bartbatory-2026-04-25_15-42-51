using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private MapGeneration m_Board;
    [Header("References Inherited by Spawner")]
    public Transform target;
    public PlayerController PlayerCharacter;

    public Vector3 offset = new Vector3(0f, 0f, 0f);

    public float followSpeed = 5f;

    public XPOrb XPOrbPrefab;
    [Header("XP Settings Inherited by Spawner")]
    public int XP = 10;

    [Header("Damage Settings Inherited by Spawner")]
    public int Damage;
    public float cooldown;

    [Header("Health Settings Inherited by Spawner")]
    public int maxHealth = 10;
    public int currentHealth;

    // 👇 NEW: Sprites
    [Header("Sprites")]
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private SpriteRenderer sr;

    public void Init(MapGeneration mapGeneration, Transform targetTransform, int h, PlayerController playerCharacter, float followSpeed, int Zombdamage, float ZombCooldown, int xp)
    {
        this.followSpeed = followSpeed;
        this.Damage = Zombdamage;
        this.cooldown = ZombCooldown;

        m_Board = mapGeneration;
        target = targetTransform;
        maxHealth = h;
        currentHealth = maxHealth;
        PlayerCharacter = playerCharacter;
        XP = xp;
    }

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 previousPosition = transform.position;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.MoveTowards(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        // 👇 Calculate movement direction
        Vector2 moveDir = (transform.position - previousPosition);

        UpdateSprite(moveDir);
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
        xpOrb.Init(XP);
    }

    Destroy(gameObject);
}
}