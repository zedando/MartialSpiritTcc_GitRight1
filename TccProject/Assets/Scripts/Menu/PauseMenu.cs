using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using FMODUnity;

public class PauseMenu: MonoBehaviour
{
    [Header("Referências")]
    public CanvasGroup pauseCanvasGroup;
    public string nomeCenaMenu = "MainMenu";
    public GameObject icone;
    public float fadeDuration = 0.3f;
    public GameObject settingsPanel;

    [Header("Som")]
    [EventRef] public string eventoAbrirPause = "event:/ambiente/192271__lebaston100__click";
    [EventRef] public string eventoFecharPause = "event:/ambiente/192271__lebaston100__click";

    public static bool jogoPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (jogoPausado)
            StartCoroutine(FadeOutAndResume());
        else
        {
            icone.SetActive(true);
            StartCoroutine(FadeInAndPause());
            icone.SetActive(false);
        }
    }

    IEnumerator FadeInAndPause()
    {
        if (!string.IsNullOrEmpty(eventoAbrirPause))
            RuntimeManager.PlayOneShot(eventoAbrirPause);

        pauseCanvasGroup.gameObject.SetActive(true);
        pauseCanvasGroup.alpha = 0;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            pauseCanvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        pauseCanvasGroup.alpha = 1;
        Time.timeScale = 0f;
        jogoPausado = true;
    }

    IEnumerator FadeOutAndResume()
    {
        if (!string.IsNullOrEmpty(eventoFecharPause))
            RuntimeManager.PlayOneShot(eventoFecharPause);

        Time.timeScale = 1f;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            pauseCanvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        pauseCanvasGroup.alpha = 0;
        pauseCanvasGroup.gameObject.SetActive(false);
        jogoPausado = false;
    }

    public void ResumeGame()
    {
        StartCoroutine(FadeOutAndResume());
        icone.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeCenaMenu);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}
