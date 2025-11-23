using UnityEngine;
using System.Collections;

public class FadeInController : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;  
    public float fadeDuration = 1.5f;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.alpha = 1f; // começa totalmente escuro

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }
}
