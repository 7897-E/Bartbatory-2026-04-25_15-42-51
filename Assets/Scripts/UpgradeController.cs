using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeUIController : MonoBehaviour
{
    [Header("UI Toolkit")]
    public UIDocument uiDocument;          // points to UpgradeUI.uxml
    public VisualTreeAsset cardTemplate;   // points to UpgradeCard.uxml


    [Header("Upgrades")]
    public Upgrades[] allUpgrades;
    public int choicesPerLevel = 3;

    [Header("References")]
    public PlayerController PlayerController;
    public BatScript BatScript;
    private VisualElement root;
    private VisualElement cardsContainer;

    private readonly List<Upgrades> currentChoices = new List<Upgrades>();
    private readonly List<VisualElement> spawnedCards = new List<VisualElement>();

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        root = uiDocument.rootVisualElement;
        cardsContainer = root.Q<VisualElement>("cards-container");

        if (cardsContainer == null)
        {
            Debug.LogError("cards-container not found in UpgradeUI.uxml");
        }

        Hide();
    }

    public void ShowRandomUpgrades()
    {
        if (allUpgrades == null || allUpgrades.Length == 0)
        {
            Debug.LogWarning("No upgrades assigned to UpgradeUIController");
            return;
        }

        ClearCards();
        currentChoices.Clear();
        int count = Mathf.Min(choicesPerLevel, allUpgrades.Length);
        var used = new HashSet<int>();
        

        for (int i = 0; i < count; i++)
        {
            int index;
            do
            {
                index = Random.Range(0, allUpgrades.Length);
            } while (used.Contains(index));

            used.Add(index);
            currentChoices.Add(allUpgrades[index]);
        }
        Show();
        // Instantiate cards from template
        Debug.Log($"currentChoices is null? {currentChoices == null}");

        if (currentChoices == null || currentChoices.Count == 0)
        {
            Debug.LogWarning("currentChoices is null or empty before foreach");
            return; // or handle however you want
        }

        foreach (var upgrade in currentChoices)
        {
            
            CreateCard(upgrade);
        }


        Time.timeScale = 0f; 
    }

    private void CreateCard(Upgrades data)
    {
        if (cardTemplate == null)
        {
            Debug.LogError("Card template (VisualTreeAsset) is not assigned.");
            return;
        }

        VisualElement card = cardTemplate.Instantiate();

        card.AddToClassList("upgrade-card-instance");

        Label titleLabel = card.Q<Label>("card-title");
        Label descLabel = card.Q<Label>("card-description");
        Button button = card.Q<Button>("card-button");

        if (titleLabel != null)
            titleLabel.text = data.upgradeName;

        if (descLabel != null)
            descLabel.text = data.description;

        if (button != null)
        {
            button.text = "Select";
            button.clicked += () => OnUpgradeSelected(data);
            
        }

        cardsContainer.Add(card);
        spawnedCards.Add(card);
    }

    private void OnUpgradeSelected(Upgrades data)
    {
        Debug.Log($"Selected upgrade: {data.upgradeName}");
        data.Apply(PlayerController, BatScript);
        Hide();
        Time.timeScale = 1f;
    }

    private void ClearCards()
    {
        foreach (var card in spawnedCards)
        {
            cardsContainer.Remove(card);
        }
        spawnedCards.Clear();
    }

    public void Show()
    {
        if (root != null)
            root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        if (root != null)
            root.style.display = DisplayStyle.None;
    }
}
