using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeUIController : MonoBehaviour
{
    [Header("UI Toolkit")]
    public UIDocument uiDocument;
    public VisualTreeAsset cardTemplate;

    [Header("Upgrades")]
    public Upgrades[] allUpgrades;
    public int choicesPerLevel = 3;

    [Header("Weapons")]
    public Weapons[] allWeapons;
    public bool isWeaponMode = false;
    public string upgradeTitle = "Select Upgrade";
    public string weaponTitle = "Select New Weapon";

    [Header("References")]
    public PlayerController PlayerController;
    public BatScript BatScript;
    public Transform weaponHolder;
    public Camera playerCamera;

    private VisualElement root;
    private VisualElement cardsContainer;
    private Label titleLabel;

    private readonly List<Upgrades> currentChoices = new();
    private readonly List<Weapons> currentWeaponChoices = new();
    private readonly List<VisualElement> spawnedCards = new();
    private readonly Dictionary<Button, VisualElement> dropdownPanels = new();

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        root = uiDocument.rootVisualElement;
        cardsContainer = root.Q<VisualElement>("cards-container");
        titleLabel = root.Q<Label>("title-label");

        if (cardsContainer == null)
            Debug.LogError("cards-container not found in UpgradeUI.uxml");

        if (titleLabel != null)
        {
            titleLabel.text = isWeaponMode ? weaponTitle : upgradeTitle;
        }

        Hide();
    }

    public void ShowChoices(PlayerController player)
    {
        if (isWeaponMode)
        {
            ShowRandomWeapons(player);
        }
        else
        {
            ShowRandomUpgrades(player);
        }
    }

    public void ShowRandomUpgrades(PlayerController player)
    {
        PlayerController = player;
        if (allUpgrades == null || allUpgrades.Length == 0)
        {
            Debug.LogWarning("No upgrades assigned.");
            return;
        }

        ClearCards();
        currentChoices.Clear();

        int count = Mathf.Min(choicesPerLevel, allUpgrades.Length);
        HashSet<int> usedIndexes = new();

        for (int i = 0; i < count; i++)
        {
            int index;

            do
            {
                index = Random.Range(0, allUpgrades.Length);
            }
            while (usedIndexes.Contains(index));

            usedIndexes.Add(index);
            currentChoices.Add(allUpgrades[index]);
        }

        Show();

        foreach (Upgrades upgrade in currentChoices)
        {
            CreateUpgradeCard(upgrade);
        }

        MatchCardSizes();

        Time.timeScale = 0f;
    }

    public void ShowRandomWeapons(PlayerController player)
    {
        PlayerController = player;
        if (allWeapons == null || allWeapons.Length == 0)
        {
            Debug.LogWarning("No weapons assigned.");
            return;
        }

        ClearCards();
        currentWeaponChoices.Clear();

        int count = Mathf.Min(choicesPerLevel, allWeapons.Length);
        HashSet<int> usedIndexes = new();

        for (int i = 0; i < count; i++)
        {
            int index;

            do
            {
                index = Random.Range(0, allWeapons.Length);
            }
            while (usedIndexes.Contains(index));

            usedIndexes.Add(index);
            currentWeaponChoices.Add(allWeapons[index]);
        }

        Show();

        foreach (Weapons weapon in currentWeaponChoices)
        {
            CreateWeaponCard(weapon);
        }

        MatchCardSizes();

        Time.timeScale = 0f;
    }

    private void CreateUpgradeCard(Upgrades data)
    {
        if (cardTemplate == null)
        {
            Debug.LogError("Card template not assigned.");
            return;
        }

        VisualElement card = cardTemplate.Instantiate();
        card.AddToClassList("upgrade-card-container");

        Button button = card.Q<Button>("card-button");
        VisualElement descriptionPanel = card.Q<VisualElement>("description-panel");
        Label descriptionText = card.Q<Label>("description-text");

        if (button == null)
        {
            Debug.LogError("card-button not found in card template.");
            return;
        }

        if (descriptionPanel == null)
        {
            Debug.LogError("description-panel not found in card template.");
            return;
        }

        if (descriptionText == null)
        {
            Debug.LogError("description-text not found in card template.");
            return;
        }

        button.text = data.upgradeName;
        descriptionText.text = data.description;

        descriptionPanel.style.display = DisplayStyle.None;

        dropdownPanels[button] = descriptionPanel;

        button.clicked += () => OnUpgradeSelected(data);

        button.RegisterCallback<MouseEnterEvent>(_ => ShowDropdown(button));
        button.RegisterCallback<MouseLeaveEvent>(_ => HideDropdown(button));

        descriptionPanel.RegisterCallback<MouseEnterEvent>(_ => ShowDropdown(button));
        descriptionPanel.RegisterCallback<MouseLeaveEvent>(_ => HideDropdown(button));

        cardsContainer.Add(card);
        spawnedCards.Add(card);
    }

    private void CreateWeaponCard(Weapons data)
    {
        if (cardTemplate == null)
        {
            Debug.LogError("Card template not assigned.");
            return;
        }

        VisualElement card = cardTemplate.Instantiate();
        card.AddToClassList("weapon-card-container");

        Button button = card.Q<Button>("card-button");
        VisualElement descriptionPanel = card.Q<VisualElement>("description-panel");
        Label descriptionText = card.Q<Label>("description-text");

        if (button == null)
        {
            Debug.LogError("card-button not found in card template.");
            return;
        }

        if (descriptionPanel == null)
        {
            Debug.LogError("description-panel not found in card template.");
            return;
        }

        if (descriptionText == null)
        {
            Debug.LogError("description-text not found in card template.");
            return;
        }

        button.text = data.weaponName;
        descriptionText.text = data.description;

        descriptionPanel.style.display = DisplayStyle.None;

        dropdownPanels[button] = descriptionPanel;

        button.clicked += () => OnWeaponSelected(data);

        button.RegisterCallback<MouseEnterEvent>(_ => ShowDropdown(button));
        button.RegisterCallback<MouseLeaveEvent>(_ => HideDropdown(button));

        descriptionPanel.RegisterCallback<MouseEnterEvent>(_ => ShowDropdown(button));
        descriptionPanel.RegisterCallback<MouseLeaveEvent>(_ => HideDropdown(button));

        cardsContainer.Add(card);
        spawnedCards.Add(card);
    }

    private void ShowDropdown(Button button)
    {
        if (dropdownPanels.TryGetValue(button, out VisualElement panel))
        {
            panel.style.display = DisplayStyle.Flex;
        }
    }

    private void HideDropdown(Button button)
    {
        if (dropdownPanels.TryGetValue(button, out VisualElement panel))
        {
            panel.style.display = DisplayStyle.None;
        }
    }

    private void MatchCardSizes()
    {
        if (spawnedCards.Count == 0)
            return;

        root.schedule.Execute(() =>
        {
            float maxWidth = 0f;
            float maxHeight = 0f;

            foreach (VisualElement card in spawnedCards)
            {
                maxWidth = Mathf.Max(maxWidth, card.layout.width);
                maxHeight = Mathf.Max(maxHeight, card.layout.height);
            }

            foreach (VisualElement card in spawnedCards)
            {
                card.style.width = maxWidth;
                card.style.minHeight = maxHeight;
            }
        }).StartingIn(0);
    }

    private void OnUpgradeSelected(Upgrades data)
    {
        Debug.Log($"Selected: {data.upgradeName}");

        if (PlayerController.currentWeapon != null && PlayerController.weaponUpgradeLevels.ContainsKey(PlayerController.currentWeapon))
        {
            int level = PlayerController.weaponUpgradeLevels[PlayerController.currentWeapon];
            data.Apply(PlayerController, BatScript, level + 1);
            PlayerController.weaponUpgradeLevels[PlayerController.currentWeapon]++;
        }
        else
        {
            data.Apply(PlayerController, BatScript);
        }

        Hide();

        Time.timeScale = 1f;
    }

    private void OnWeaponSelected(Weapons data)
    {
        Debug.Log($"Selected: {data.weaponName}");

        // Do not destroy current weapon, add new one
        PlayerController.weaponCount++;

        data.Apply(PlayerController, weaponHolder, playerCamera, PlayerController.weaponCount - 1);

        PlayerController.currentWeapon = data;
        if (!PlayerController.weaponUpgradeLevels.ContainsKey(data))
        {
            PlayerController.weaponUpgradeLevels[data] = 0;
        }

        Hide();

        Time.timeScale = 1f;
    }

    private void ClearCards()
    {
        foreach (VisualElement card in spawnedCards)
        {
            cardsContainer.Remove(card);
        }

        spawnedCards.Clear();
        dropdownPanels.Clear();
    }

    public void Show()
    {
        root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        root.style.display = DisplayStyle.None;
    }
}