using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private float speed = 10f;
    private int damage = 1;
    private int maxHits = 1;   
    private int currentHits = 0;

    private int bouncesLeft = 0;

    private Camera cam;
    private float destroyDistance = .5f;
    private Vector3 _direction = Vector3.up; 

    public void Init(Vector3 direction, int damages, float speed, int maxHits, int bounces, Camera camera)
    {
        _direction = direction.normalized;
        damage = damages;
        this.speed = speed;
        this.maxHits = maxHits;
        bouncesLeft = bounces;
        cam = camera;
    }
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void Update()
{
    transform.position += _direction * speed * Time.deltaTime;

    Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);

    if (viewportPos.x < -destroyDistance || viewportPos.x > 1 + destroyDistance ||
        viewportPos.y < -destroyDistance || viewportPos.y > 1 + destroyDistance)
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
    }
}
