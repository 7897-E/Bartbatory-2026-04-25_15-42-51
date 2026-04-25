using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static MapGeneration Instance { get; private set; }

    public MapGeneration BoardManager;
    public PlayerController PlayerController;
    public EnemySpawner enemySpawner;
    public UpgradeUIController upgrades;

    public float spawnInterval = 3f;   
    public float offscreenMargin = 2f;
    public int Scaling = 10;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Start()
    {
        
    }
    public void StartGame()
    {
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(80, 1));
        StartCoroutine(SpawnEnemiesLoop());
        upgrades.ShowRandomUpgrades();
    }

    private IEnumerator SpawnEnemiesLoop()
    {
        int count = 0;
        
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (PlayerController == null || PlayerController.transform == null)
                continue;
            if (count == Scaling && spawnInterval >=0) { spawnInterval -= 0.1f; count = 0; Scaling += Scaling + (int)(Scaling * .5);  }
            if(spawnInterval < 0) { spawnInterval = 0; }
            Vector3 spawnPos = GetOffscreenSpawnPosition();
            enemySpawner.SpawnEnemy(BoardManager, PlayerController.transform, spawnPos);
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
}
