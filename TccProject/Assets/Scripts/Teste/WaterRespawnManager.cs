using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WaterRespawnManager : MonoBehaviour
{
    [Header("Cena")]
    [Tooltip("Nome da cena que será carregada. Se deixar vazio, recarrega a cena atual.")]
    public string sceneName;

    [Header("Fade")]
    [Tooltip("CanvasGroup do painel preto de fade.")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    private bool isFading = false;

    // Chamado quando o player encostar na água
    public void OnPlayerHitWater()
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndReload());
        }
    }

    private IEnumerator FadeAndReload()
    {
        isFading = true;

        // Garantir que o painel está ativo
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.gameObject.SetActive(true);

            float t = 0f;
            // Fade para preto (alpha 0 -> 1)
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }

            fadeCanvasGroup.alpha = 1f;
        }

        // Se não tiver nome, recarrega a cena atual
        string targetScene = string.IsNullOrEmpty(sceneName)
            ? SceneManager.GetActiveScene().name
            : sceneName;

        SceneManager.LoadScene(targetScene);
    }
}
