using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GuitarHero : MonoBehaviour
{
    [Header("Configuração de HUD")]
    public RectTransform containerSequencia;
    public GameObject prefabIconeGolpe;
    public Transform ponteiroCentral;
    public Transform playerTransform;
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

    [Header("Animações do Haruki")]
    public Animator animatorHaruki;
    public string parametroAura = "Aura";
    public string parametroIdle = "Idle";
    public string parametroOiZuki = "OiZuki";
    public string parametroMaeGeri = "MaeGeri";
    public string parametroMawashiGeri = "MawashiGeri";
    public string parametroGedanBarai = "GedanBarai";
    public string parametroJodanUke = "JodanUke";
    public string triggerGolpeEspecial = "GolpeEspecial";

    [Header("Diálogo")]
    public DialogoSimples dialogoSimples;
    [TextArea] public string falaInicial = "Prepare-se! Mostre o seu domínio da técnica.";
    [TextArea] public string falaFinal = "A técnica é o corpo. O espírito... é você quem molda.";
    public Sprite fotoInstrutor;
    public string nomeInstrutor = "Instrutor";

    [Header("Diálogos Ambiente")]
    [TextArea] public string[] dialogosAmbiente;
    public float intervaloAmbiente = 5f;
    public float duracaoFalaAmbiente = 1.6f;
    public bool dialogoAmbienteAtivo = true;

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
    private bool bloqueado = false;
    private Coroutine coroutineAmbiente = null;
    private int indiceAmbiente = 0;
    private int indexProximoIcone = 0;

    public enum Golpe { OiZuki, MaeGeri, MawashiGeri, GedanBarai, JodanUke }

    void Start()
    {
        if (dialogoSimples != null)
        {
            dialogoSimples.MostrarDialogo(nomeInstrutor, fotoInstrutor, falaInicial);
            StartCoroutine(EsperarDialogoInicial());
        }
        else
        {
            minigameIniciado = true;
            IniciarRodada();
            StartAmbientDialogueIfNeeded();
        }
    }

    IEnumerator EsperarDialogoInicial()
    {
        yield return new WaitForSeconds(3f);
        if (dialogoSimples != null)
            dialogoSimples.FecharDialogo();

        minigameIniciado = true;
        IniciarRodada();
        StartAmbientDialogueIfNeeded();
    }

    void Update()
    {
        if (!minigameIniciado || !comboAtivo) return;

        MoverHUDVertical();

        // CONTINUA FUNCIONANDO NO PC COM TECLADO
        if (Input.GetKeyDown(teclaOiZuki)) VerificarAcerto(Golpe.OiZuki);
        if (Input.GetKeyDown(teclaMaeGeri)) VerificarAcerto(Golpe.MaeGeri);
        if (Input.GetKeyDown(teclaMawashiGeri)) VerificarAcerto(Golpe.MawashiGeri);
        if (Input.GetKeyDown(teclaGedanBarai)) VerificarAcerto(Golpe.GedanBarai);
        if (Input.GetKeyDown(teclaJodanUke)) VerificarAcerto(Golpe.JodanUke);
    }

    void IniciarRodada()
    {
        if (containerSequencia != null)
        {
            foreach (Transform child in containerSequencia)
                Destroy(child.gameObject);
        }

        sequencia = new Queue<Golpe>();
        icones = new List<GameObject>();

        List<Golpe> golpes = new List<Golpe>
        {
            Golpe.OiZuki, Golpe.MaeGeri, Golpe.MawashiGeri, Golpe.GedanBarai, Golpe.JodanUke
        };

        // embaralhar
        for (int i = 0; i < golpes.Count; i++)
        {
            var tmp = golpes[i];
            int r = Random.Range(i, golpes.Count);
            golpes[i] = golpes[r];
            golpes[r] = tmp;
        }

        for (int i = 0; i < 5; i++)
        {
            Golpe g = golpes[i];
            sequencia.Enqueue(g);

            GameObject icone = Instantiate(prefabIconeGolpe, containerSequencia);
            Image img = icone.GetComponent<Image>();
            img.sprite = SpriteDoGolpe(g);

            RectTransform rt = icone.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, -i * espacoEntreGolpes);

            icones.Add(icone);
        }

        indexProximoIcone = 0;
        velocidadeHUD = (rodadaAtual == 1 ? velocidadeRodada1 : velocidadeRodada2);
        comboAtivo = true;
    }

    void MoverHUDVertical()
    {
        foreach (GameObject icone in icones)
        {
            if (icone == null) continue;
            RectTransform rt = icone.GetComponent<RectTransform>();
            rt.anchoredPosition += Vector2.up * velocidadeHUD * Time.deltaTime;
        }
    }

    // =========================
    // MÉTODOS PARA BOTÕES MOBILE
    // =========================
    public void BotaoOiZuki()
    {
        if (!minigameIniciado || !comboAtivo) return;
        VerificarAcerto(Golpe.OiZuki);
    }

    public void BotaoMaeGeri()
    {
        if (!minigameIniciado || !comboAtivo) return;
        VerificarAcerto(Golpe.MaeGeri);
    }

    public void BotaoMawashiGeri()
    {
        if (!minigameIniciado || !comboAtivo) return;
        VerificarAcerto(Golpe.MawashiGeri);
    }

    public void BotaoGedanBarai()
    {
        if (!minigameIniciado || !comboAtivo) return;
        VerificarAcerto(Golpe.GedanBarai);
    }

    public void BotaoJodanUke()
    {
        if (!minigameIniciado || !comboAtivo) return;
        VerificarAcerto(Golpe.JodanUke);
    }
    // =========================

    void VerificarAcerto(Golpe golpeTentado)
    {
        if (bloqueado) return;
        if (sequencia.Count == 0 || indexProximoIcone >= icones.Count) return;

        GameObject iconeAtual = icones[indexProximoIcone];
        RectTransform rt = iconeAtual.GetComponent<RectTransform>();

        Vector2 icon = RectTransformUtility.WorldToScreenPoint(null, rt.position);
        Vector2 pointer = RectTransformUtility.WorldToScreenPoint(null, ponteiroCentral.position);

        float distancia = Mathf.Abs(icon.y - pointer.y);
        Golpe golpeCorreto = sequencia.Peek();

        // ACERTO
        if (golpeTentado == golpeCorreto && distancia < 60f)
        {
            StartCoroutine(Feedback(iconeAtual, Color.green));
            sequencia.Dequeue();
            indexProximoIcone++;
            StartCoroutine(BloqueioCurto());

            if (sequencia.Count == 0)
                StartCoroutine(FinalizarRodada());
        }
        else
        {
            StartCoroutine(Feedback(iconeAtual, Color.red));
            StartCoroutine(ReiniciarRodadaCompleta());
        }
    }

    IEnumerator ReiniciarRodadaCompleta()
    {
        bloqueado = true;

        yield return new WaitForSeconds(0.35f);

        IniciarRodada();

        bloqueado = false;
    }

    IEnumerator BloqueioCurto()
    {
        bloqueado = true;
        yield return new WaitForSeconds(0.15f);
        bloqueado = false;
    }

    IEnumerator Feedback(GameObject icone, Color cor)
    {
        Image img = icone.GetComponent<Image>();
        img.color = cor;

        yield return new WaitForSeconds(0.15f);
        icone.SetActive(false);
    }

    IEnumerator FinalizarRodada()
    {
        comboAtivo = false;

        if (rodadaAtual == 1)
        {
            rodadaAtual = 2;
            IniciarRodada();
            yield break;
        }

        StopAmbientDialogueIfNeeded();

        if (animatorHaruki != null)
        {
            animatorHaruki.SetBool(parametroAura, true);
            animatorHaruki.SetTrigger(triggerGolpeEspecial);
            yield return new WaitForSeconds(9f);
        }

        if (dialogoSimples != null)
        {
            dialogoSimples.MostrarDialogo(nomeInstrutor, fotoInstrutor, falaFinal);
            yield return new WaitForSeconds(4f);
        }

        yield return StartCoroutine(TransicaoFinal());
    }

    IEnumerator TransicaoFinal()
    {
        if (!fadeImage.gameObject.activeSelf)
            fadeImage.gameObject.SetActive(true);

        Color c = fadeImage.color;
        fadeImage.color = new Color(c.r, c.g, c.b, 0f);

        float t = 0f;
        float duracao = 1.4f;

        while (t < duracao)
        {
            t += Time.deltaTime;
            fadeImage.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0, 1, t / duracao));
            yield return null;
        }

        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(nomeCenaFinal);
    }

    void StartAmbientDialogueIfNeeded()
    {
        if (!dialogoAmbienteAtivo) return;
        if (dialogosAmbiente == null || dialogosAmbiente.Length == 0) return;
        if (dialogoSimples == null) return;

        coroutineAmbiente = StartCoroutine(AmbienteLoop());
    }

    void StopAmbientDialogueIfNeeded()
    {
        if (coroutineAmbiente != null)
            StopCoroutine(coroutineAmbiente);

        if (dialogoSimples.gameObject.activeSelf)
            dialogoSimples.FecharDialogo();
    }

    IEnumerator AmbienteLoop()
    {
        while (true)
        {
            if (comboAtivo)
            {
                dialogoSimples.MostrarDialogo(nomeInstrutor, fotoInstrutor, dialogosAmbiente[indiceAmbiente]);
                yield return new WaitForSeconds(duracaoFalaAmbiente);
                dialogoSimples.FecharDialogo();

                indiceAmbiente = (indiceAmbiente + 1) % dialogosAmbiente.Length;
            }

            yield return new WaitForSeconds(intervaloAmbiente);
        }
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
        }
        return null;
    }
}
