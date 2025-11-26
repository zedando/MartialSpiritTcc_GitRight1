using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashSequence : MonoBehaviour
{
    [Header("Imagens em ordem")]
    public Image[] imagens; // 0 = apps, 1 = logo equipe, 2 = logo jogo

    [Header("Tempos")]
    public float tempoFade = 1f;      // tempo para aparecer/desaparecer
    public float tempoNaTela = 1.5f;  // tempo que a imagem fica visível

    [Header("Cena do Menu")]
    public string nomeCenaMenu = "Menu"; // coloca aqui o nome exato da cena do menu

    void Start()
    {
        StartCoroutine(SequenciaSplash());
    }

    IEnumerator SequenciaSplash()
    {
        // Deixa todas as imagens invisíveis no início
        foreach (Image img in imagens)
        {
            if (img != null)
                SetAlpha(img, 0f);
        }

        // Passa por cada imagem da lista
        for (int i = 0; i < imagens.Length; i++)
        {
            Image img = imagens[i];
            if (img == null) continue;

            // Fade IN
            yield return StartCoroutine(FadeImage(img, 0f, 1f, tempoFade));

            // Fica um tempo na tela
            yield return new WaitForSeconds(tempoNaTela);

            // Fade OUT
            yield return StartCoroutine(FadeImage(img, 1f, 0f, tempoFade));
        }

        // Quando terminar todas, vai para o menu
        SceneManager.LoadScene(nomeCenaMenu);
    }

    void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    IEnumerator FadeImage(Image img, float alphaInicial, float alphaFinal, float duracao)
    {
        float t = 0f;

        SetAlpha(img, alphaInicial);

        while (t < duracao)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duracao);
            float alphaAtual = Mathf.Lerp(alphaInicial, alphaFinal, lerp);
            SetAlpha(img, alphaAtual);
            yield return null;
        }

        SetAlpha(img, alphaFinal);
    }
}
