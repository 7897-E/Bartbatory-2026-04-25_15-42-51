using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUI : MonoBehaviour
{
    public PlayerController player;

    public float animationSpeed = 8f;

    private ProgressBar healthBar;
    private ProgressBar xpBar;
    private Label timerLabel;
    
    private float displayedHealth;
    private float displayedXP;
    private float displayedTime;


    private GameManager gameManager;

    private void Start()
    {
        UIDocument doc = GetComponent<UIDocument>();
        var root = doc.rootVisualElement;

        healthBar = root.Q<ProgressBar>("HealthBar");
        xpBar = root.Q<ProgressBar>("XPBar");
        timerLabel = root.Q<Label>("TimerLabel");

        if (player != null)
        {
            displayedHealth = player.currentHealth;
            displayedXP = player.currentXP;
        }

  
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            displayedTime = gameManager.gameTime;
        }
    }

    private void Update()
    {
    
        if (player != null)
        {
            displayedHealth = Mathf.Lerp(displayedHealth, player.currentHealth, Time.deltaTime * animationSpeed);
            displayedXP = Mathf.Lerp(displayedXP, player.currentXP, Time.deltaTime * animationSpeed);

            if (healthBar != null)
            {
                healthBar.lowValue = 0;
                healthBar.highValue = player.maxHealth;
                healthBar.value = displayedHealth;
            }

            if (xpBar != null)
            {
                xpBar.lowValue = 0;
                xpBar.highValue = player.maxXP;
                xpBar.value = displayedXP;
            }
        }

   
        
        
            gameManager = FindObjectOfType<GameManager>();
            
        

   
        if (gameManager != null && timerLabel != null)
        {
            displayedTime = Mathf.Lerp(displayedTime, gameManager.gameTime, Time.deltaTime * animationSpeed);
            timerLabel.text = FormatTime((((gameManager.spawnIntervalOrginal * 10) * gameManager.diffstep)-displayedTime));
        }
    }

    private static string FormatTime(float timeSeconds)
    {
        int minutes = Mathf.FloorToInt(timeSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeSeconds % 60f);
        return string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }
}