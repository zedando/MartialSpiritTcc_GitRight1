using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    [Header("Referências")]
    public Transform alvo;
    public FighterController playerController;

    private CharacterController controller;

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
    public string cenaAoMorrer = "CenaVitoria";

    [Header("Dialogo e Fade")]
    public DialogoSimples dialogoSimples;
    public Sprite spriteJuiz;
    public Image fadeImage;

    private bool atacando = false;
    private bool morto = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

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
            direcaoRecuo.y = 0;

            controller.Move(direcaoRecuo * velocidade * Time.deltaTime);

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
        direcao.y = 0;

        controller.Move(direcao * velocidade * Time.deltaTime);

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
            StartCoroutine(Morrer());  
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

        yield return new WaitForSeconds(1.2f);

        if (dialogoSimples != null)
        {
            dialogoSimples.MostrarDialogo("Juiz", spriteJuiz, "O Kenji foi derrotado por pontos!");
            yield return new WaitForSeconds(6f);
            dialogoSimples.FecharDialogo();
        }

        if (fadeImage != null)
        {
            if (!fadeImage.gameObject.activeSelf)
                fadeImage.gameObject.SetActive(true);

            Color c = fadeImage.color;
            float t = 0f;
            float duracao = 1.2f;

            while (t < duracao)
            {
                t += Time.deltaTime;
                fadeImage.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0, 1, t / duracao));
                yield return null;
            }
        }

        SceneManager.LoadScene(cenaAoMorrer);
    }

    IEnumerator ImpactShake()
    {
        float shakeTempo = 0.2f;
        float shakeForca = 0.1f;
        float timer = 0f;

        while (timer < shakeTempo)
        {
            Vector3 offset = (Vector3)Random.insideUnitCircle * shakeForca;
            offset.y = 0;

            controller.Move(offset);

            timer += Time.deltaTime;
            yield return null;
        }
    }

    void AtualizarBarras()
    {
        if (barraVida != null)
            barraVida.value = vidaAtual / vidaMax;
        if (barraEstamina != null)
            barraEstamina.value = estaminaAtual / estaminaMax;
    }
}
