using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUI : MonoBehaviour
{
     public PlayerController player;

     public float animationSpeed = 8f;

    private ProgressBar healthBar;
    private ProgressBar xpBar;

    private float displayedHealth;
    private float displayedXP;

    private void Start()
    {
        UIDocument doc = GetComponent<UIDocument>();
        var root = doc.rootVisualElement;

        healthBar = root.Q<ProgressBar>("HealthBar");
        xpBar = root.Q<ProgressBar>("XPBar");

        if (player != null)
        {
            displayedHealth = player.currentHealth;
            displayedXP = player.currentXP;
        }
        
    }

    private void Update()
    {
        if (player == null) return;

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
}