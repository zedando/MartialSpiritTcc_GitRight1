using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TrocaDeCenaProximidade : MonoBehaviour
{
   [Header("Configurações")]
    public CanvasGroup botaoInteracao; // botão de interação (canvas group para controlar alpha)
    public string cenaDestino = "NomeDaCena"; // nome da cena que vai carregar
    public float duracaoFade = 1f;

    private bool jogadorPerto = false;
    private bool interagindo = false;

    void Start()
    {
        if (botaoInteracao != null)
        {
            botaoInteracao.alpha = 0f;
            botaoInteracao.interactable = false;
            botaoInteracao.blocksRaycasts = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
            MostrarBotao(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            MostrarBotao(false);
        }
    }

    void MostrarBotao(bool mostrar)
    {
        if (botaoInteracao == null) return;

        botaoInteracao.alpha = mostrar ? 1f : 0f;
        botaoInteracao.interactable = mostrar;
        botaoInteracao.blocksRaycasts = mostrar;
    }

    // Método chamado pelo PlayerInput via SendMessage
    public void Interagir()
    {
        if (jogadorPerto && !interagindo)
        {
            interagindo = true;
            StartCoroutine(FadeECarregarCena());
        }
    }

    IEnumerator FadeECarregarCena()
    {
        Image imgFade = CriarImagemFade();

        float tempo = 0f;
        Color cor = imgFade.color;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            cor.a = Mathf.Lerp(0, 1, tempo / duracaoFade);
            imgFade.color = cor;
            yield return null;
        }

        SceneManager.LoadScene(cenaDestino);
    }

    Image CriarImagemFade()
    {
        GameObject go = new GameObject("FadeTela");
        go.transform.SetParent(FindObjectOfType<Canvas>().transform, false);

        Image img = go.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0);

        RectTransform rt = img.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return img;
    }
}
