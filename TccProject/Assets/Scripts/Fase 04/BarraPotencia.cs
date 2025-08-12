using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

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

    [Header("Zonas de Acerto")]
    [Range(0f, 1f)] public float zonaPerfeitaCentro = 0.50f;
    public float toleranciaCentro = 0.03f;
    public float toleranciaBoa = 0.15f;

    [Header("Alvo para Balancar")]
    public AlvoBalanco alvo; // arraste a esfera aqui

    [Header("Sistema de Estabilidade")]
    public Slider barraEstabilidade;  // assign no Inspector, barra que mostra estabilidade
    private float estabilidadeAtual = 1f;  // comeca cheia
    private int acertosSeguidos = 0;
    public int acertosParaVencer = 3;

    [Header("Fade e Cena")]
    public CanvasGroup fadeCanvasGroup;  // assign no Inspector, painel preto para fade
    public float duracaoFade = 2f;
    public string nomeCenaVitoria = "CenaVitoria";
    public string nomeCenaDerrota = "CenaDerrota";

    private bool executandoGolpe = false;
    private bool jaMostrouDerrota = false;

    void Start()
    {
        AtualizarBarraEstabilidade();
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f; // garante que comece transparente
    }

    void Update()
    {
        // Oscilacão da barra
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
            VerificarAcerto();
        }
    }

    public void AcionarGolpe()
    {
        if (!executandoGolpe)
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
            Debug.Log("Centro Exato! Golpe perfeito.");
            alvo.Balancar(20f);
            acertosSeguidos++;
            aumentarEstabilidade(0.2f);  // recupera um pouco
            StartCoroutine(ExecutarGolpe(1.2f, false));
        }
        else if (distanciaDoCentro <= toleranciaBoa)
        {
            Debug.Log("Bom timing! Mas ainda pode melhorar.");
            alvo.Balancar(12f);
            acertosSeguidos = 0;  // perdeu sequência
            diminuirEstabilidade(0.15f);
            StartCoroutine(ExecutarGolpe(0.8f, false));
        }
        else
        {
            Debug.Log("Muito fraco. Concentre-se mais.");
            alvo.Balancar(5f);
            acertosSeguidos = 0;  // perdeu sequência
            diminuirEstabilidade(0.3f);
            StartCoroutine(ExecutarGolpe(0.5f, true));
        }

        ChecarVitoriaOuDerrota();
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
        if(barraEstabilidade != null)
            barraEstabilidade.value = estabilidadeAtual;
    }

    void ChecarVitoriaOuDerrota()
    {
        if (acertosSeguidos >= acertosParaVencer)
        {
            Debug.Log("Parabéns! Você venceu a fase!");
            dialogoSimples.MostrarDialogo(
                "Haruki",
                spriteDoPersonagemHaruki,
                FalaDoPersonagemVitoria
            );
            StartCoroutine(FinalizarDepoisDialogo(nomeCenaVitoria));
        }
        else if (estabilidadeAtual <= 0 && !jaMostrouDerrota)
        {
            jaMostrouDerrota = true;
            Debug.Log("Game Over! Estabilidade zerada.");
            dialogoSimples.MostrarDialogo(
                "Haruki",
                spriteDoPersonagemHaruki,
                FalaDoPersonagemDerrota
            );
            StartCoroutine(FinalizarDepoisDialogo(nomeCenaDerrota));
        }
    }

    IEnumerator FinalizarDepoisDialogo(string cena)
    {
        yield return new WaitForSeconds(5f);

        if(fadeCanvasGroup != null)
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
}
