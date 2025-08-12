using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class TempoGolpeScript : MonoBehaviour
{
    public Slider slider;
    public float tempoTotal = 2f; // tempo por tecla

    public string proximaFase;

    // Imagens das teclas no Canvas
    public GameObject teclaJ;
    public GameObject teclaE;
    public GameObject teclaQ;
    public GameObject SegundaParte;
    public GameObject PrimeiraParte;
    public GameObject TerceiraParte;
    public float Venceu = 0;



    private string teclaAtual;
    private bool minigameAtivo = false;
    private float tempoRestante;
    private int teclasCorretas = 0;
    private string[] teclas = { "J", "E", "Q" };
    public Animator animator;
    public Animator Senseianimator;

    public void iniciou()
    {
        if (!minigameAtivo)
        {
            IniciarMinigame();
        }
    }

    void IniciarMinigame()
    {
        minigameAtivo = true;
        teclasCorretas = 0;
        slider.gameObject.SetActive(true);

        // Esconde todas as imagens no início
        teclaJ.SetActive(false);
        teclaE.SetActive(false);
        teclaQ.SetActive(false);

        NovaTecla();
    }

    void NovaTecla()
    {
        tempoRestante = tempoTotal;
        slider.value = 0;

        // Garante que a nova tecla não seja igual à última
        string novaTecla;
        do
        {
            novaTecla = teclas[Random.Range(0, teclas.Length)];
        } while (novaTecla == teclaAtual);

        teclaAtual = novaTecla;

        // Esconde todas as teclas e mostra a sorteada
        teclaJ.SetActive(false);
        teclaE.SetActive(false);
        teclaQ.SetActive(false);

        if (teclaAtual == "J") teclaJ.SetActive(true);
        else if (teclaAtual == "E") teclaE.SetActive(true);
        else if (teclaAtual == "Q") teclaQ.SetActive(true);
    }

    void Update()
    {
        if (!minigameAtivo) return;

        tempoRestante -= Time.deltaTime;
        slider.value = 1 - (tempoRestante / tempoTotal);

        if (Input.anyKeyDown)
        {


            if (Input.GetKeyDown(teclaAtual.ToLower()))
            {

                teclasCorretas++;
                if (teclasCorretas > 3)
                {
                    StartCoroutine(PassarDeFase());
                }
                else
                {
                    NovaTecla();
                }
            }

        }

        if (tempoRestante < 0)
        {
            ReiniciarFase();
        }
    }

    IEnumerator PassarDeFase()
    {
        minigameAtivo = false;
        slider.gameObject.SetActive(false);

        // Esconde todas as teclas
        teclaJ.SetActive(false);
        teclaE.SetActive(false);
        teclaQ.SetActive(false);

        animator.SetBool("Oi-zuki", true);
        Senseianimator.SetBool("Oi-zuki", true);

        Venceu++;
        if (Venceu == 1)
        {
            SegundaParte.SetActive(true);
            PrimeiraParte.SetActive(false);
        }
        if (Venceu == 2)
        {
            TerceiraParte.SetActive(true);
            SegundaParte.SetActive(false);
        }




        yield return new WaitForSeconds(2f);

        animator.SetBool("Oi-zuki", false);
        Senseianimator.SetBool("Oi-zuki", false);


    }

    void ReiniciarFase()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnclickAndroid()
    { 
        teclasCorretas++;
                if (teclasCorretas > 3)
                {
                    StartCoroutine(PassarDeFase());
                }
                else
                {
                    NovaTecla();
                }
    }
}