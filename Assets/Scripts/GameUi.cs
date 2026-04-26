using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    private VisualElement root;
    private Label uDiedLabel;

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        uDiedLabel = root.Q<Label>("UDied");

        root.style.display = DisplayStyle.None;
        root.style.opacity = 0f;

        uDiedLabel.style.display = DisplayStyle.Flex;
        uDiedLabel.style.opacity = 1f;
    }

    public void ShowDeathScreen()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        

        root.style.display = DisplayStyle.Flex;
        root.style.opacity = 0f;

        uDiedLabel.style.display = DisplayStyle.Flex;
        uDiedLabel.style.opacity = 1f;

        yield return FadeRoot(0f, 1f, 1.5f);
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator FadeRoot(float start, float end, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            root.style.opacity = Mathf.Lerp(start, end, t);

            yield return null;
        }

        root.style.opacity = end;
    }
}