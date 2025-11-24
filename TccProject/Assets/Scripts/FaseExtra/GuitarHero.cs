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
    public Transform playerTransform;   // <<< ícones surgem a partir daqui
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
    // nome do trigger para o golpe especial (coloque no Animator se quiser usar)
    public string triggerGolpeEspecial = "GolpeEspecial";

    [Header("Diálogo")]
    public DialogoSimples dialogoSimples;
    [TextArea] public string falaInicial = "Prepare-se! Mostre o seu domínio da técnica.";
    [TextArea] public string falaFinal = "A técnica é o corpo. O espírito... é você quem molda.";

    [Header("Diálogos Ambiente (sequenciais durante a fase)")]
    [TextArea] public string[] dialogosAmbiente;   // lista sequencial de falas
    public float intervaloAmbiente = 5f;           // intervalo fixo entre falas (modo C)
    public float duracaoFalaAmbiente = 1.6f;       // tempo que cada fala fica visível
    public bool dialogoAmbienteAtivo = true;       // pode desligar pelo Inspector

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

    // bloqueio curto após um acerto para evitar leitura precoce
    private bool bloqueado = false;

    // coroutine de diálogo ambiente
    private Coroutine coroutineAmbiente = null;
    private int indiceAmbiente = 0;

    public enum Golpe { OiZuki, MaeGeri, MawashiGeri, GedanBarai, JodanUke }

    void Start()
    {
        if (dialogoSimples != null)
        {
            dialogoSimples.MostrarDialogo("Instrutor", null, falaInicial);
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
        if (dialogoSimples != null) dialogoSimples.FecharDialogo();
        minigameIniciado = true;
        IniciarRodada();
        StartAmbientDialogueIfNeeded();
    }

    void Update()
    {
        if (!minigameIniciado || !comboAtivo) return;

        MoverHUDVertical();

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
        if (containerSequencia != null)
        {
            foreach (Transform child in containerSequencia)
                Destroy(child.gameObject);
        }

        sequencia = new Queue<Golpe>();
        icones = new List<GameObject>();

        List<Golpe> todosGolpes = new List<Golpe>
        {
            Golpe.OiZuki, Golpe.MaeGeri, Golpe.MawashiGeri,
            Golpe.GedanBarai, Golpe.JodanUke
        };

        // embaralhar
        for (int i = 0; i < todosGolpes.Count; i++)
        {
            Golpe tmp = todosGolpes[i];
            int r = Random.Range(i, todosGolpes.Count);
            todosGolpes[i] = todosGolpes[r];
            todosGolpes[r] = tmp;
        }

        for (int i = 0; i < 5; i++)
        {
            Golpe g = todosGolpes[i % todosGolpes.Count];
            sequencia.Enqueue(g);

            GameObject icone = Instantiate(prefabIconeGolpe, containerSequencia);
            Image img = icone.GetComponent<Image>();
            if (img != null) img.sprite = SpriteDoGolpe(g);

            RectTransform rt = icone.GetComponent<RectTransform>();

            if (playerTransform != null && containerSequencia != null)
            {
                Vector2 spawn = containerSequencia.InverseTransformPoint(playerTransform.position);
                rt.anchoredPosition = spawn + new Vector2(0, -i * espacoEntreGolpes);
            }
            else
            {
                rt.anchoredPosition = new Vector2(0, -i * espacoEntreGolpes);
            }

            icones.Add(icone);
        }

        velocidadeHUD = (rodadaAtual == 1) ? velocidadeRodada1 : velocidadeRodada2;
        comboAtivo = true;
    }

    void MoverHUDVertical()
    {
        if (icones == null) return;

        for (int i = 0; i < icones.Count; i++)
        {
            if (icones[i] == null) continue;

            RectTransform rt = icones[i].GetComponent<RectTransform>();
            rt.anchoredPosition += Vector2.up * velocidadeHUD * Time.deltaTime;
        }
    }

    void VerificarAcerto(Golpe golpeTentado)
    {
        if (bloqueado) return; // ignora input enquanto bloqueado
        if (sequencia == null || sequencia.Count == 0 || icones == null || icones.Count == 0) return;

        // encontra primeiro ícone ativo (em casos onde algum foi desativado)
        GameObject iconeAtual = null;
        int indexAtual = -1;
        for (int i = 0; i < icones.Count; i++)
        {
            if (icones[i] != null && icones[i].activeInHierarchy)
            {
                iconeAtual = icones[i];
                indexAtual = i;
                break;
            }
        }

        if (iconeAtual == null) return;

        Golpe golpeCorreto = sequencia.Peek();
        RectTransform rt = iconeAtual.GetComponent<RectTransform>();

        // compara em screen space para garantir consistência entre hierarquias/canvases
        Vector2 iconScreen = RectTransformUtility.WorldToScreenPoint(null, rt.position);
        Vector2 pointerScreen = RectTransformUtility.WorldToScreenPoint(null, ponteiroCentral.position);
        float distancia = Mathf.Abs(iconScreen.y - pointerScreen.y);

        // Acertou
        if (golpeTentado == golpeCorreto && distancia < 60f)
        {
            // fala curta de acerto (não impede diálogos ambiente)
            if (dialogoSimples != null)
                dialogoSimples.MostrarDialogo("Haruki", null, "Bom!");

            StartCoroutine(FeedbackCor(iconeAtual, Color.green));
            AtivarAnimacaoGolpe(golpeTentado);

            // remove da fila lógica e da lista
            sequencia.Dequeue();
            if (indexAtual >= 0) icones.RemoveAt(indexAtual);

            // bloqueia input por um curto período para evitar leituras prematuras
            StartCoroutine(BloquearInputCurto(0.18f));

            if (sequencia.Count == 0)
                StartCoroutine(FinalizarRodada());
        }
        else
        {
            // fala de erro (não impede diálogos ambiente)
            if (dialogoSimples != null)
                dialogoSimples.MostrarDialogo("Instrutor", null, "Tente de novo!");

            StartCoroutine(FeedbackCor(iconeAtual, Color.red));
            rodadaAtual = 1;
            IniciarRodada();
        }
    }

    IEnumerator BloquearInputCurto(float duracao)
    {
        bloqueado = true;
        yield return new WaitForSeconds(duracao);
        bloqueado = false;
    }

    // não destrói imediatamente, apenas desativa (evita reordenações inesperadas)
    IEnumerator FeedbackCor(GameObject icone, Color cor)
    {
        if (icone != null)
        {
            Image img = icone.GetComponent<Image>();
            if (img != null) img.color = cor;

            yield return new WaitForSeconds(0.15f);

            if (icone != null)
                icone.SetActive(false); // não destrói, apenas oculta
        }
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

        // PARA O DIÁLOGO AMBIENTE antes de executar o especial/final
        StopAmbientDialogueIfNeeded();

        if (animatorHaruki != null)
        {
            if (TemParametro(animatorHaruki, parametroAura))
                animatorHaruki.SetBool(parametroAura, true);
        }

        // Executa golpe especial (uma vez) — só se o trigger existir
        if (animatorHaruki != null && TemParametro(animatorHaruki, triggerGolpeEspecial))
        {
            animatorHaruki.SetTrigger(triggerGolpeEspecial);
        }

        if (dialogoSimples != null)
        {
            dialogoSimples.MostrarDialogo("Instrutor", null, falaFinal);

            float timeout = 10f;
            float t = 0f;

            while (dialogoSimples.gameObject.activeSelf && t < timeout)
            {
                if (Input.anyKeyDown) break;

                t += Time.deltaTime;
                yield return null;
            }

            if (dialogoSimples.gameObject.activeSelf)
                dialogoSimples.FecharDialogo();
        }
        else
        {
            yield return new WaitForSeconds(0.25f);
        }

        yield return StartCoroutine(TransicaoFinal());
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
        if (animatorHaruki == null) yield break;

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
        }
        return null;
    }

    // -------------------- DIALOGOS AMBIENTE (MODO C: lista sequencial) --------------------------------

    void StartAmbientDialogueIfNeeded()
    {
        if (!dialogoAmbienteAtivo) return;
        if (dialogoSimples == null) return;
        if (coroutineAmbiente != null) return;
        if (dialogosAmbiente == null || dialogosAmbiente.Length == 0) return;

        indiceAmbiente = 0;
        coroutineAmbiente = StartCoroutine(AmbientDialogueLoop());
    }

    void StopAmbientDialogueIfNeeded()
    {
        if (coroutineAmbiente != null)
        {
            StopCoroutine(coroutineAmbiente);
            coroutineAmbiente = null;
        }

        // garante que o diálogo ambiente não fique aberto na tela
        if (dialogoSimples != null && dialogoSimples.gameObject.activeSelf)
            dialogoSimples.FecharDialogo();
    }

    IEnumerator AmbientDialogueLoop()
    {
        // roda enquanto o minigame estiver em andamento (ou até ser interrompido)
        while (true)
        {
            // se o minigame ainda não começou ou não estiver ativo, espera um frame
            if (!minigameIniciado || !comboAtivo)
            {
                yield return null;
                continue;
            }

            // pega próximo índice sequencial
            if (dialogosAmbiente != null && dialogosAmbiente.Length > 0)
            {
                string fala = dialogosAmbiente[indiceAmbiente];

                // mostra diálogo (independente do que já está acontecendo)
                dialogoSimples.MostrarDialogo("Instrutor", null, fala);

                // espera o tempo que a fala deve ficar visível
                float display = Mathf.Max(0.05f, duracaoFalaAmbiente);
                float elapsed = 0f;
                while (elapsed < display)
                {
                    // se a fase terminar, fecha e sai
                    if (!minigameIniciado) break;
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                // fecha fala ambiente
                if (dialogoSimples.gameObject.activeSelf)
                    dialogoSimples.FecharDialogo();

                // incrementa índice (cicla quando chegar ao fim)
                indiceAmbiente = (indiceAmbiente + 1) % dialogosAmbiente.Length;
            }

            // espera intervalo fixo até próxima fala
            float wait = Mathf.Max(0.05f, intervaloAmbiente);
            float w = 0f;
            while (w < wait)
            {
                if (!minigameIniciado) break;
                w += Time.deltaTime;
                yield return null;
            }
        }
    }

    // -------------------- TRANSIÇÃO / FADE --------------------------------

    IEnumerator TransicaoFinal()
    {
        if (fadeImage == null)
        {
            if (string.IsNullOrEmpty(nomeCenaFinal))
            {
                Debug.LogError("[GuitarHero] nomeCenaFinal vazio! Não há o que carregar.");
                yield break;
            }
            SceneManager.LoadScene(nomeCenaFinal);
            yield break;
        }

        if (!fadeImage.gameObject.activeSelf) fadeImage.gameObject.SetActive(true);
        Color c = fadeImage.color;
        fadeImage.color = new Color(c.r, c.g, c.b, 0f);

        yield return StartCoroutine(FadeOut());

        if (string.IsNullOrEmpty(nomeCenaFinal))
        {
            Debug.LogError("[GuitarHero] nomeCenaFinal vazio! Não há o que carregar.");
            yield break;
        }

        AsyncOperation ao = SceneManager.LoadSceneAsync(nomeCenaFinal);
        ao.allowSceneActivation = true;
        while (!ao.isDone) yield return null;
    }

    IEnumerator FadeOut()
    {
        float duracao = 1.2f;
        float t = 0f;

        if (fadeImage == null)
        {
            Debug.LogWarning("[GuitarHero] FadeOut chamado sem fadeImage.");
            yield break;
        }

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

        fadeImage.color = new Color(c.r, c.g, c.b, 1f);
        yield break;
    }

    // -------------------- HELPERS --------------------------------

    // verifica se o Animator tem um parâmetro com esse nome (compatível com todas as versões Unity)
    bool TemParametro(Animator anim, string nome)
    {
        if (anim == null || string.IsNullOrEmpty(nome)) return false;
        foreach (AnimatorControllerParameter p in anim.parameters)
        {
            if (p.name == nome) return true;
        }
        return false;
    }
}
