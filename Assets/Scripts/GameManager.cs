using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    
    public static MapGeneration Instance { get; private set; }
    [Header("References")]
    public MapGeneration BoardManager;
    public PlayerController PlayerController;
    public EnemySpawner enemySpawner;
    public UpgradeUIController upgrades;
    [Header("UI")]
    public UIDocument gameUIDocument;
    public float fadeInDuration = 1.5f;
    private VisualElement fadeScreen;
    [Header("Spawn Settings")]
    public float offscreenMargin = 2f;
    public bool done = false;
    public int level = 1;
    private Camera mainCam;
    [Header("Timing")]
    public float spawnIntervalOrginal = 3f;
    private float spawnInterval = 3f;
    public float postBossSpawnInterval = 2f;
    public float gameTime = 0f;
    public float difftime = 0f;
    public float diffstep = 0f;
    [SerializeField] AudioSource BossMusic;
    void Awake()
    {
        mainCam = Camera.main;
    }
public void OnBossDefeated()
    {
        ClearEnemies();
        ClearXp();
        ClearProjectiles();
        done = true;
        spawnInterval = spawnIntervalOrginal;
        difftime = 0f;
        if(level == 2)
        {
            PlayerController.EndScreen();
        }
        fadeScreen = gameUIDocument.rootVisualElement.Q<VisualElement>("FadeScreen");

        if (fadeScreen != null)
        {
            fadeScreen.style.display = DisplayStyle.Flex;
            fadeScreen.style.opacity = 1f;
            StartCoroutine(FadeInFromBlack());
        }
        PlayerController.Spawn(BoardManager, new Vector2Int(80, 5));
        done = false;

        StartCoroutine(SpawnEnemiesLoop(level++));

    }
    void Start()
    {
        if (gameUIDocument != null)
        {
            fadeScreen = gameUIDocument.rootVisualElement.Q<VisualElement>("FadeScreen");

            if (fadeScreen != null)
            {
                fadeScreen.style.display = DisplayStyle.Flex;
                fadeScreen.style.opacity = 1f;
                StartCoroutine(FadeInFromBlack());
            }
        }
    spawnInterval = spawnIntervalOrginal;
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(32, 5));
        UpgradeUIController UpgradeController = FindObjectOfType<UpgradeUIController>();

        UpgradeController.ShowRandomWeapons(PlayerController,1);
        StartCoroutine(SpawnEnemiesLoop(1));
    }

    

    private IEnumerator SpawnEnemiesLoop(int level)
{

    while (!done)
    {
        yield return new WaitForSeconds(spawnInterval);

        if (PlayerController == null || PlayerController.transform == null)
            continue;

        

        if (spawnInterval < 0)
        {
            spawnInterval = 0;
        }

        Vector3 spawnPos = BoardManager != null ? BoardManager.GetOffscreenSpawnPosition(mainCam, offscreenMargin) : GetOffscreenSpawnPosition();

            if (spawnInterval <= .1f)

            {
                ClearEnemies();
                BossMusic.Play();
                if(level == 1)
                enemySpawner.SpawnBoss1(BoardManager, PlayerController.transform, spawnPos, PlayerController, level);
                if(level == 2)
                {
                    enemySpawner.SpawnBoss2(BoardManager, PlayerController.transform, spawnPos, PlayerController, level);
                }
                spawnInterval = postBossSpawnInterval;                
            }
if (spawnInterval <= 0.3f)
{
    int randomEnemy = Random.Range(0, 4);

    if (randomEnemy == 0)
    {
        enemySpawner.SpawnZomb(BoardManager, PlayerController.transform, spawnPos, PlayerController, level);
    }
    else if (randomEnemy == 1)
    {
        enemySpawner.SpawnCheetah(BoardManager, PlayerController.transform, spawnPos, PlayerController, level);
    }
    else if (randomEnemy == 2)
    {
        enemySpawner.SpawnBulk(BoardManager, PlayerController.transform, spawnPos, PlayerController, level);
    }
    else
    {
        enemySpawner.SpawnBabyBert(BoardManager, PlayerController.transform, spawnPos, PlayerController, level);
    }

}
else if (spawnInterval <= 0.4f)
{
    if (Random.value < 0.5f)
    {
        enemySpawner.SpawnZomb(BoardManager, PlayerController.transform, spawnPos, PlayerController, level);
    }
    else
    {
        enemySpawner.SpawnCheetah(BoardManager, PlayerController.transform, spawnPos, PlayerController, level);
    }
}
else
{
    enemySpawner.SpawnZomb(BoardManager, PlayerController.transform, spawnPos, PlayerController, level);
}

        
    }
    
}

    private Vector3 GetOffscreenSpawnPosition()
    {        Vector3 bottomLeft = mainCam.ViewportToWorldPoint(new Vector3(0f, 0f, mainCam.nearClipPlane));
        Vector3 topRight = mainCam.ViewportToWorldPoint(new Vector3(1f, 1f, mainCam.nearClipPlane));

        float minX = bottomLeft.x;
        float maxX = topRight.x;
        float minY = bottomLeft.y;
        float maxY = topRight.y;
        int side = Random.Range(0, 4); 

        float x = 0f;
        float y = 0f;

        switch (side)
        {
            case 0: // left
                x = minX - offscreenMargin;
                y = Random.Range(minY, maxY);
                break;
            case 1: // right
                x = maxX + offscreenMargin;
                y = Random.Range(minY, maxY);
                break;
            case 2: // bottom
                x = Random.Range(minX, maxX);
                y = minY - offscreenMargin;
                break;
            case 3: // top
                x = Random.Range(minX, maxX);
                y = maxY + offscreenMargin;
                break;
        }
        return new Vector3(x, y, 0f);
    }

    private void ClearEnemies()
    {
        EnemyScript[] enemies = FindObjectsOfType<EnemyScript>();
        foreach (EnemyScript enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
    }
    private void ClearXp()
    
    {
        XPOrb[] xps = FindObjectsOfType<XPOrb>();
        foreach (XPOrb xp in xps)
        {
            Destroy(xp.gameObject);
        }
        gameTime = 0f;
    }
    private void ClearProjectiles()
    {
        Baseball[] projectiles = FindObjectsOfType<Baseball>();
        foreach (Baseball proj in projectiles)
        {
            Destroy(proj.gameObject);
        }
        BossBall[] bossProjectiles = FindObjectsOfType<BossBall>();
        foreach (BossBall proj in bossProjectiles)        {
            Destroy(proj.gameObject);
        }
    }
    private IEnumerator FadeInFromBlack()
{
    float timer = 0f;

    while (timer < fadeInDuration)
    {
        timer += Time.deltaTime;
        float t = timer / fadeInDuration;

        fadeScreen.style.opacity = Mathf.Lerp(1f, 0f, t);

        yield return null;
    }

    fadeScreen.style.opacity = 0f;
    fadeScreen.style.display = DisplayStyle.None;
}
    void Update()
    {
        if (done) return;
        gameTime += Time.deltaTime;
        if(gameTime>= difftime)
        {
            difftime += diffstep;
            spawnInterval -= 0.1f;
        }
    }
}
