using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MinigameCombo : MonoBehaviour
{
    [Header("Configuração de HUD")]
    public Transform containerSequencia;
    public GameObject prefabIconeGolpe;
    public Transform ponteiroCentral;
    public float velocidadeRodada1 = 200f;
    public float velocidadeRodada2 = 400f;
    public float espacoEntreGolpes = 120f;

    [Header("Sprites dos Golpes")]
    public Sprite spriteOiZuki;
    public Sprite spriteMaeGeri;
    public Sprite spriteMawashiGeri;
    public Sprite spriteGedanBarai;
    public Sprite spriteJodanUke;

    [Header("Teclas dos Golpes")]
    public KeyCode teclaOiZuki = KeyCode.A;
    public KeyCode teclaMaeGeri = KeyCode.S;
    public KeyCode teclaMawashiGeri = KeyCode.D;
    public KeyCode teclaGedanBarai = KeyCode.F;
    public KeyCode teclaJodanUke = KeyCode.G;

    [Header("Animações")]
    public Animator animatorHaruki;
    public Animator animatorSensei;
    public string parametroAura = "Aura";
    public string parametroReverencia = "Reverencia";
    public string parametroIdle = "Idle";
    public string parametroOiZuki = "OiZuki";
    public string parametroMaeGeri = "MaeGeri";
    public string parametroMawashiGeri = "MawashiGeri";
    public string parametroGedanBarai = "GedanBarai";
    public string parametroJodanUke = "JodanUke";
    public string parametroSenseiAtaque = "Ataque";

    [Header("Diálogo")]
    public DialogoSimples dialogoSimples;
    public Sprite spriteSensei;
    [TextArea] public string falaInicial = "Agora, mostre-me tudo o que aprendeu. Com um só sopro, mostre cinco caminhos.";
    [TextArea] public string falaFinal = "A técnica é o corpo. O espírito... é você quem molda.";

    [Header("Cena Final")]
    public string nomeCenaFinal;

    [Header("Fade")]
    public Image fadeImage;

    private Queue<Golpe> sequencia;
    private List<GameObject> icones;
    private bool comboAtivo = false;
    private float velocidadeHUD;
    private int rodadaAtual = 1;
    private bool minigameIniciado = false;
    private int totalRodadas = 2;

    public enum Golpe { OiZuki, MaeGeri, MawashiGeri, GedanBarai, JodanUke }

    void Start()
    {
        if (dialogoSimples != null)
        {
            dialogoSimples.MostrarDialogo("Sensei", spriteSensei, falaInicial);
            StartCoroutine(EsperarDialogoInicial());
        }
        else
        {
            minigameIniciado = true;
            IniciarRodada();
        }
    }

    IEnumerator EsperarDialogoInicial()
    {
        yield return new WaitForSeconds(4f);
        dialogoSimples.FecharDialogo();
        minigameIniciado = true;
        IniciarRodada();
    }

    void Update()
    {
        if (!minigameIniciado || !comboAtivo) return;

        MoverHUD();

        if (Input.GetKeyDown(teclaOiZuki)) VerificarAcerto(Golpe.OiZuki);
        if (Input.GetKeyDown(teclaMaeGeri)) VerificarAcerto(Golpe.MaeGeri);
        if (Input.GetKeyDown(teclaMawashiGeri)) VerificarAcerto(Golpe.MawashiGeri);
        if (Input.GetKeyDown(teclaGedanBarai)) VerificarAcerto(Golpe.GedanBarai);
        if (Input.GetKeyDown(teclaJodanUke)) VerificarAcerto(Golpe.JodanUke);
    }

    public void OnClickOiZuki() => VerificarAcerto(Golpe.OiZuki);
    public void OnClickMaeGeri() => VerificarAcerto(Golpe.MaeGeri);
    public void OnClickMawashiGeri() => VerificarAcerto(Golpe.MawashiGeri);
    public void OnClickGedanBarai() => VerificarAcerto(Golpe.GedanBarai);
    public void OnClickJodanUke() => VerificarAcerto(Golpe.JodanUke);

    void IniciarRodada()
    {
        foreach (Transform child in containerSequencia)
            Destroy(child.gameObject);

        sequencia = new Queue<Golpe>();
        icones = new List<GameObject>();

        List<Golpe> todosGolpes = new List<Golpe> 
        { 
            Golpe.OiZuki, Golpe.MaeGeri, Golpe.MawashiGeri, 
            Golpe.GedanBarai, Golpe.JodanUke 
        };

        for (int i = 0; i < todosGolpes.Count; i++)
        {
            Golpe temp = todosGolpes[i];
            int randomIndex = Random.Range(i, todosGolpes.Count);
            todosGolpes[i] = todosGolpes[randomIndex];
            todosGolpes[randomIndex] = temp;
        }

        for (int i = 0; i < 5; i++)
        {
            Golpe g = todosGolpes[i % todosGolpes.Count];
            sequencia.Enqueue(g);

            GameObject icone = Instantiate(prefabIconeGolpe, containerSequencia);
            icone.GetComponent<Image>().sprite = SpriteDoGolpe(g);
            RectTransform rt = icone.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(i * espacoEntreGolpes, 0);
            icones.Add(icone);
        }

        velocidadeHUD = (rodadaAtual == 1) ? velocidadeRodada1 : velocidadeRodada2;
        comboAtivo = true;
    }

    void MoverHUD()
    {
        if (icones == null) return;

        for (int i = 0; i < icones.Count; i++)
        {
            if (icones[i] == null) continue;
            RectTransform rt = icones[i].GetComponent<RectTransform>();
            rt.anchoredPosition += Vector2.left * velocidadeHUD * Time.deltaTime;
        }
    }

    void VerificarAcerto(Golpe golpeTentado)
    {
        if (sequencia == null || sequencia.Count == 0 || icones.Count == 0) return;

        Golpe golpeCorreto = sequencia.Peek();
        GameObject iconeAtual = icones[0];
        if (iconeAtual == null) return;

        RectTransform rt = iconeAtual.GetComponent<RectTransform>();
        float distancia = Mathf.Abs(rt.position.x - ponteiroCentral.position.x);

        if (golpeTentado == golpeCorreto && distancia < 60f)
        {
            StartCoroutine(FeedbackCor(iconeAtual, Color.green));
            AtivarAnimacaoGolpe(golpeTentado);

            if (golpeTentado == Golpe.GedanBarai || golpeTentado == Golpe.JodanUke)
                AtacarSensei();

            sequencia.Dequeue();
            icones.RemoveAt(0);

            if (sequencia.Count == 0)
                StartCoroutine(FinalizarRodada());
        }
        else
        {
            StartCoroutine(FeedbackCor(iconeAtual, Color.red));
            rodadaAtual = 1;
            IniciarRodada();
        }
    }

    void AtacarSensei()
    {
        if (animatorSensei == null) return;
        StartCoroutine(ExecutarAnimacaoSensei());
    }

    IEnumerator ExecutarAnimacaoSensei()
    {
        animatorSensei.SetBool(parametroSenseiAtaque, true);
        yield return new WaitForSeconds(0.6f);
        animatorSensei.SetBool(parametroSenseiAtaque, false);
    }

    IEnumerator FeedbackCor(GameObject icone, Color cor)
    {
        if (icone != null)
        {
            Image img = icone.GetComponent<Image>();
            img.color = cor;
            yield return new WaitForSeconds(0.2f);
            if (icone != null) Destroy(icone);
        }
    }

    IEnumerator FinalizarRodada()
    {
        comboAtivo = false;

        if (animatorSensei != null) animatorSensei.SetBool(parametroReverencia, true);

        yield return new WaitForSeconds(2f);

        if (animatorSensei != null) animatorSensei.SetBool(parametroReverencia, false);

        if (rodadaAtual == 1)
        {
            rodadaAtual = 2;
            IniciarRodada();
        }
        else
        {
            if (animatorHaruki != null) animatorHaruki.SetBool(parametroAura, true);

            if (dialogoSimples != null)
            {
                dialogoSimples.MostrarDialogo("Sensei", spriteSensei, falaFinal);
            }

            StartCoroutine(TransicaoFinal());
        }
    }

    void AtivarAnimacaoGolpe(Golpe g)
    {
        if (animatorHaruki == null) return;

        string parametro = "";
        switch (g)
        {
            case Golpe.OiZuki: parametro = parametroOiZuki; break;
            case Golpe.MaeGeri: parametro = parametroMaeGeri; break;
            case Golpe.MawashiGeri: parametro = parametroMawashiGeri; break;
            case Golpe.GedanBarai: parametro = parametroGedanBarai; break;
            case Golpe.JodanUke: parametro = parametroJodanUke; break;
        }

        StartCoroutine(ExecutarAnimacaoGolpe(parametro));
    }

    IEnumerator ExecutarAnimacaoGolpe(string parametro)
    {
        animatorHaruki.SetBool(parametro, true);
        yield return new WaitForSeconds(0.6f);
        animatorHaruki.SetBool(parametro, false);
        animatorHaruki.SetBool(parametroIdle, true);
        yield return new WaitForSeconds(0.1f);
        animatorHaruki.SetBool(parametroIdle, false);
    }

    Sprite SpriteDoGolpe(Golpe g)
    {
        switch (g)
        {
            case Golpe.OiZuki: return spriteOiZuki;
            case Golpe.MaeGeri: return spriteMaeGeri;
            case Golpe.MawashiGeri: return spriteMawashiGeri;
            case Golpe.GedanBarai: return spriteGedanBarai;
            case Golpe.JodanUke: return spriteJodanUke;
            default: return null;
        }
    }

    // ----------------------------- FADE OUT + TROCA DE CENA -----------------------------
    // ----------------------------- FADE OUT + TROCA DE CENA (ROBUSTO) -----------------------------
IEnumerator TransicaoFinal()
{
    // DEBUG
    Debug.Log("[MinigameCombo] Iniciando TransicaoFinal()");

    // 1) Espera o diálogo sumir OU timeout (10s) OU jogador apertar qualquer tecla para pular
    if (dialogoSimples != null)
    {
        float timeout = 10f;
        float timer = 0f;

        // Se o diálogo for desativado ao fechar, esse WaitUntil resolve.
        while (dialogoSimples.gameObject.activeSelf && timer < timeout)
        {
            if (Input.anyKeyDown) // permite pular se o jogador apertar algo
            {
                Debug.Log("[MinigameCombo] Pulei espera do diálogo por input do jogador.");
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (dialogoSimples.gameObject.activeSelf)
            Debug.LogWarning("[MinigameCombo] TransicaoFinal: diálogo ainda ativo após timeout. Continuando de qualquer forma.");
    }
    else
    {
        // Se não tem DialogoSimples, espera um pequeno delay pra ficar suave
        yield return new WaitForSeconds(0.25f);
    }

    // 2) Verifica fadeImage
    if (fadeImage == null)
    {
        Debug.LogWarning("[MinigameCombo] fadeImage não atribuído. Carregando cena sem fade.");
        // garante que a cena esteja na build settings
        if (string.IsNullOrEmpty(nomeCenaFinal))
        {
            Debug.LogError("[MinigameCombo] nomeCenaFinal vazio! Não há o que carregar.");
            yield break;
        }
        SceneManager.LoadScene(nomeCenaFinal);
        yield break;
    }

    // Garante que a imagem esteja ativa e inicial com alpha 0
    if (!fadeImage.gameObject.activeSelf) fadeImage.gameObject.SetActive(true);
    Color c = fadeImage.color;
    c.a = Mathf.Clamp01(c.a); // ok
    fadeImage.color = new Color(c.r, c.g, c.b, 0f);

    // 3) Faz o fade out
    yield return StartCoroutine(FadeOut());

    // 4) Carrega cena assincronamente
    if (string.IsNullOrEmpty(nomeCenaFinal))
    {
        Debug.LogError("[MinigameCombo] nomeCenaFinal vazio! Não há o que carregar.");
        yield break;
    }

    Debug.Log("[MinigameCombo] Carregando cena: " + nomeCenaFinal);
    AsyncOperation ao = SceneManager.LoadSceneAsync(nomeCenaFinal);
    ao.allowSceneActivation = true;
    // opcional: esperar até o carregamento terminar (normalmente LoadSceneAsync com allowSceneActivation true retorna automaticamente)
    while (!ao.isDone)
    {
        yield return null;
    }
}

// Fade out simples porém com garantia de configuração
IEnumerator FadeOut()
{
    float duracao = 1.2f;
    float t = 0f;

    if (fadeImage == null)
    {
        Debug.LogWarning("[MinigameCombo] FadeOut chamado sem fadeImage.");
        yield break;
    }

    // assegura imagem ativa
    if (!fadeImage.gameObject.activeSelf) fadeImage.gameObject.SetActive(true);

    Color c = fadeImage.color;
    float startA = c.a;
    float targetA = 1f;

    while (t < duracao)
    {
        t += Time.deltaTime;
        float a = Mathf.Lerp(startA, targetA, t / duracao);
        fadeImage.color = new Color(c.r, c.g, c.b, a);
        yield return null;
    }

    // garante alpha final = 1
    fadeImage.color = new Color(c.r, c.g, c.b, 1f);
}
}