using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlash : MonoBehaviour
{
    public Image flashImage;
    public float flashDuration = 0.2f;
    public float maxAlpha = 0.5f;

    public void TriggerFlash()
    {
        StopAllCoroutines();
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        Color color = flashImage.color;
        color.a = maxAlpha;
        flashImage.color = color;

        yield return new WaitForSeconds(flashDuration);

        // Fade out
        while (flashImage.color.a > 0)
        {
            color = flashImage.color;
            color.a -= Time.deltaTime * 5f;
            flashImage.color = color;
            yield return null;
        }
    }
}