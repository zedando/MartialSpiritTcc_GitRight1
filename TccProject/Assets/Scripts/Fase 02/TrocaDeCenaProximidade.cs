using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using FMODUnity; //  Import do FMOD

public class TrocaDeCenaProximidade : MonoBehaviour
{
    [Header("UI")]
    public GameObject textoUI; // "Pressione E para interagir"
    public CanvasGroup telaFade; // Objeto com CanvasGroup para fade

    [Header("Cena")]
    public string nomeCenaDestino = "NomeDaCena";

    [Header("Input")]
    public InputActionAsset inputAsset;

    [Header("Som")]
    [EventRef]
    public string eventoTrocaCena; // Caminho do evento no FMOD (ex: "event:/UI/TrocaCena")

    private InputAction interagirAction;
    private bool jogadorPerto = false;
    private bool jaInteragiu = false;

    void Start()
    {
        if (textoUI != null)
            textoUI.SetActive(false);

        if (telaFade != null)
            telaFade.alpha = 0f;

        interagirAction = inputAsset.FindAction("interagir", true);
        interagirAction.Enable();
    }

    void Update()
    {
        if (!jogadorPerto || jaInteragiu) return;

        if (interagirAction.triggered)
        {
            IniciarTransicao();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
            textoUI?.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            textoUI?.SetActive(false);
        }
    }

    private void IniciarTransicao()
    {
        if (jaInteragiu) return;

        jaInteragiu = true;
        textoUI?.SetActive(false);

        //  Toca o som da troca de cena
        if (!string.IsNullOrEmpty(eventoTrocaCena))
        {
            RuntimeManager.PlayOneShot(eventoTrocaCena, transform.position);
        }

        StartCoroutine(FazerFadeETrocarCena());
    }

    IEnumerator FazerFadeETrocarCena()
    {
        float duracao = 1.5f;
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            telaFade.alpha = Mathf.Lerp(0f, 1f, tempo / duracao);
            yield return null;
        }

        telaFade.alpha = 1f;

        SceneManager.LoadScene(nomeCenaDestino);
    }

    // Caso queira chamar a transição por outro script
    public void Scane()
    {
        IniciarTransicao();
    }
}
