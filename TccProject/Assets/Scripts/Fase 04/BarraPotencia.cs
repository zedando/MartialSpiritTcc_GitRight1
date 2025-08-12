using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarraPotencia : MonoBehaviour
{
    [Header("Configuracão da Barra")]
    public Slider barra;
    public float velocidade = 2f;
    private bool subindo = true;

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

    private bool executandoGolpe = false;

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

        // Teclado (PC)
        if (!executandoGolpe && Input.GetKeyDown(teclaAcao))
        {
            VerificarAcerto();
        }
    }

    // Método público para usar no botão do celular
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
            alvo.Balancar(20f); // balanco forte
            StartCoroutine(ExecutarGolpe(1.2f, false));
        }
        else if (distanciaDoCentro <= toleranciaBoa)
        {
            Debug.Log("Bom timing! Mas ainda pode melhorar.");
            alvo.Balancar(12f); // balanco médio
            StartCoroutine(ExecutarGolpe(0.8f, false));
        }
        else
        {
            Debug.Log("Muito fraco. Concentre-se mais.");
            alvo.Balancar(5f); // balanco fraco
            StartCoroutine(ExecutarGolpe(0.5f, true));
        }
    }

    IEnumerator ExecutarGolpe(float speed, bool cortarNoMeio)
    {
        executandoGolpe = true;
        animator.speed = speed;
        animator.SetBool(parametroMaeGeri, true);

        if (cortarNoMeio)
        {
            yield return new WaitForSeconds(0.4f / speed);
        }
        else
        {
            yield return new WaitForSeconds(0.9f / speed);
        }

        animator.SetBool(parametroMaeGeri, false);
        animator.speed = 1f;

        yield return new WaitForSeconds(0.05f);
        executandoGolpe = false; // libera próximo golpe
    }
}
