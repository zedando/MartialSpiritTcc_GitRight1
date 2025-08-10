using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MiniMapaDesliga : MonoBehaviour
{
    public CanvasGroup minimapaFechado; // CanvasGroup do minimapa fechado
    public CanvasGroup minimapaAberto;  // CanvasGroup do minimapa aberto

    private bool estaAberto = false;
    private Coroutine fadeCoroutine;

    public float duracaoFade = 0.2f;

    public void OnToggleMiniMapa()
    {
        Debug.Log("OnToggleMiniMapa foi chamado!");
        estaAberto = !estaAberto;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        if (estaAberto)
            fadeCoroutine = StartCoroutine(Fade(minimapaFechado, minimapaAberto));
        else
            fadeCoroutine = StartCoroutine(Fade(minimapaAberto, minimapaFechado));
    }

    void Start()
    {
        minimapaFechado.alpha = 1;
        minimapaFechado.interactable = true;
        minimapaFechado.blocksRaycasts = true;

        minimapaAberto.alpha = 0;
        minimapaAberto.interactable = false;
        minimapaAberto.blocksRaycasts = false;

        minimapaAberto.gameObject.SetActive(false);
        minimapaFechado.gameObject.SetActive(true);
    }

    private IEnumerator Fade(CanvasGroup deSumir, CanvasGroup deAparecer)
    {
        float tempo = 0f;

        deAparecer.gameObject.SetActive(true);

        while (tempo < duracaoFade)
        {
            tempo += Time.unscaledDeltaTime;
            float proporcao = Mathf.Clamp01(tempo / duracaoFade);

            deSumir.alpha = 1 - proporcao;
            deAparecer.alpha = proporcao;

            yield return null;
        }

        deSumir.alpha = 0;
        deSumir.interactable = false;
        deSumir.blocksRaycasts = false;
        deSumir.gameObject.SetActive(false);

        deAparecer.alpha = 1;
        deAparecer.interactable = true;
        deAparecer.blocksRaycasts = true;
    }
}
