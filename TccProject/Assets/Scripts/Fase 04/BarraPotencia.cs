using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using FMODUnity;

public class BarraPotencia : MonoBehaviour
{
    [Header("Configuracão da Barra")]
    public Slider barra;
    public float velocidade = 2f;
    private bool subindo = true;
    public DialogoSimples dialogoSimples;
    public Sprite spriteDoPersonagemHaruki;
    public string FalaDoPersonagemVitoria;
    public string FalaDoPersonagemDerrota;

    [Header("Tecla de Acão (PC)")]
    public KeyCode teclaAcao = KeyCode.Space;

    [Header("Configuracão do Animator")]
    public Animator animator;
    public string parametroMaeGeri = "MaeGeri";
    public string parametroDefesa = "Defesa";

    [Header("Zonas de Acerto")]
    [Range(0f, 1f)] public float zonaPerfeitaCentro = 0.50f;
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

    void Start()
    {
        AtualizarBarraEstabilidade();
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        if (modoDefesa)
            StartCoroutine(AtacarJogador());
    }

    void Update()
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

        if (!executandoGolpe && Input.GetKeyDown(teclaAcao))
        {
            if (modoDefesa)
                VerificarDefesa();
            else
                VerificarAcerto();
        }
    }

    public void AcionarGolpe()
    {
        if (!executandoGolpe && !modoDefesa)
        {
            VerificarAcerto();
        }
    }

    void VerificarAcerto()
    {
        float valor = barra.value;
        float distanciaDoCentro = Mathf.Abs(valor - zonaPerfeitaCentro);

        if (distanciaDoCentro <= toleranciaCentro)
        {
            BalancarAlvo(anguloAtaqueForte, eixoDoGolpe);
            acertosSeguidos++;
            aumentarEstabilidade(0.2f);
            StartCoroutine(ExecutarGolpe(1.2f, false));
        }
        else if (distanciaDoCentro <= toleranciaBoa)
        {
            BalancarAlvo(anguloAtaqueMedio, eixoDoGolpe);
            acertosSeguidos = 0;
            diminuirEstabilidade(0.15f);
            StartCoroutine(ExecutarGolpe(0.8f, false));
        }
        else
        {
            BalancarAlvo(anguloAtaqueFraco, eixoDoGolpe);
            acertosSeguidos = 0;
            diminuirEstabilidade(0.3f);
            StartCoroutine(ExecutarGolpe(0.5f, true));
        }

        ChecarVitoriaOuDerrota();
    }

    IEnumerator AtacarJogador()
    {
        while (modoDefesa)
        {
            yield return new WaitForSeconds(intervaloAtaque - tempoAntesChute);
            if (animatorSensei != null && !string.IsNullOrEmpty(parametroChuteSensei))
                animatorSensei.SetBool(parametroChuteSensei, true);

            yield return new WaitForSeconds(tempoAntesChute);
            BalancarAlvo(anguloAtaqueInimigo, eixoDoInimigo);
            if (animatorSensei != null && !string.IsNullOrEmpty(parametroChuteSensei))
                animatorSensei.SetBool(parametroChuteSensei, false);
        }
    }

    void VerificarDefesa()
    {
        float valor = barra.value;
        float distanciaDoCentro = Mathf.Abs(valor - zonaPerfeitaCentro);
        if (animator != null && !string.IsNullOrEmpty(parametroDefesa))
            StartCoroutine(ExecutarDefesa());

        if (distanciaDoCentro <= toleranciaCentro)
        {
            acertosSeguidos++;
            aumentarEstabilidade(0.2f);
        }
        else if (distanciaDoCentro <= toleranciaBoa)
        {
            acertosSeguidos = 0;
            diminuirEstabilidade(0.15f);
            falhasDefesa++;
        }
        else
        {
            acertosSeguidos = 0;
            diminuirEstabilidade(0.3f);
            falhasDefesa++;
        }

        if (falhasDefesa >= falhasParaDerrota)
        {
            StartCoroutine(FinalizarDepoisDialogo(nomeCenaDerrota));
        }

        ChecarVitoriaOuDerrota();
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
        if (acertosSeguidos >= acertosParaVencer && !modoDefesa)
        {
            dialogoSimples.MostrarDialogo("Haruki", spriteDoPersonagemHaruki, FalaDoPersonagemVitoria);
            StartCoroutine(FinalizarDepoisDialogo(nomeCenaVitoria));
        }
        else if (estabilidadeAtual <= 0 && !jaMostrouDerrota)
        {
            jaMostrouDerrota = true;
            dialogoSimples.MostrarDialogo("Haruki", spriteDoPersonagemHaruki, FalaDoPersonagemDerrota);
            StartCoroutine(FinalizarDepoisDialogo(nomeCenaDerrota));
        }
    }

    IEnumerator FinalizarDepoisDialogo(string cena)
    {
        yield return new WaitForSeconds(5f);
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
}

IEnumerator ExecutarDefesa()
{
    if (!string.IsNullOrEmpty(eventoDefesa))
        RuntimeManager.PlayOneShot(eventoDefesa, transform.position);

    animator.SetBool(parametroDefesa, true);
    yield return new WaitForSeconds(0.6f);
    animator.SetBool(parametroDefesa, false);
}

}
