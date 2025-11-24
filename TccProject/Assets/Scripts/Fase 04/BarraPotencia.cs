using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class BarraPotencia : MonoBehaviour
{
    [Header("Configuracão da Barra")]
    public Slider barra;
    public float velocidade = 2f;
    private bool subindo = true;

    [Header("Sistema de Diálogo")]
    public DialogoSimples dialogoSimples;
    public Sprite spriteDoPersonagemHaruki;
    public string FalaDoPersonagemVitoria;
    public string FalaDoPersonagemDerrota;

    [Header("Som de Diálogo (FMOD)")]
    public EventReference somDialogo;
    private EventInstance dialogInstance;
    private bool somDialogoAtivo = false;

    [Header("Falas Iniciais (opcional)")]
    public bool usarFalasIniciais = false;
    [TextArea] public string[] falasIniciais;
    public float tempoCadaFala = 2f;

    [Header("Tecla de Acão (PC)")]
    public KeyCode teclaAcao = KeyCode.Space;

    [Header("Configuracão do Animator")]
    public Animator animator;
    public string parametroMaeGeri = "MaeGeri";
    public string parametroDefesa = "Defesa";

    [Header("Zonas de Acerto")]
    [Range(0f, 1f)]
    public float zonaPerfeitaCentro = 0.50f;
    public float toleranciaCentro = 0.03f;
    public float toleranciaBoa = 0.15f;

    [Header("Alvo para Balancar")]
    public AlvoBalanco alvo;

    [Header("Sistema de Estabilidade")]
    public Slider barraEstabilidade;
    private float estabilidadeAtual = 1f;

    private int acertosSeguidos = 0;
    public int acertosParaVencer = 3;

    [Header("Fade e Cena")]
    public CanvasGroup fadeCanvasGroup;
    public float duracaoFade = 2f;
    public string nomeCenaVitoria = "CenaVitoria";
    public string nomeCenaDerrota = "CenaDerrota";

    private bool executandoGolpe = false;
    private bool jaMostrouDerrota = false;

    [Header("Ângulos de Balanço do Ataque")]
    public float anguloAtaqueForte = 20f;
    public float anguloAtaqueMedio = 12f;
    public float anguloAtaqueFraco = 5f;

    [Header("Ângulo do ataque inimigo")]
    public float anguloAtaqueInimigo = -20f;

    public enum EixoBalanço { X, Y, Z }

    [Header("Eixo do balanço do golpe do jogador")]
    public EixoBalanço eixoDoGolpe = EixoBalanço.X;

    [Header("Eixo do balanço do ataque inimigo")]
    public EixoBalanço eixoDoInimigo = EixoBalanço.X;

    [Header("Configuração de Defesa")]
    public bool modoDefesa = false;
    public float intervaloAtaque = 2f;
    public float tempoAntesChute = 0.5f;

    [Header("Animator do Sensei")]
    public Animator animatorSensei;
    public string parametroChuteSensei = "Chute";

    private int falhasDefesa = 0;
    public int falhasParaDerrota = 3;

    [EventRef] public string eventoAtaque = "event:/SocoChute/chuteVento";
    [EventRef] public string eventoDefesa = "event:/SocoChute/chuteVento2";

    // -----------------------------
    // JANELA DE DEFESA
    // -----------------------------
    private bool janelaDeDefesaAtiva = false;
    public float tempoJanelaDefesa = 0.25f;

    private bool ataqueCancelado = false;

    private Coroutine atacarCoroutine = null;

    // -----------------------------
    // Sincronizar ataque com barra
    // -----------------------------
    [Header("Sincronizar ataque com barra")]
    public bool sincronizarComBarra = true;
    public float alvoAtaqueBarra = 0.5f;
    public float toleranciaAtaqueBarra = 0.015f;

    [Header("Delay opcional para iniciar diálogo")]
    public bool iniciarComDelay = false;
    public float tempoDelayInicio = 11f;

    // ==========================================================
    // START
    // ==========================================================
    void Start()
    {
        AtualizarBarraEstabilidade();

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        StartCoroutine(IniciarFaseComPossivelFalaInicial());
    }

    // ==========================================================
    // SOM DE DIÁLOGO
    // ==========================================================
    private void IniciarSomDialogo()
    {
        if (somDialogoAtivo || somDialogo.IsNull) return;

        dialogInstance = RuntimeManager.CreateInstance(somDialogo);
        dialogInstance.start();
        somDialogoAtivo = true;
    }

   private void PararSomDialogo()
{
    if (!somDialogoAtivo) return;

    dialogInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    dialogInstance.release();
    somDialogoAtivo = false;
}

    private void OnDestroy()
    {
        PararSomDialogo();
    }

    // ==========================================================
    // FALA INICIAL (AGORA MÚLTIPLA)
    // ==========================================================
IEnumerator IniciarFaseComPossivelFalaInicial()
{
    // --- SE EU MARCAR NO INSPECTOR, ESPERA 11s (OU O VALOR QUE EU PUSER) ---
    if (iniciarComDelay)
        yield return new WaitForSeconds(tempoDelayInicio);

    yield return null;

    // --- FALAS INICIAIS MÚLTIPLAS ---
    if (usarFalasIniciais && dialogoSimples != null && falasIniciais != null && falasIniciais.Length > 0)
    {
        executandoGolpe = true;

        foreach (string fala in falasIniciais)
        {
            if (string.IsNullOrWhiteSpace(fala)) continue;

            dialogoSimples.MostrarDialogo("Haruki", spriteDoPersonagemHaruki, fala);
            IniciarSomDialogo();

            yield return new WaitForSeconds(tempoCadaFala);

            PararSomDialogo();
            dialogoSimples.FecharDialogo();

            yield return new WaitForSeconds(0.1f);
        }

        executandoGolpe = false;
    }

    // --- MODO DEFESA INICIA APÓS DIÁLOGO ---
    if (modoDefesa)
        atacarCoroutine = StartCoroutine(AtacarJogador());
}

    // ==========================================================
    // UPDATE
    // ==========================================================
    void Update()
    {
        if (!executandoGolpe)
            AtualizarMovimentoBarra();

        if (!executandoGolpe && Input.GetKeyDown(teclaAcao))
        {
            if (modoDefesa)
                VerificarDefesa();
            else
                VerificarAcerto();
        }
    }

    void AtualizarMovimentoBarra()
    {
        if (subindo)
        {
            barra.value += velocidade * Time.deltaTime;
            if (barra.value >= 1f) subindo = false;
        }
        else
        {
            barra.value -= velocidade * Time.deltaTime;
            if (barra.value <= 0f) subindo = true;
        }
    }

    public void AcionarGolpe()
    {
        if (!executandoGolpe && !modoDefesa)
            VerificarAcerto();
    }

    // -----------------------------
    // ATAQUE NORMAL
    // -----------------------------
    void VerificarAcerto()
    {
        float valor = barra.value;
        float distancia = Mathf.Abs(valor - zonaPerfeitaCentro);

        if (distancia <= toleranciaCentro)
        {
            BalancarAlvo(anguloAtaqueForte, eixoDoGolpe);
            acertosSeguidos++;
            aumentarEstabilidade(0.20f);
            StartCoroutine(ExecutarGolpe(1.2f, false));
        }
        else if (distancia <= toleranciaBoa)
        {
            BalancarAlvo(anguloAtaqueMedio, eixoDoGolpe);
            acertosSeguidos++;
            diminuirEstabilidade(0.10f);
            StartCoroutine(ExecutarGolpe(0.9f, false));
        }
        else
        {
            BalancarAlvo(anguloAtaqueFraco, eixoDoGolpe);
            acertosSeguidos = 0;
            diminuirEstabilidade(0.25f);
            StartCoroutine(ExecutarGolpe(0.6f, true));
        }
    }

    // ==========================================================
    // ATAQUE DO SENSEI — SINCRONIZADO COM A BARRA
    // ==========================================================
    IEnumerator AtacarJogador()
    {
        while (modoDefesa)
        {
            ataqueCancelado = false;

            if (sincronizarComBarra)
            {
                yield return StartCoroutine(EsperarBarraChegarNoPonto());
            }
            else
            {
                yield return new WaitForSeconds(intervaloAtaque - tempoAntesChute);
            }

            if (animatorSensei != null)
                animatorSensei.SetBool(parametroChuteSensei, true);

            yield return new WaitForSeconds(tempoAntesChute - tempoJanelaDefesa);

            janelaDeDefesaAtiva = true;
            yield return new WaitForSeconds(tempoJanelaDefesa);
            janelaDeDefesaAtiva = false;

            if (!ataqueCancelado)
                BalancarAlvo(anguloAtaqueInimigo, eixoDoInimigo);

            if (animatorSensei != null)
                animatorSensei.SetBool(parametroChuteSensei, false);

            yield return null;
        }
    }

    IEnumerator EsperarBarraChegarNoPonto()
    {
        bool chegou = false;

        while (!chegou && modoDefesa)
        {
            float valor = barra.value;
            float dist = Mathf.Abs(valor - alvoAtaqueBarra);

            if (dist <= toleranciaAtaqueBarra)
                chegou = true;

            yield return null;
        }
    }

    // -----------------------------
    // DEFESA
    // -----------------------------
    void VerificarDefesa()
    {
        float valor = barra.value;
        float distancia = Mathf.Abs(valor - zonaPerfeitaCentro);

        StartCoroutine(ExecutarDefesa());

        bool barraCerta = distancia <= toleranciaBoa;

        if (!janelaDeDefesaAtiva)
        {
            ErroDefesa();
            return;
        }

        ataqueCancelado = true;
        acertosSeguidos++;
        aumentarEstabilidade(0.15f);

        if (acertosSeguidos >= acertosParaVencer)
        {
            modoDefesa = false;
            if (atacarCoroutine != null) StopCoroutine(atacarCoroutine);

            StartCoroutine(MostrarDialogoEFinalizar("Haruki", spriteDoPersonagemHaruki, FalaDoPersonagemVitoria, nomeCenaVitoria));
        }
    }

    void ErroDefesa()
    {
        acertosSeguidos = 0;
        diminuirEstabilidade(0.2f);
        falhasDefesa++;

        if (falhasDefesa >= falhasParaDerrota)
        {
            modoDefesa = false;

            if (atacarCoroutine != null)
            {
                StopCoroutine(atacarCoroutine);
                atacarCoroutine = null;
            }

            StartCoroutine(MostrarDialogoEFinalizar("Haruki", spriteDoPersonagemHaruki, FalaDoPersonagemDerrota, nomeCenaDerrota));
        }
    }

    void BalancarAlvo(float angulo, EixoBalanço eixo)
    {
        if (alvo == null) return;

        switch (eixo)
        {
            case EixoBalanço.X: alvo.BalancarComEixo(angulo, 0, 0); break;
            case EixoBalanço.Y: alvo.BalancarComEixo(0, angulo, 0); break;
            case EixoBalanço.Z: alvo.BalancarComEixo(0, 0, angulo); break;
        }
    }

    void aumentarEstabilidade(float valor)
    {
        estabilidadeAtual += valor;
        if (estabilidadeAtual > 1f) estabilidadeAtual = 1f;
        AtualizarBarraEstabilidade();
    }

    void diminuirEstabilidade(float valor)
    {
        estabilidadeAtual -= valor;
        if (estabilidadeAtual < 0f) estabilidadeAtual = 0f;
        AtualizarBarraEstabilidade();
    }

    void AtualizarBarraEstabilidade()
    {
        if (barraEstabilidade != null)
            barraEstabilidade.value = estabilidadeAtual;
    }

    void ChecarVitoriaOuDerrota()
    {
        if (acertosSeguidos >= acertosParaVencer)
        {
            modoDefesa = false;

            if (atacarCoroutine != null)
            {
                StopCoroutine(atacarCoroutine);
                atacarCoroutine = null;
            }

            StartCoroutine(MostrarDialogoEFinalizar("Haruki", spriteDoPersonagemHaruki, FalaDoPersonagemVitoria, nomeCenaVitoria));
            return;
        }

        if (!jaMostrouDerrota && estabilidadeAtual <= 0f)
        {
            jaMostrouDerrota = true;
            modoDefesa = false;

            if (atacarCoroutine != null)
            {
                StopCoroutine(atacarCoroutine);
                atacarCoroutine = null;
            }

            StartCoroutine(MostrarDialogoEFinalizar("Haruki", spriteDoPersonagemHaruki, FalaDoPersonagemDerrota, nomeCenaDerrota));
        }
    }

    IEnumerator FinalizarDepoisDialogo(string cena)
    {
        // tempo pro diálogo ficar na tela
        yield return new WaitForSeconds(5f);

        // para som de diálogo antes de fade/cena
        PararSomDialogo();

        if (fadeCanvasGroup != null)
        {
            float tempo = 0f;
            while (tempo < duracaoFade)
            {
                tempo += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(tempo / duracaoFade);
                yield return null;
            }
        }

        SceneManager.LoadScene(cena);
    }

    IEnumerator ExecutarGolpe(float speed, bool cortarNoMeio)
    {
        executandoGolpe = true;

        if (!string.IsNullOrEmpty(eventoAtaque))
            RuntimeManager.PlayOneShot(eventoAtaque, transform.position);

        animator.speed = speed;
        animator.SetBool(parametroMaeGeri, true);

        if (cortarNoMeio)
            yield return new WaitForSeconds(0.4f / speed);
        else
            yield return new WaitForSeconds(0.9f / speed);

        animator.SetBool(parametroMaeGeri, false);
        animator.speed = 1f;

        yield return new WaitForSeconds(0.05f);

        executandoGolpe = false;

        ChecarVitoriaOuDerrota();
    }

    IEnumerator ExecutarDefesa()
    {
        executandoGolpe = true;

        if (!string.IsNullOrEmpty(eventoDefesa))
            RuntimeManager.PlayOneShot(eventoDefesa, transform.position);

        animator.SetBool(parametroDefesa, true);
        yield return new WaitForSeconds(0.6f);
        animator.SetBool(parametroDefesa, false);

        executandoGolpe = false;

        ChecarVitoriaOuDerrota();
    }

    IEnumerator MostrarDialogoComDelay(string nome, Sprite sprite, string fala)
    {
        yield return new WaitForSeconds(0.05f);

        if (dialogoSimples != null)
        {
            dialogoSimples.MostrarDialogo(nome, sprite, fala);
            IniciarSomDialogo();
        }
    }

    IEnumerator MostrarDialogoEFinalizar(string nome, Sprite sprite, string fala, string cena)
    {
        yield return StartCoroutine(MostrarDialogoComDelay(nome, sprite, fala));
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(FinalizarDepoisDialogo(cena));
    }
}
