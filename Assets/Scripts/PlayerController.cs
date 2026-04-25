using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private MapGeneration m_Board;
    private Vector2Int m_CellPosition;

    [SerializeField] private float moveSpeed = 5f; // higher = faster movement

    private Vector3 m_TargetWorldPos;
    private bool m_IsMoving = false;
    [Header("Player Stats")]
    public int maxHealth = 10;
    public int currentHealth;
    private class EnemyData
    {
        public EnemyScript enemy;
        public float cooldownRemaining;
    }
    private List<EnemyData> CollidingEnemies = new List<EnemyData>();

    public void Spawn(MapGeneration MapGeneration, Vector2Int cell)
    {
        m_Board = MapGeneration;
        MoveTo(cell, snapInstantly: true);
        currentHealth = maxHealth;
    }

    public void MoveTo(Vector2Int cell, bool snapInstantly = false)
    {
        m_CellPosition = cell;
        m_TargetWorldPos = m_Board.CellToWorld(m_CellPosition);

        if (snapInstantly)
        {
            transform.position = m_TargetWorldPos;
            m_IsMoving = false;
        }
        else
        {
            m_IsMoving = true;
        }
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        Vector2 input = Vector2.zero;

        if (Keyboard.current.upArrowKey.isPressed)
            input.y += 1f;
        if (Keyboard.current.downArrowKey.isPressed)
            input.y -= 1f;
        if (Keyboard.current.rightArrowKey.isPressed)
            input.x += 1f;
        if (Keyboard.current.leftArrowKey.isPressed)
            input.x -= 1f;
        if (input.sqrMagnitude > 1f)
            input = input.normalized;
        Vector3 delta = new Vector3(input.x, input.y, 0f) * moveSpeed * Time.deltaTime;
        transform.position += delta;
        if (m_IsMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                m_TargetWorldPos,
                moveSpeed * Time.deltaTime
            );
            if (Vector3.Distance(transform.position, m_TargetWorldPos) < 0.001f)
            {
                transform.position = m_TargetWorldPos;
                m_IsMoving = false;
            }
        }
        UpdateDamages();
    }
    private void UpdateDamages()
    {
        if (CollidingEnemies.Count == 0)
            return;

        float dt = Time.deltaTime;

        for (int i = CollidingEnemies.Count - 1; i >= 0; i--)
        {
            EnemyData data = CollidingEnemies[i];

            if (data.enemy == null)
            {
                CollidingEnemies.RemoveAt(i);
                continue;
            }

            data.cooldownRemaining -= dt;

            if (data.cooldownRemaining <= 0f)
            {
                TakeDamage(data.enemy.Damage);

                data.cooldownRemaining = data.enemy.cooldown;
            }
        }
    }
    private void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player took {amount} damage. Health = {currentHealth}");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
        if (enemy != null)
        {
            // Only add if not already in the list
            bool alreadyTracked = false;
            foreach (var e in CollidingEnemies)
            {
                if (e.enemy == enemy)
                {
                    alreadyTracked = true;
                    break;
                }
            }

            if (!alreadyTracked)
            {
                var data = new EnemyData
                {
                    enemy = enemy,
                    cooldownRemaining = 0f // 0 so it hits immediately on first contact
                };
                CollidingEnemies.Add(data);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
        if (enemy != null)
        {
            // remove from list when we stop colliding
            for (int i = CollidingEnemies.Count - 1; i >= 0; i--)
            {
                if (CollidingEnemies[i].enemy == enemy)
                {
                    CollidingEnemies.RemoveAt(i);
                }
            }
        }
    }
}
