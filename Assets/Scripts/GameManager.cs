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
    public GameObject pauseMenu;
    private bool isPaused;
    public GameObject endScreenUI;
void Update()
    {

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
    
    }
public void PauseGame()
{
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
}
public void ResumeGame()
{
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
}
public void GoToMainMenu()
{
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
}
public void QuitGame()
    {
        Application.Quit();
    }
    public void StartGame()
    {
        
    }
private VisualElement fadeScreen;
    [Header("Spawn Settings")]
    public float spawnInterval = 3f;   
    public float offscreenMargin = 2f;
    public int Scaling = 10;
    public float postBossSpawnInterval = 2f;
    public int PostBossScaling = 5;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }
public void OnBossDefeated()
    {
        Debug.Log("Boss defeated!");

        endScreenUI.SetActive(true);

        Time.timeScale = 0f;
    }
    void Start()
{
    pauseMenu.SetActive(false);
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

    BoardManager.Init();
    PlayerController.Spawn(BoardManager, new Vector2Int(80, 5));
    StartCoroutine(SpawnEnemiesLoop());
}

    

    private IEnumerator SpawnEnemiesLoop()
{
    int count = 0;

    while (true)
    {
        yield return new WaitForSeconds(spawnInterval);

        if (PlayerController == null || PlayerController.transform == null)
            continue;

        if (count == Scaling && spawnInterval >= 0)
        {
            spawnInterval -= 0.1f;
            count = 0;
            Scaling += Scaling + (int)(Scaling * .5);
        }

        if (spawnInterval < 0)
        {
            spawnInterval = 0;
        }

        Vector3 spawnPos = BoardManager != null ? BoardManager.GetOffscreenSpawnPosition(mainCam, offscreenMargin) : GetOffscreenSpawnPosition();

            if (spawnInterval == .1f)

            {
                ClearEnemies();
                enemySpawner.SpawnBossBulk(BoardManager, PlayerController.transform, spawnPos, PlayerController);
                spawnInterval = postBossSpawnInterval;
                Scaling = PostBossScaling;
                count = 0;
                // Continue spawning instead of breaking
            }
        if (spawnInterval <= 0.3f)
{
    int randomEnemy = Random.Range(0, 3);

    if (randomEnemy == 0)
    {
        enemySpawner.SpawnZomb(BoardManager, PlayerController.transform, spawnPos, PlayerController);
    }
    else if (randomEnemy == 1)
    {
        enemySpawner.SpawnCheetah(BoardManager, PlayerController.transform, spawnPos, PlayerController);
    }
    else
    {
        enemySpawner.SpawnBulk(BoardManager, PlayerController.transform, spawnPos, PlayerController);
    }

}
else if (spawnInterval <= 0.4f)
{
    if (Random.value < 0.5f)
    {
        enemySpawner.SpawnZomb(BoardManager, PlayerController.transform, spawnPos, PlayerController);
    }
    else
    {
        enemySpawner.SpawnCheetah(BoardManager, PlayerController.transform, spawnPos, PlayerController);
    }
}
else
{
    enemySpawner.SpawnZomb(BoardManager, PlayerController.transform, spawnPos, PlayerController);
}

        count++;
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
}
