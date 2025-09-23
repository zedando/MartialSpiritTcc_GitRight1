using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FighterController : MonoBehaviour
{
    [Header("Barras")]
    public Slider barraVida;
    public Slider barraEstamina;
    public float vidaMax = 100f;
    public float estaminaMax = 100f;

    [Header("Animator")]
    public Animator animator;
    public string parametroIdle = "Idle";
    public string parametroOiZuki = "OiZuki";
    public string parametroMaeGeri = "MaeGeri";
    public string parametroMawashiGeri = "MawashiGeri";
    public string parametroGedanBarai = "GedanBarai";
    public string parametroJodanUke = "JodanUke";
    public string parametroDefesa = "Defesa";

    [Header("Golpes")]
    public float danoOiZuki = 10f;
    public float danoMaeGeri = 15f;
    public float danoMawashiGeri = 20f;
    public float danoGedanBarai = 0f; // Defesa
    public float danoJodanUke = 0f; // Defesa

    [Header("Teclas")]
    public KeyCode teclaOiZuki = KeyCode.J;
    public KeyCode teclaMaeGeri = KeyCode.K;
    public KeyCode teclaMawashiGeri = KeyCode.U;
    public KeyCode teclaGedanBarai = KeyCode.L;
    public KeyCode teclaJodanUke = KeyCode.S;

    [Header("Referencia do Inimigo")]
    public EnemyController inimigo;

    [Header("Efeitos")]
    public GameObject efeitoGolpePrefab;
    public GameObject efeitoImpactoPrefab;
    public GameObject efeitoTrilhaGolpe;

    private float vidaAtual;
    private float estaminaAtual;
    private bool golpeExecutando = false;
    private bool defendendo = false;

    void Start()
    {
        vidaAtual = vidaMax;
        estaminaAtual = estaminaMax;
        AtualizarBarras();
    }

    void Update()
    {
        if (golpeExecutando) return;

        if (Input.GetKeyDown(teclaOiZuki)) StartCoroutine(ExecutarGolpe(parametroOiZuki, danoOiZuki));
        if (Input.GetKeyDown(teclaMaeGeri)) StartCoroutine(ExecutarGolpe(parametroMaeGeri, danoMaeGeri));
        if (Input.GetKeyDown(teclaMawashiGeri)) StartCoroutine(ExecutarGolpe(parametroMawashiGeri, danoMawashiGeri));

        if (Input.GetKeyDown(teclaGedanBarai)) StartCoroutine(ExecutarDefesa(parametroGedanBarai));
        if (Input.GetKeyDown(teclaJodanUke)) StartCoroutine(ExecutarDefesa(parametroJodanUke));
    }

    IEnumerator ExecutarGolpe(string parametro, float dano)
    {
        golpeExecutando = true;

        // Spawn do efeito de golpe
        if (efeitoGolpePrefab != null)
        {
            Instantiate(efeitoGolpePrefab, transform.position + Vector3.right * 1f, Quaternion.identity);
        }

        // Spawn da trilha luminosa
        if (efeitoTrilhaGolpe != null)
        {
            GameObject trilha = Instantiate(efeitoTrilhaGolpe, transform.position + Vector3.right * 1f, Quaternion.identity);
            Destroy(trilha, 0.5f);
        }

        animator.SetBool(parametro, true);
        yield return new WaitForSeconds(0.4f); // Golpe rápido
        animator.SetBool(parametro, false);

        animator.SetBool(parametroIdle, true);
        yield return new WaitForSeconds(0.05f);
        animator.SetBool(parametroIdle, false);

        if (inimigo != null)
            inimigo.ReceberDano(dano, efeitoImpactoPrefab);

        golpeExecutando = false;
    }

    IEnumerator ExecutarDefesa(string parametro)
    {
        defendendo = true;
        animator.SetBool(parametroDefesa, true);
        animator.SetBool(parametro, true);

        yield return new WaitForSeconds(0.4f);

        animator.SetBool(parametro, false);
        animator.SetBool(parametroDefesa, false);
        animator.SetBool(parametroIdle, true);
        yield return new WaitForSeconds(0.05f);
        animator.SetBool(parametroIdle, false);

        defendendo = false;
    }

    public void ReceberDano(float dano, GameObject efeitoImpacto = null)
    {
        if (defendendo)
            dano *= 0.3f;

        vidaAtual -= dano;
        if (vidaAtual < 0) vidaAtual = 0;
        AtualizarBarras();

        if (efeitoImpacto != null)
        {
            Instantiate(efeitoImpacto, transform.position + Vector3.right * 0.5f, Quaternion.identity);
        }

        StartCoroutine(ImpactShake());

        if (vidaAtual <= 0)
        {
            Debug.Log("Player derrotado!");
            animator.SetBool(parametroIdle, false);
            animator.SetBool(parametroDefesa, false);
            // Aqui pode chamar fim de combate ou cena de derrota
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

    void AtualizarBarras()
    {
        if (barraVida != null) barraVida.value = vidaAtual / vidaMax;
        if (barraEstamina != null) barraEstamina.value = estaminaAtual / estaminaMax;
    }

    // -----------------------------
    // Métodos OnClick para Mobile
    // -----------------------------
    public void BtnOiZuki() => StartCoroutine(ExecutarGolpe(parametroOiZuki, danoOiZuki));
    public void BtnMaeGeri() => StartCoroutine(ExecutarGolpe(parametroMaeGeri, danoMaeGeri));
    public void BtnMawashiGeri() => StartCoroutine(ExecutarGolpe(parametroMawashiGeri, danoMawashiGeri));
    public void BtnGedanBarai() => StartCoroutine(ExecutarDefesa(parametroGedanBarai));
    public void BtnJodanUke() => StartCoroutine(ExecutarDefesa(parametroJodanUke));
}
