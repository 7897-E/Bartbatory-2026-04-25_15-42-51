using UnityEngine;

public class XPOrb : MonoBehaviour
{
    [Header("XP Settings Inherited From EnemySpawner")]
    public int XPValue = 10;

    public void Init(int xp)
    {
        XPValue = xp;
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
}