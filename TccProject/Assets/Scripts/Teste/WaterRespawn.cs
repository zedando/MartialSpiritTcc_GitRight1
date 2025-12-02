using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WaterRespawn : MonoBehaviour
{
    [Header("Cena")]
    [Tooltip("Nome da cena que será carregada. Se deixar vazio, recarrega a cena atual.")]
    public string sceneName;

    [Header("Fade")]
    [Tooltip("CanvasGroup do painel preto de fade.")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    private bool isFading = false;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se encostou em algo com tag Water
        if (isFading) return;

        if (other.CompareTag("Water"))
        {
            Debug.Log("Player encostou na Water, iniciando fade...");
            StartCoroutine(FadeAndReload());
        }
    }

    private IEnumerator FadeAndReload()
    {
        isFading = true;

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.gameObject.SetActive(true);

            float t = 0f;
            float startAlpha = fadeCanvasGroup.alpha;

            // Garante que começa transparente
            if (startAlpha < 0.01f)
                fadeCanvasGroup.alpha = 0f;

            // Fade para preto (alpha 0 -> 1)
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float normalized = Mathf.Clamp01(t / fadeDuration);
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, normalized);
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
