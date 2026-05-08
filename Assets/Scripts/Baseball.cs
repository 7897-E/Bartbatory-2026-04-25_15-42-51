using UnityEngine;

public class Baseball : MonoBehaviour
{
    private float speed = 10f;
    private int damage = 1;
     public bool canHitPlayer = false;
    private int maxHits = 1;   
    private int currentHits = 0;

    private int bouncesLeft = 0;

    private Camera cam;
    private float destroyDistance = .5f;
    private Vector3 _direction = Vector3.up; 
    private bool shotgunMode = false;
    private float shotgunCounter = 0f;

    [Header("Sprite References")]
    public SpriteRenderer spriteRenderer;
    public Sprite normalSprite;
    public Sprite levelUpSprite;
    private bool isLevelUpBullet = false;

    [Header("Fire Effect")]
    public bool appliesFire = false;
    public float fireDuration = 3f;
    public int fireDamagePerSecond = 2;
   

    public void Init(Vector3 direction, int damages, float speed, int maxHits, int bounces, Camera camera, bool shotgun, float shotgunCount, bool levelUp = false, bool fire = false, float fireDur = 3f, int fireDps = 2)
    {
        _direction = direction.normalized;
        damage = damages;
        this.speed = speed;
        this.maxHits = maxHits;
        this.shotgunMode = shotgun;
        bouncesLeft = bounces;
        cam = camera;
        shotgunCounter = shotgunCount;
        isLevelUpBullet = levelUp;
        appliesFire = fire;
        fireDuration = fireDur;
        fireDamagePerSecond = fireDps;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            if (isLevelUpBullet && levelUpSprite != null)
            {
                spriteRenderer.sprite = levelUpSprite;
            }
            else if (!isLevelUpBullet && normalSprite != null)
            {
                spriteRenderer.sprite = normalSprite;
            }
        }
    }
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void Update()
{
    transform.position += _direction * speed * Time.deltaTime;
    
    Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        shotgunCounter-= Time.deltaTime;

    if (viewportPos.x < -destroyDistance || viewportPos.x > 1 + destroyDistance ||
        viewportPos.y < -destroyDistance || viewportPos.y > 1 + destroyDistance || (shotgunMode && shotgunCounter <= 0))
    {
        Destroy(gameObject);
    }
    
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyScript enemy = other.GetComponent<EnemyScript>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            
            // Apply fire effect if enabled
            if (appliesFire)
            {
                enemy.ApplyFireEffect(fireDuration, fireDamagePerSecond);
            }

            if (bouncesLeft > 0)
            {
                bouncesLeft--;
                _direction = Random.insideUnitCircle.normalized;
            }
            else
            {
                currentHits++;

                if (currentHits >= maxHits)
                {
                    Destroy(gameObject);
                }
            }
        }
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && canHitPlayer)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

