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
    public GameObject imagemCompleta;
    public GameObject proximoBotao;
    public CanvasGroup telaPreta;


    private int pecasColocadas = 0;

    void Awake()
    {
        Instance = this;
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
        "Haruki: Esse homem... parece com ele.",
        "Haruki: Espera... esse é o Dojo da foto da vó...",
        "Haruki: Mas por que esconder isso?",
        "Haruki: Ele parece feliz. Isso não faz sentido.",
        "Haruki: Ele viveu isso. Por que nega?",
        "Haruki: Por que jogou isso fora?",
        "Haruki: Isso é importante pra mim.",
        "Haruki: Mesmo que ele não entenda...",
        "Haruki: Eu preciso seguir meu caminho."
    };

    if (etapa <= frases.Length)
        StartCoroutine(DigitarTexto(frases[etapa - 1]));
}
IEnumerator DigitarTexto(string texto)
{
    caixaDeTexto.text = "";
    foreach (char letra in texto)
    {
        caixaDeTexto.text += letra;
        yield return new WaitForSeconds(0.03f); // ajuste a velocidade como quiser
    }
}

   void FinalizarMinigame()
{ proximoBotao.SetActive(true); 

    StartCoroutine(FadeFinal());
}
IEnumerator FadeFinal()
{
    float duracao = 1.5f;
    float tempo = 0f;

    imagemCompletaGroup.gameObject.SetActive(true); // Garante que está ativa

    // Fade das peças e da imagem final
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

    // Espera 7 segundos com imagem montada
    yield return new WaitForSeconds(7f);

    // Fade para tela preta
    float tempoFade = 0f;
    while (tempoFade < 1.5f)
    {
        tempoFade += Time.deltaTime;
        float t = tempoFade / 1.5f;
        telaPreta.alpha = Mathf.Lerp(0f, 1f, t);
        yield return null;
    }

    // Troca de cena
    SceneManager.LoadScene("Map1"); // ou o nome da próxima cena
}
}
