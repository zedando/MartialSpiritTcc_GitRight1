using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LivroBrilhando : MonoBehaviour
{
    [Header("Botão de Continuar")]
    public CanvasGroup botaoContinuarUI;
    public string nomeCenaDestino = "NomeDaCena";

    [Header("Brilho")]
    public Renderer livroRenderer;
    public Color corBrilho = Color.yellow;
    public float intervaloPiscada = 0.5f;

    [Header("Input & UI")]
    public InputActionAsset playerInputAsset;
    public GameObject textoUI;
    public Button botaoInteragir;

    [Header("UI do Livro")]
    public GameObject uiLivroCompleto;
    public CanvasGroup painelCompletoUI;
    public CanvasGroup imagemLivroUI;
    public CanvasGroup nomeLivroUI;
    public CanvasGroup caixaDescricaoUI;

    private InputAction interagirAction;
    private Material material;
    private bool jogadorPerto = false;
    private float tempo = 0f;
    private bool estaBrilhando = false;
    private bool jaInteragiu = false;

    void Start()
    {
        material = livroRenderer.material;
        material.EnableKeyword("_EMISSION");
        DesligarBrilho();

        if (textoUI != null)
            textoUI.SetActive(false);

        if (botaoInteragir != null)
            botaoInteragir.onClick.AddListener(InteragirViaClique);

        interagirAction = playerInputAsset.FindAction("interagir");
        interagirAction.Enable();

        if (uiLivroCompleto != null)
            uiLivroCompleto.SetActive(false);

        painelCompletoUI.alpha = 0f;
        imagemLivroUI.alpha = 0f;
        nomeLivroUI.alpha = 0f;
        caixaDescricaoUI.alpha = 0f;
        botaoContinuarUI.alpha = 0f;
    }

    void Update()
    {
        if (jaInteragiu) return;

        if (!jogadorPerto)
        {
            tempo += Time.deltaTime;
            if (tempo >= intervaloPiscada)
            {
                tempo = 0f;
                if (estaBrilhando) DesligarBrilho();
                else LigarBrilho();
                estaBrilhando = !estaBrilhando;
            }
        }

        if (jogadorPerto && interagirAction.triggered)
        {
            InteragirViaClique();
        }
    }

    private void LigarBrilho()
    {
        material.SetColor("_EmissionColor", corBrilho);
    }

    private void DesligarBrilho()
    {
        material.SetColor("_EmissionColor", Color.black);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
            textoUI?.SetActive(true);
            DesligarBrilho();
            estaBrilhando = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            textoUI?.SetActive(false);
            tempo = 0f;
        }
    }

    public void InteragirViaClique()
    {
        if (!jaInteragiu && jogadorPerto)
        {
            textoUI?.SetActive(false);
            StartCoroutine(MostrarLivroComFade());
            jaInteragiu = true;
        }
    }

    IEnumerator MostrarLivroComFade()
    {
        uiLivroCompleto.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(painelCompletoUI, 1f, 0.6f));
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(FadeCanvasGroup(imagemLivroUI, 1f, 0.8f));
        yield return new WaitForSeconds(0.4f);
        yield return StartCoroutine(FadeCanvasGroup(nomeLivroUI, 1f, 0.8f));
        yield return new WaitForSeconds(0.4f);
        yield return StartCoroutine(FadeCanvasGroup(caixaDescricaoUI, 1f, 0.8f));
        yield return new WaitForSeconds(0.9f);
        yield return StartCoroutine(FadeCanvasGroup(botaoContinuarUI, 1f, 0.9f));
    }

    public void TrocarCena()
    {
        SceneManager.LoadScene(nomeCenaDestino);
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
