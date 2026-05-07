using UnityEngine;

public class XPOrb : MonoBehaviour
{
    public float minSpeed = 2f;
    public float maxSpeed = 10f;
    public float maxDistance = 20f;
    [Header("XP Settings Inherited From EnemySpawner")]
    public int XPValue = 10;
    public Transform target;
    

    public void Init(int xp, Transform ta)
    {
        XPValue = xp;
        target = ta;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.AddXP(XPValue);
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        float factor = 1f - Mathf.Clamp01(distance / maxDistance);

        float speed = Mathf.Lerp(minSpeed, maxSpeed, factor);

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }
}

