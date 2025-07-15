using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TrocaDeCenaProximidade : MonoBehaviour
{
    public GameObject canvasImagem;
    public Button botaoClique;
    public string nomeCena;
    public Image painelFade;
    public float duracaoFade = 1f;

    private bool podeInteragir = false;
    private bool estaFazendoFade = false;

    void Start()
    {
        if (canvasImagem != null)
            canvasImagem.SetActive(false);

        if (painelFade != null)
            painelFade.color = new Color(0, 0, 0, 0);

        if (botaoClique != null)
            botaoClique.onClick.AddListener(IniciarTransicao);
    }

    public void OnInteragir(InputAction.CallbackContext context)
    {
        if (context.performed && podeInteragir && !estaFazendoFade)
            IniciarTransicao();
    }

    void IniciarTransicao()
    {
        if (!estaFazendoFade)
            StartCoroutine(FazerFadeETrocarCena());
    }

    IEnumerator FazerFadeETrocarCena()
    {
        estaFazendoFade = true;

        float tempo = 0f;
        while (tempo < duracaoFade)
        {
            float alpha = Mathf.Lerp(0f, 1f, tempo / duracaoFade);
            painelFade.color = new Color(0, 0, 0, alpha);
            tempo += Time.deltaTime;
            yield return null;
        }

        painelFade.color = new Color(0, 0, 0, 1f);
        SceneManager.LoadScene(nomeCena);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = true;
            if (canvasImagem != null)
                canvasImagem.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = false;
            if (canvasImagem != null)
                canvasImagem.SetActive(false);
        }
    }
}
