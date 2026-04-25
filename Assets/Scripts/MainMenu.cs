using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenu : MonoBehaviour
{
    public GameManager gameManager;  

    private VisualElement _menuRoot;

    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;

        _menuRoot = root.Q<VisualElement>(className: "menu-root");
        if (_menuRoot == null)
        {
            Debug.LogWarning("MainMenuUI: menu-root element not found.");
        }

        var startButton = root.Q<Button>("NewRun");

        if (startButton == null)
        {
            Debug.LogWarning("MainMenuUI: Start button not found.");
            return;
        }

        startButton.clicked += OnStartClicked;
    }

    void OnDisable()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null) return;

        var root = uiDoc.rootVisualElement;
        var startButton = root.Q<Button>("StartButton");
        if (startButton != null)
            startButton.clicked -= OnStartClicked;
    }

    private void OnStartClicked()
    {
 
        if (gameManager != null)
        {
            gameManager.StartGame();
        }


        if (_menuRoot != null)
        {
            _menuRoot.AddToClassList("fade-out");

            StartCoroutine(DisableUIAfterFade(0.5f));
        }
    }

    private System.Collections.IEnumerator DisableUIAfterFade(float delay)
    {
        yield return new WaitForSeconds(delay);

        gameObject.SetActive(false);


    }
}
