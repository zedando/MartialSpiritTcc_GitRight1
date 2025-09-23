using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Barras")]
    public UnityEngine.UI.Slider barraVida;
    public float vidaMax = 100f;

    [Header("Animator")]
    public Animator animator;
    public string parametroIdle = "Idle";
    public string parametroAtaque = "Ataque";
    public string parametroDefesa = "Defesa";

    [Header("Golpes")]
    public float danoAtaque = 12f;

    [Header("Referencia do Player")]
    public FighterController player;

    [Header("Efeitos")]
    public GameObject efeitoImpactoPrefab;

    private float vidaAtual;
    private bool atacando = false;
    private bool defendendo = false;

    void Start()
    {
        vidaAtual = vidaMax;
        AtualizarBarra();
        StartCoroutine(AI());
    }

    IEnumerator AI()
    {
        while (vidaAtual > 0)
        {
            float decisao = Random.value;

            if (decisao < 0.4f) yield return StartCoroutine(ExecutarAtaque());
            else if (decisao < 0.7f) yield return StartCoroutine(ExecutarDefesa());
            else yield return new WaitForSeconds(0.5f);

            yield return new WaitForSeconds(0.3f); // pequeno delay entre ações
        }
    }

    IEnumerator ExecutarAtaque()
    {
        if (atacando) yield break;

        atacando = true;
        animator.SetBool(parametroAtaque, true);

        yield return new WaitForSeconds(0.4f); // ataque rápido

        if (player != null)
            player.ReceberDano(danoAtaque, efeitoImpactoPrefab);

        animator.SetBool(parametroAtaque, false);
        animator.SetBool(parametroIdle, true);
        yield return new WaitForSeconds(0.05f);
        animator.SetBool(parametroIdle, false);

        atacando = false;
    }

    IEnumerator ExecutarDefesa()
    {
        if (defendendo) yield break;

        defendendo = true;
        animator.SetBool(parametroDefesa, true);

        yield return new WaitForSeconds(0.4f);

        animator.SetBool(parametroDefesa, false);
        animator.SetBool(parametroIdle, true);
        yield return new WaitForSeconds(0.05f);
        animator.SetBool(parametroIdle, false);

        defendendo = false;
    }

    public void ReceberDano(float dano, GameObject efeitoImpacto = null)
    {
        if (defendendo)
            dano *= 0.5f;

        vidaAtual -= dano;
        if (vidaAtual < 0) vidaAtual = 0;
        AtualizarBarra();

        if (efeitoImpacto != null)
        {
            Instantiate(efeitoImpacto, transform.position + Vector3.left * 0.5f, Quaternion.identity);
        }

        StartCoroutine(ImpactShake());

        if (vidaAtual <= 0)
        {
            Debug.Log("Inimigo derrotado!");
        }
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

    void AtualizarBarra()
    {
        if (barraVida != null)
            barraVida.value = vidaAtual / vidaMax;
    }
}
