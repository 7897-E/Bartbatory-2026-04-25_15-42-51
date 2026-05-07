using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class MainMenu : MonoBehaviour
{
    private VisualElement _menuRoot;
    private Button _startButton;

    public float fadeDuration = 1f;

    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;

        _menuRoot = root.Q<VisualElement>(className: "menu-root");
        _startButton = root.Q<Button>("NewRun");

        if (_menuRoot == null)
            Debug.LogWarning("MainMenuUI: menu-root element not found.");

        if (_startButton == null)
        {
            Debug.LogWarning("MainMenuUI: NewRun button not found.");
            return;
        }

        _menuRoot.style.opacity = 1f;
        _menuRoot.style.display = DisplayStyle.Flex;

        _startButton.clicked += OnStartClicked;
    }

    void OnDisable()
    {
        if (_startButton != null)
            _startButton.clicked -= OnStartClicked;
    }

    private void OnStartClicked()
    {
        if (_startButton != null)
            _startButton.SetEnabled(false);

        StartCoroutine(FadeOutAndLoad());
    }

    private IEnumerator FadeOutAndLoad()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            _menuRoot.style.opacity = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        _menuRoot.style.opacity = 0f;

        SceneManager.LoadScene("GamePlay");
    }
}