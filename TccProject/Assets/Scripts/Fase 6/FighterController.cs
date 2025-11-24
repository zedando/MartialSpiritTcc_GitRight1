using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public float danoGedanBarai = 0f;
    public float danoJodanUke = 0f;

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

    [Header("Dialogo")]
    public DialogoSimples dialogoSimples;
    public Sprite spriteSensei;
    public Sprite spriteJuiz;

    [Header("Fade")]
    public Image fadeImage;

    private float vidaAtual;
    private float estaminaAtual;
    private bool golpeExecutando = false;
    private bool defendendo = false;
    private bool invulneravel = false;
    private bool lutaIniciada = false;

    void Start()
    {
        vidaAtual = vidaMax;
        estaminaAtual = estaminaMax;
        AtualizarBarras();
        StartCoroutine(DialogoInicial());
    }

    void Update()
    {
        if (!lutaIniciada) return;
        if (golpeExecutando) return;

        // Ataques só se tiver estamina suficiente
        if (estaminaAtual >= 20f)
        {
            if (Input.GetKeyDown(teclaOiZuki)) StartCoroutine(ExecutarGolpe(parametroOiZuki, danoOiZuki));
            if (Input.GetKeyDown(teclaMaeGeri)) StartCoroutine(ExecutarGolpe(parametroMaeGeri, danoMaeGeri));
            if (Input.GetKeyDown(teclaMawashiGeri)) StartCoroutine(ExecutarGolpe(parametroMawashiGeri, danoMawashiGeri));
        }

        if (Input.GetKeyDown(teclaGedanBarai) && estaminaAtual >= 10f) StartCoroutine(ExecutarDefesa(parametroGedanBarai));
        if (Input.GetKeyDown(teclaJodanUke) && estaminaAtual >= 10f) StartCoroutine(ExecutarDefesa(parametroJodanUke));

        // Recupera estamina quando não atacando
        if (!golpeExecutando && !defendendo)
        {
            estaminaAtual += 15f * Time.deltaTime;
            if (estaminaAtual > estaminaMax) estaminaAtual = estaminaMax;
            AtualizarBarras();
        }
    }

    IEnumerator DialogoInicial()
    {
        if (dialogoSimples != null)
        {
            dialogoSimples.MostrarDialogo("Sensei", spriteSensei, "Você treinou para este momento. Agora, não é sobre vencer ou perder – é sobre mostrar quem você se tornou.");
            yield return new WaitForSeconds(9f);
            dialogoSimples.FecharDialogo();

            yield return new WaitForSeconds(0.5f);
            dialogoSimples.MostrarDialogo("Juiz", spriteJuiz, "Comece!");
            yield return new WaitForSeconds(3f);
            dialogoSimples.FecharDialogo();
        }

        lutaIniciada = true;
        if (inimigo != null) inimigo.lutaIniciada = true;
    }

    IEnumerator ExecutarGolpe(string parametro, float dano)
    {
        golpeExecutando = true;
        estaminaAtual -= 20f;
        AtualizarBarras();

        if (efeitoGolpePrefab != null)
            Instantiate(efeitoGolpePrefab, transform.position + Vector3.right * 1f, Quaternion.identity);

        if (efeitoTrilhaGolpe != null)
        {
            GameObject trilha = Instantiate(efeitoTrilhaGolpe, transform.position + Vector3.right * 1f, Quaternion.identity);
            Destroy(trilha, 0.5f);
        }

        animator.SetBool(parametro, true);
        yield return new WaitForSeconds(0.4f);
        animator.SetBool(parametro, false);

        animator.SetBool(parametroIdle, true);
        yield return new WaitForSeconds(0.05f);
        animator.SetBool(parametroIdle, false);

        if (inimigo != null && Vector3.Distance(transform.position, inimigo.transform.position) <= 2f)
            inimigo.ReceberDano(dano, efeitoImpactoPrefab);

        golpeExecutando = false;
    }

    IEnumerator ExecutarDefesa(string parametro)
    {
        defendendo = true;
        invulneravel = true;
        estaminaAtual -= 10f;
        AtualizarBarras();

        animator.SetBool(parametroDefesa, true);
        animator.SetBool(parametro, true);

        yield return new WaitForSeconds(2f);

        animator.SetBool(parametro, false);
        animator.SetBool(parametroDefesa, false);
        animator.SetBool(parametroIdle, true);
        yield return new WaitForSeconds(0.05f);
        animator.SetBool(parametroIdle, false);

        defendendo = false;
        invulneravel = false;
    }

    public void ReceberDano(float dano, GameObject efeitoImpacto = null)
    {
        if (invulneravel) return;

        vidaAtual -= dano;
        if (vidaAtual < 0) vidaAtual = 0;
        AtualizarBarras();

        if (efeitoImpacto != null)
            Instantiate(efeitoImpacto, transform.position + Vector3.right * 0.5f, Quaternion.identity);

        StartCoroutine(ImpactShake());

        if (vidaAtual <= 0)
        {
            Debug.Log("Player derrotado!");
            StartCoroutine(DerrotaPlayer());
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

    IEnumerator DerrotaPlayer()
    {
        lutaIniciada = false;

        // Mensagem do juiz
        if (dialogoSimples != null)
        {
            dialogoSimples.MostrarDialogo("Juiz", spriteJuiz, "Haruki foi derrotado por pontos!");
            yield return new WaitForSeconds(6f);
            dialogoSimples.FecharDialogo();
        }

        // Fade out
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

        // Troca de cena
        SceneManager.LoadScene("ct-derrota");
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
