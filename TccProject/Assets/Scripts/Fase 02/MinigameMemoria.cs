using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MinigameMemoria : MonoBehaviour
{
    public static MinigameMemoria Instance;

    public Slider barraProgresso;
    public CanvasGroup pecasGroup;
    public CanvasGroup imagemCompletaGroup;

    public TextMeshProUGUI caixaDeTexto;

    public GameObject caixaDialogo;          // NOVO: caixa visual
    public CanvasGroup caixaDialogoGroup;    // NOVO: para fade opcional

    public GameObject imagemCompleta;
    public GameObject proximoBotao;
    public CanvasGroup telaPreta;

    private int pecasColocadas = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Desliga caixa de diálogo no início
        if (caixaDialogo != null)
            caixaDialogo.SetActive(false);

        if (caixaDialogoGroup != null)
            caixaDialogoGroup.alpha = 0f;
    }

    public void PecaCorreta()
    {
        pecasColocadas++;
        barraProgresso.value = pecasColocadas / 9f;

        MostrarFrase(pecasColocadas);

        if (pecasColocadas >= 9)
        {
            FinalizarMinigame();
        }
    }

    void MostrarFrase(int etapa)
    {
        string[] frases =
        {
            " Esse homem... parece com ele.",
            " Espera... esse é o Dojo da foto da vila.",
            " Mas por que esconder isso?",
            " Ele parece feliz. Isso não faz sentido.",
            " Ele viveu isso. Por que nega?",
            " Por que jogou isso fora?",
            " Isso é importante pra mim.",
            " Mesmo que ele não entenda...",
            " Eu preciso seguir meu caminho."
        };

        if (etapa <= frases.Length)
        {
            // Liga a caixa de diálogo
            if (caixaDialogo != null)
                caixaDialogo.SetActive(true);

            if (caixaDialogoGroup != null)
                StartCoroutine(FadeCanvasGroup(caixaDialogoGroup, 1f, 0.3f));

            StartCoroutine(DigitarTexto(frases[etapa - 1]));
        }
    }

    IEnumerator DigitarTexto(string texto)
    {
        caixaDeTexto.text = "";

        foreach (char letra in texto)
        {
            caixaDeTexto.text += letra;
            yield return new WaitForSeconds(0.03f);
        }
    }

    public void FinalizarMinigame()
    {
        proximoBotao.SetActive(true);
        StartCoroutine(FadeFinal());
    }

    IEnumerator FadeFinal()
    {
        float duracao = 1.5f;
        float tempo = 0f;

        imagemCompletaGroup.gameObject.SetActive(true);

        // Fade das peças + imagem final
        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            float t = tempo / duracao;

            pecasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            imagemCompletaGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        pecasGroup.alpha = 0f;
        imagemCompletaGroup.alpha = 1f;

        // Desliga caixa de diálogo no fim do minigame
        if (caixaDialogo != null)
            caixaDialogo.SetActive(false);

        // Espera com imagem montada
        yield return new WaitForSeconds(7f);

        // Fade preto
        float tempoFade = 0f;
        while (tempoFade < 1.5f)
        {
            tempoFade += Time.deltaTime;
            float t = tempoFade / 1.5f;
            telaPreta.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        SceneManager.LoadScene("MapFase3");
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float alvo, float duracao)
    {
        float tempo = 0f;
        float alphaInicial = cg.alpha;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            cg.alpha = Mathf.Lerp(alphaInicial, alvo, tempo / duracao);
            yield return null;
        }

        cg.alpha = alvo;
    }
}
