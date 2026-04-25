using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private float speed = 10f;
    private int damage = 1;
    private int maxHits = 1;   
    private int currentHits = 0;

    private Vector3 _direction = Vector3.up; 

    public void Init(Vector3 direction, int damages, float speed,  int maxHits)
    {
        _direction = direction.normalized;
        damage = damages;
        this.speed = speed;
        this.maxHits = maxHits;
    }

   

    private void Update()
    {
        transform.position += _direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyScript enemy = other.GetComponent<EnemyScript>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            currentHits++;

            if (currentHits >= maxHits)
            {
                Destroy(gameObject);
            }
        }
        
    }
}
