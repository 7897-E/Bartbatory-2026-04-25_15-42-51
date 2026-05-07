using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private MapGeneration m_Board;
    private Vector2Int m_CellPosition;

    public float moveSpeed = 5f;
    private float activeMoveSpeed;

    private Vector3 m_TargetWorldPos;


    [Header("Player Stats")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("Dash stats")]
    public float dashSpeed;
    public float dashLength = .05f, dashCooldown = .15f;
    private float dashCounter;
    private float dashCoolCounter;
    private TrailRenderer _trailRenderer;

    private bool m_IsDashing = false;
    private Vector3 m_DashDirection = Vector3.zero;
    private float dashRecoverSpeed = 10f;

    [Header("XP")]
    public int currentXP = 0;
    public int maxXP = 100;
    public int levelsForWeapon = 5; 

    public Weapons startingWeapon; 

    private int totalLevels = 0;
    public int TotalLevels => totalLevels;
    public Weapons currentWeapon;
    public Dictionary<Weapons, int> weaponUpgradeLevels = new();
    public Dictionary<Weapons, BatScript> weaponInstances = new();
    public int weaponCount = 0;
    
    [Header("UI")]
    public GameObject uiDocumentObject;
    public float barAnimationSpeed = 8f;

    public GameUI gameUI;
    public bool isPaused = false;

    public UpgradeUIController upgrades;

    private UIDocument uiDocument;
    private ProgressBar healthBar;
    private ProgressBar xpBar;
    private ProgressBar CD;

    private float displayedHealth;
    private float displayedXP;
    private float displayedCD;
    public DamageFlash damageFlash;

    [Header("Sprites")]
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public float scale = 1f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Vector2 moveInput = Vector2.zero;

    private class EnemyData
    {
        public EnemyScript enemy;
        public float cooldownRemaining;
    }

    private List<EnemyData> CollidingEnemies = new List<EnemyData>();

    public void Spawn(MapGeneration MapGeneration, Vector2Int cell)
    {
        sr = GetComponent<SpriteRenderer>();
        m_Board = MapGeneration;
        MoveTo(cell, snapInstantly: true);

        currentHealth = maxHealth;
        currentXP = 0;
        totalLevels = 0;
        weaponUpgradeLevels.Clear();
        weaponInstances.Clear();
        if (startingWeapon != null && weaponCount == 0)
        {
            currentWeapon = startingWeapon;
            weaponUpgradeLevels[startingWeapon] = 0;
            weaponCount = 1;
            GameObject weaponPivot = startingWeapon.Apply(this, upgrades.weaponHolder, upgrades.playerCamera, 0);
            BatScript bat = weaponPivot.GetComponentInChildren<BatScript>();
            if (bat != null)
            {
                weaponInstances[startingWeapon] = bat;
            }
        }
        activeMoveSpeed = moveSpeed;
        dashSpeed = moveSpeed * 2.5f;
        _trailRenderer = GetComponent<TrailRenderer>();

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 0f;

        SetupUI();
    }

    public void MoveTo(Vector2Int cell, bool snapInstantly = false)
    {
        m_CellPosition = cell;
        m_TargetWorldPos = m_Board.CellToWorld(m_CellPosition);

        if (snapInstantly)
        {
            transform.position = m_TargetWorldPos;
            
        }
        else
        {
            
        }
    }



    private void Update()
    {
        if (Keyboard.current == null) return;

        moveInput = Vector2.zero;

        if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed)
            moveInput.y += 1f;  
        if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed)
            moveInput.y -= 1f;  
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            moveInput.x += 1f;  

        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            moveInput.x -= 1f;  
        if (moveInput.sqrMagnitude > 0f){
            sr.sprite = moveInput.x > 0f ? rightSprite : moveInput.x < 0f ? leftSprite : moveInput.y > 0f ? upSprite : downSprite;  
            Vector3 scaleVector = new Vector3(scale, scale, 1f);
            sr.transform.localScale = scaleVector;
        }
        if(Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame){
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            if (dashCounter <= 0f && dashCoolCounter <= 0f)
            {
                m_IsDashing = true;
                activeMoveSpeed = dashSpeed;

                dashCoolCounter = dashLength;   
                dashCounter = dashCooldown; 

                _trailRenderer.emitting = true;

                Vector2 dir2 = moveInput.sqrMagnitude > 0f
                    ? moveInput.normalized
                    : new Vector2(transform.right.x, transform.right.y);

                m_DashDirection = new Vector3(dir2.x, dir2.y, 0f);
            }
        }

        if (dashCoolCounter > 0f)
        {
            dashCoolCounter -= Time.deltaTime;
            if (dashCoolCounter <= 0f)
            {
                dashCoolCounter = 0f;
                m_IsDashing = false;
                _trailRenderer.emitting = false;
            }
        }

        if (dashCounter > 0f)
        {
            dashCounter -= Time.deltaTime;
            if (dashCounter < 0f) dashCounter = 0f;
        }

        UpdateDamages();
        AnimateBars();
    }



    private void FixedUpdate()
    {
        if (rb == null) return;

        Vector2 velocity = Vector2.zero;

        if (!m_IsDashing)
        {
            Vector2 input = moveInput;

            if (input.sqrMagnitude > 1f)
                input = input.normalized;

            activeMoveSpeed = Mathf.Lerp(activeMoveSpeed, moveSpeed, Time.deltaTime * dashRecoverSpeed);
            velocity = input * activeMoveSpeed;
        }
        else
        {
            velocity = (Vector2)m_DashDirection * dashSpeed;
        }

        rb.velocity = velocity;
    }
    private void PauseGame()
    {
        VisualElement PauseMenu = uiDocument.rootVisualElement.Q<VisualElement>("PauseMenu");
        if (PauseMenu != null)
        {
            PauseMenu.style.display = DisplayStyle.Flex;
            Time.timeScale = 0f;
            isPaused = true;
        }
        else
        {
            Debug.LogError("Could not find PauseMenu in UI Document!");
        }
    }
    private void ResumeGame()
    {
        VisualElement PauseMenu = uiDocument.rootVisualElement.Q<VisualElement>("PauseMenu");
        if (PauseMenu != null)
        {
            PauseMenu.style.display = DisplayStyle.None;
            Time.timeScale = 1f;
            isPaused = false;
        }
        else
        {
            Debug.LogError("Could not find PauseMenu in UI Document!");
        }
    }
    public void EndScreen()
    {
        VisualElement root = uiDocument.rootVisualElement;
        VisualElement EndScreen = root.Q<VisualElement>("EndScreen");
        EndScreen.style.display = DisplayStyle.Flex;
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
        CD = root.Q<ProgressBar>("DashCooldown");
        VisualElement PauseMenu = root.Q<VisualElement>("PauseMenu");
        PauseMenu.style.display = DisplayStyle.None;
        PauseMenu.Q<Button>("Resume").clicked += () => ResumeGame();
        PauseMenu.Q<Button>("MainMenu").clicked += () => SceneManager.LoadScene("MainMenu");
        PauseMenu.Q<Button>("QuitGame").clicked += () => Application.Quit();
        VisualElement EndScreen = root.Q<VisualElement>("EndScreen");
        EndScreen.style.display = DisplayStyle.None;
        EndScreen.Q<Button>("Restart").clicked += () => SceneManager.LoadScene(SceneManager.GetActiveScene().name);;
        if (healthBar == null)
            Debug.LogError("Could not find ProgressBar named HealthBar");

        if (xpBar == null)
            Debug.LogError("Could not find ProgressBar named XPBar");

        if (CD == null)
            Debug.LogError("Could not find ProgressBar named XPBar");

        displayedHealth = currentHealth;
        displayedXP = currentXP;
        displayedCD = dashCooldown - dashCounter;
        

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
        if (CD != null)
        {
            CD.lowValue = 0;
            CD.highValue = dashCooldown;
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
        if (CD != null)
        {
            CD.value = displayedCD;
            CD.title = currentXP + " / " + maxXP;
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
        if (CD != null)
        {
            displayedCD = Mathf.Lerp(
                displayedCD,
                dashCoolCounter,
                Time.deltaTime * barAnimationSpeed
            );

            CD.value = dashCooldown - dashCounter;
            CD.title = currentXP + " / " + maxXP;
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

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateBarLimits();
        damageFlash.TriggerFlash();
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
        totalLevels++;
        if (totalLevels % levelsForWeapon == 0)
        {
            upgrades.isWeaponMode = true;
        }
        else
        {
            upgrades.isWeaponMode = false;
        }

        upgrades.ShowChoices(this);
        UpdateBarLimits();
    }

    private void Die()
    {
        currentXP = 0;
        totalLevels = 0;
        weaponUpgradeLevels.Clear();
        weaponCount = 0;
        if (upgrades.weaponHolder != null)
        {
            foreach (Transform child in upgrades.weaponHolder)
            {
                Destroy(child.gameObject);
            }
        }
        if (startingWeapon != null)
        {
            currentWeapon = startingWeapon;
            weaponUpgradeLevels[startingWeapon] = 0;
            weaponCount = 1;
            startingWeapon.Apply(this, upgrades.weaponHolder, upgrades.playerCamera, 0);
        }
        gameUI.ShowDeathScreen();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
        XPOrb xpOrb = collision.gameObject.GetComponent<XPOrb>();
        BossBall bad = collision.gameObject.GetComponent<BossBall>();
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
        if(bad != null)
        {
            
            TakeDamage(bad.damage);
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