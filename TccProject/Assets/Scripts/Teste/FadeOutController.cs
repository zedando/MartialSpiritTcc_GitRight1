using UnityEngine;
using System.Collections;

public class FadeOutController : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;  
    public float fadeDuration = 1.2f;

    public IEnumerator FadeOut()
    {
        fadeCanvasGroup.blocksRaycasts = true;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }
}
