using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    [Header("Referências")]
    public Transform alvo;
    public FighterController playerController;

    [Header("Status")]
    public float vidaMax = 100f;
    private float vidaAtual;
    public Slider barraVida;
    public float estaminaMax = 100f;
    private float estaminaAtual;
    public Slider barraEstamina;

    [Header("Movimento")]
    public float velocidade = 3f;
    public float distanciaAtaque = 2f;

    [Header("Animator")]
    public Animator animator;
    public string parametroCorrendo = "Correndo";
    public string parametroAtaque = "Ataque";
    public string parametroMorte = "Morte";
    public string parametroIdle = "Idle";

    [Header("Ataque")]
    public float danoAtaque = 10f;
    public GameObject efeitoImpactoPrefab;

    [Header("Controle Luta")]
    public bool lutaIniciada = false;

    [Header("Cena ao morrer")]
    public string cenaAoMorrer = "CenaVitoria"; // Coloque aqui no Inspector

    private bool atacando = false;
    private bool morto = false;

    void Start()
    {
        vidaAtual = vidaMax;
        estaminaAtual = estaminaMax;
        AtualizarBarras();
    }

    void Update()
    {
        if (!lutaIniciada || morto || atacando) return;
        if (alvo == null || playerController == null) return;

        float distancia = Vector3.Distance(transform.position, alvo.position);

        // Recuperar estamina recuando
        if (estaminaAtual < 20f)
        {
            Vector3 direcaoRecuo = (transform.position - alvo.position).normalized;
            transform.position += direcaoRecuo * velocidade * Time.deltaTime;
            if (animator != null)
                animator.SetBool(parametroCorrendo, true);

            estaminaAtual += 15f * Time.deltaTime;
            if (estaminaAtual > estaminaMax) estaminaAtual = estaminaMax;
            AtualizarBarras();
            return;
        }

        // Mover até o player
        if (distancia > distanciaAtaque)
        {
            MoverAtrasDoPlayer();
        }
        else
        {
            StartCoroutine(ExecutarAtaque());
        }
    }

    void MoverAtrasDoPlayer()
    {
        Vector3 direcao = (alvo.position - transform.position).normalized;
        transform.position += direcao * velocidade * Time.deltaTime;

        transform.LookAt(new Vector3(alvo.position.x, transform.position.y, alvo.position.z));

        if (animator != null)
            animator.SetBool(parametroCorrendo, true);

        estaminaAtual += 10f * Time.deltaTime;
        if (estaminaAtual > estaminaMax) estaminaAtual = estaminaMax;
        AtualizarBarras();
    }

    IEnumerator ExecutarAtaque()
    {
        if (estaminaAtual < 20f) yield break;

        atacando = true;

        if (animator != null)
        {
            animator.SetBool(parametroCorrendo, false);
            animator.SetBool(parametroAtaque, true);
        }

        yield return new WaitForSeconds(0.5f);

        if (playerController != null)
            playerController.ReceberDano(danoAtaque, efeitoImpactoPrefab);

        estaminaAtual -= 20f;
        AtualizarBarras();

        if (animator != null)
            animator.SetBool(parametroAtaque, false);

        yield return new WaitForSeconds(0.8f);
        atacando = false;
    }

    public void ReceberDano(float dano, GameObject efeitoImpacto = null)
    {
        if (morto) return;

        vidaAtual -= dano;
        if (vidaAtual <= 0)
        {
            vidaAtual = 0;
            StartCoroutine(Morrer());  // 👈 Usa coroutine para esperar animação
        }

        if (efeitoImpacto != null)
            Instantiate(efeitoImpacto, transform.position + Vector3.up * 1f, Quaternion.identity);

        StartCoroutine(ImpactShake());
        AtualizarBarras();
    }

    IEnumerator Morrer()
    {
        morto = true;

        if (animator != null)
        {
            animator.SetBool(parametroCorrendo, false);
            animator.SetBool(parametroAtaque, false);
            animator.SetTrigger(parametroMorte);
        }

        Debug.Log("Inimigo derrotado!");

        // Espera animação de morte (ajuste conforme sua animação)
        yield return new WaitForSeconds(1.2f);

        // Troca de cena
        SceneManager.LoadScene(cenaAoMorrer);
    }

    IEnumerator ImpactShake()
    {
        Vector3 posOriginal = transform.position;
        float shakeTempo = 0.2f;
        float shakeForca = 0.1f;
        float timer = 0f;

        while (timer < shakeTempo)
        {
            transform.position = posOriginal + (Vector3)Random.insideUnitCircle * shakeForca;
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = posOriginal;
    }

    void AtualizarBarras()
    {
        if (barraVida != null)
            barraVida.value = vidaAtual / vidaMax;
        if (barraEstamina != null)
            barraEstamina.value = estaminaAtual / estaminaMax;
    }
}
