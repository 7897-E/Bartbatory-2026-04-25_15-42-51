using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private MapGeneration m_Board;
    public Transform target;

    public Vector3 offset = new Vector3(0f, 0f, 0f);
    public float followSpeed = 5f;

    [Header("Damage Settings")]
    public int Damage;
    public float cooldown;

    [Header("Health Settings")]
    public int maxHealth = 10;
    public int currentHealth;

    public void Init(MapGeneration mapGeneration, Transform targetTransform, int h)
    {
        m_Board = mapGeneration;
        target = targetTransform;
        maxHealth = h;
        currentHealth = maxHealth;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );
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
        Destroy(gameObject);
    }
}
