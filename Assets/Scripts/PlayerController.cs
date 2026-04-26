using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private MapGeneration m_Board;
    private Vector2Int m_CellPosition;

    public float moveSpeed = 5f;

    private Vector3 m_TargetWorldPos;
    private bool m_IsMoving = false;

    [Header("Player Stats")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("XP")]
    public int currentXP = 0;
    public int maxXP = 100;

    [Header("UI")]
    public GameObject uiDocumentObject;
    public float barAnimationSpeed = 8f;

    public GameUI gameUI;

    public UpgradeUIController upgrades;

    private UIDocument uiDocument;
    private ProgressBar healthBar;
    private ProgressBar xpBar;

    private float displayedHealth;
    private float displayedXP;

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
        currentXP = 0;

        SetupUI();
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
        AnimateBars();
    }

    private void SetupUI()
    {
        if (uiDocumentObject == null)
        {
            Debug.LogError("UI Document GameObject is not assigned!");
            return;
        }

        uiDocument = uiDocumentObject.GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("Assigned GameObject does not have a UIDocument!");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;

        healthBar = root.Q<ProgressBar>("HealthBar");
        xpBar = root.Q<ProgressBar>("XPBar");

        if (healthBar == null)
            Debug.LogError("Could not find ProgressBar named HealthBar");

        if (xpBar == null)
            Debug.LogError("Could not find ProgressBar named XPBar");

        displayedHealth = currentHealth;
        displayedXP = currentXP;

        UpdateBarLimits();
        ForceUpdateBars();
    }

    private void UpdateBarLimits()
    {
        if (healthBar != null)
        {
            healthBar.lowValue = 0;
            healthBar.highValue = maxHealth;
        }

        if (xpBar != null)
        {
            xpBar.lowValue = 0;
            xpBar.highValue = maxXP;
        }
    }

    private void ForceUpdateBars()
    {
        if (healthBar != null)
        {
            healthBar.value = displayedHealth;
            healthBar.title = currentHealth + " / " + maxHealth;
        }

        if (xpBar != null)
        {
            xpBar.value = displayedXP;
            xpBar.title = currentXP + " / " + maxXP;
        }
    }

    private void AnimateBars()
    {
        if (healthBar != null)
        {
            displayedHealth = Mathf.Lerp(
                displayedHealth,
                currentHealth,
                Time.deltaTime * barAnimationSpeed
            );

            healthBar.value = displayedHealth;
            healthBar.title = currentHealth + " / " + maxHealth;
        }

        if (xpBar != null)
        {
            displayedXP = Mathf.Lerp(
                displayedXP,
                currentXP,
                Time.deltaTime * barAnimationSpeed
            );

            xpBar.value = displayedXP;
            xpBar.title = currentXP + " / " + maxXP;
        }
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

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateBarLimits();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        if (currentXP >= maxXP)
        {
            currentXP -= maxXP;
            LevelUp();
        }

        UpdateBarLimits();
    }

    private void LevelUp()
    {
        currentHealth = maxHealth;

        maxXP += (maxXP + (int)(maxXP * 0.3));
        upgrades.ShowRandomUpgrades();
        UpdateBarLimits();
    }

    private void Die()
    {
        currentXP= 0;
        Debug.Log("Player died!");
        gameUI.ShowDeathScreen();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
        XPOrb xpOrb = collision.gameObject.GetComponent<XPOrb>();
        if (enemy != null)
        {
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
                    cooldownRemaining = 0f
                };

                CollidingEnemies.Add(data);
            }
        }
        if (xpOrb != null)
        {
            AddXP(xpOrb.XPValue);
            Destroy(xpOrb.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();

        if (enemy != null)
        {
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