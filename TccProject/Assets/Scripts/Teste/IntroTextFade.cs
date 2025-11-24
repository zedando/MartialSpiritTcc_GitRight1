using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroTextFade : MonoBehaviour
{
    [Header("Referências de UI")]
    public Image fadePanel;                // Painel preto de tela cheia
    public TMP_Text fraseText;            // Texto das frases

    [Header("Configuração de Tempo")]
    public float textFadeDuration = 1f;    // tempo para aparecer/sumir cada frase
    public float textHoldDuration = 2f;    // tempo que a frase fica totalmente visível
    public float finalFadeDuration = 1.5f; // tempo do fade da tela preta no final

    [Header("Cena seguinte (opcional)")]
    [Tooltip("Deixe vazio se não quiser trocar de cena no final.")]
    public string nextSceneName = "";

    private string[] frases = new string[]
    {
        "Mesmo cansado, Haruki sentiu esperança.",
        "Ali, no silêncio da noite, algo mudou dentro dele.",
        "E ao amanhecer… o Dojo o aguardava."
    };

    void Start()
    {
        // Garante que o painel comece totalmente preto
        if (fadePanel != null)
        {
            var c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;
        }

        // Garante que o texto comece invisível
        if (fraseText != null)
        {
            var c = fraseText.color;
            c.a = 0f;
            fraseText.color = c;
            fraseText.text = "";
        }

        StartCoroutine(SequenciaIntro());
    }

    IEnumerator SequenciaIntro()
    {
        // Para cada frase: fade in → segura → fade out
        foreach (var frase in frases)
        {
            fraseText.text = frase;

            // Fade IN do texto
            yield return StartCoroutine(FadeText(0f, 1f, textFadeDuration));

            // Segura a frase visível
            yield return new WaitForSeconds(textHoldDuration);

            // Fade OUT do texto
            yield return StartCoroutine(FadeText(1f, 0f, textFadeDuration));
        }

        // Depois da última frase, faz fade OUT do painel preto (revelando a cena)
        if (fadePanel != null)
        {
            yield return StartCoroutine(FadePanel(1f, 0f, finalFadeDuration));
        }

        // Se quiser trocar de cena no final, preencha nextSceneName no Inspector
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float t = 0f;
        Color c = fraseText.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            c.a = Mathf.Lerp(startAlpha, endAlpha, lerp);
            fraseText.color = c;
            yield return null;
        }

        c.a = endAlpha;
        fraseText.color = c;
    }

    IEnumerator FadePanel(float startAlpha, float endAlpha, float duration)
    {
        float t = 0f;
        Color c = fadePanel.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            c.a = Mathf.Lerp(startAlpha, endAlpha, lerp);
            fadePanel.color = c;
            yield return null;
        }

        c.a = endAlpha;
        fadePanel.color = c;
    }
}
