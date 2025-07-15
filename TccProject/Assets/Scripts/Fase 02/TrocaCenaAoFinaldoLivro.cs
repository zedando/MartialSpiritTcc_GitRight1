using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using echo17.EndlessBook;
using System.Collections;

public class TrocaCenaAoFinaldoLivro : MonoBehaviour
{
    public EndlessBook book;
    public TextMeshProUGUI textoFinal;
    public string cenaDestino = "Memorias";
    public float tempoEntreLetras = 0.08f;
    public float tempoAteTrocarCena = 11f;

    public GameObject fadeImage; // arraste o painel de fade aqui

    private bool jaAtivado = false;
    private string mensagemFinal = "Haruki:... Pai?Ele parecia… feliz. Orgulhoso. Por que apagar isso da nossa história?";

    void Update()
    {
        if (!jaAtivado && book != null && book.CurrentState == EndlessBook.StateEnum.OpenBack)
        {
            jaAtivado = true;
            StartCoroutine(ExecutarFinal());
        }
    }

    IEnumerator ExecutarFinal()
    {
        textoFinal.gameObject.SetActive(true);
        textoFinal.text = "";

        foreach (char c in mensagemFinal)
        {
            textoFinal.text += c;
            yield return new WaitForSeconds(tempoEntreLetras);
        }

        yield return new WaitForSeconds(tempoAteTrocarCena - 1.5f); // inicia o fade 1.5s antes

        yield return StartCoroutine(FazerFade());

        SceneManager.LoadScene(cenaDestino);
    }

    IEnumerator FazerFade()
    {
        if (fadeImage != null)
        {
            fadeImage.SetActive(true);
            CanvasGroup cg = fadeImage.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = fadeImage.AddComponent<CanvasGroup>();
            }

            float duracao = 1.5f;
            float tempo = 0f;

            while (tempo < duracao)
            {
                tempo += Time.deltaTime;
                cg.alpha = Mathf.Lerp(0f, 1f, tempo / duracao);
                yield return null;
            }

            cg.alpha = 1f;
        }
    }
}
