using UnityEngine;
using TMPro;
using System.Collections;

public class SceneIntroFade : MonoBehaviour
{
    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;      // CanvasGroup do painel preto que cobre a tela
    public float holdTime = 2f;              // Tempo que a frase fica totalmente visível
    public float fadeDuration = 2f;          // Tempo do fade out

    [Header("Texto")]
    public TextMeshProUGUI centerText;       // Texto no meio da tela
    [TextArea]
    public string introText = "Depois daquele momento, tudo que Haruki queria era voltar para casa.";

    [Header("Tutorial")]
    public Tutorial2 tutorialScript;         // Arrasta o Tutorial2 aqui no Inspector

    private IEnumerator Start()
    {
        // Garante que o painel começa visível e com a frase
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
            fadeCanvasGroup.blocksRaycasts = true;
        }

        if (centerText != null)
        {
            centerText.gameObject.SetActive(true);
            centerText.text = introText;
        }

        // Espera um tempinho com a tela cheia + frase
        yield return new WaitForSeconds(holdTime);

        // Faz o fade out
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / fadeDuration);
            if (fadeCanvasGroup != null)
                fadeCanvasGroup.alpha = 1f - normalized;

            yield return null;
        }

        // Garante que sumiu
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }

        if (centerText != null)
        {
            centerText.gameObject.SetActive(false);
        }

        // Agora sim: começa o tutorial
        if (tutorialScript != null)
        {
            tutorialScript.BeginTutorial();
        }
    }
}