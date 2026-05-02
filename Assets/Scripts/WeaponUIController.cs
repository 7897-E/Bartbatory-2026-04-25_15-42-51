using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponUIController : MonoBehaviour
{
    [Header("UI Toolkit")]
    public UIDocument uiDocument;
    public VisualTreeAsset cardTemplate;

    [Header("Weapons")]
    public Weapons[] allWeapons;
    public int choicesPerLevel = 3;

    [Header("References")]
    public PlayerController PlayerController;
    public Transform weaponHolder;
    public Camera playerCamera;

    private VisualElement root;
    private VisualElement cardsContainer;

    private readonly List<Weapons> currentChoices = new();
    private readonly List<VisualElement> spawnedCards = new();
    private readonly Dictionary<Button, VisualElement> dropdownPanels = new();

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        root = uiDocument.rootVisualElement;
        cardsContainer = root.Q<VisualElement>("cards-container");

        if (cardsContainer == null)
            Debug.LogError("cards-container not found in WeaponUI.uxml");

        Hide();
    }

    public void ShowRandomWeapons()
    {
        if (allWeapons == null || allWeapons.Length == 0)
        {
            Debug.LogWarning("No weapons assigned.");
            return;
        }

        ClearCards();
        currentChoices.Clear();

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
            currentChoices.Add(allWeapons[index]);
        }

        Show();

        foreach (Weapons weapon in currentChoices)
        {
            CreateCard(weapon);
        }

        MatchCardSizes();

        Time.timeScale = 0f;
    }

    private void CreateCard(Weapons data)
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

    private void OnWeaponSelected(Weapons data)
    {
        Debug.Log($"Selected: {data.weaponName}");

        // Assuming we need to destroy current weapon and apply new one
        // You might need to adjust this based on how weapons are managed
        if (weaponHolder.childCount > 0)
        {
            Destroy(weaponHolder.GetChild(0).gameObject);
        }

        data.Apply(PlayerController, weaponHolder, playerCamera);

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