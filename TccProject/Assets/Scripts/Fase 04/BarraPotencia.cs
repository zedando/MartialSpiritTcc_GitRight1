using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarraPotencia : MonoBehaviour
{
    [Header("Configuração da Barra")]
    public Slider barra;
    public float velocidade = 2f;
    private bool subindo = true;

    [Header("Tecla de Ação")]
    public KeyCode teclaAcao = KeyCode.Space;

    [Header("Configuração do Animator")]
    public Animator animator;
    public string parametroMaeGeri = "MaeGeri"; // Boolean no Animator

    [Header("Zonas de Acerto")]
    [Range(0f, 1f)] public float zonaPerfeitaCentro = 0.50f; // Meio da barra
    public float toleranciaCentro = 0.03f; // ± 3% = Perfeito
    public float toleranciaBoa = 0.15f;   // ± 15% = Mais ou menos

    private bool executandoGolpe = false;

    void Update()
    {
        // Oscilação da barra
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

        // Input - só executa se não estiver golpeando
        if (!executandoGolpe && Input.GetKeyDown(teclaAcao))
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
            StartCoroutine(ExecutarGolpe(1.2f, false)); // rápido e fluido
        }
        else if (distanciaDoCentro <= toleranciaBoa)
        {
            Debug.Log("Bom timing! Mas ainda pode melhorar.");
            StartCoroutine(ExecutarGolpe(0.8f, false)); // mais lento mas completo
        }
        else
        {
            Debug.Log("Muito fraco. Concentre-se mais.");
            StartCoroutine(ExecutarGolpe(0.5f, true)); // lento e cortado
        }
    }

    IEnumerator ExecutarGolpe(float speed, bool cortarNoMeio)
    {
        executandoGolpe = true;
        animator.speed = speed;
        animator.SetBool(parametroMaeGeri, true);

        if (cortarNoMeio)
        {
            yield return new WaitForSeconds(0.4f / speed); // metade da animação
        }
        else
        {
            yield return new WaitForSeconds(0.9f / speed); // tempo quase completo
        }

        animator.SetBool(parametroMaeGeri, false);
        animator.speed = 1f;

        yield return new WaitForSeconds(0.05f);
        executandoGolpe = false;
    }
}
