using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using FMODUnity;
using UnityEngine.SceneManagement;

public class TempoGolpeScript : MonoBehaviour
{
    public Slider slider;
    public float tempoTotal = 2f;

    public string proximaFase;

    public GameObject teclaJ;
    public GameObject teclaE;
    public GameObject teclaQ;
    public GameObject SegundaParte;
    public GameObject PrimeiraParte;
    public GameObject TerceiraParte;
    public GameObject MapaParte;
    public float Venceu = 0;

    [Header("FMOD Events")]
    public string clickSound = "event:/ui/click";
    public string errorSound = "event:/ui/error";

    private string teclaAtual;
    private bool minigameAtivo = false;

    private float tempoRestante;
    private int teclasCorretas = 0;
    private string[] teclas = { "J", "E", "Q" };

    public Animator animator;
    public Animator Senseianimator;

    private bool androidClick = false; // ← impede som duplo

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

        teclaJ.SetActive(false);
        teclaE.SetActive(false);
        teclaQ.SetActive(false);

        NovaTecla();
    }

    void NovaTecla()
    {
        tempoRestante = tempoTotal;
        slider.value = 0;

        string novaTecla;
        do
        {
            novaTecla = teclas[Random.Range(0, teclas.Length)];
        } while (novaTecla == teclaAtual);

        teclaAtual = novaTecla;

        teclaJ.SetActive(false);
        teclaE.SetActive(false);
        teclaQ.SetActive(false);

        if (teclaAtual == "J") teclaJ.SetActive(true);
        if (teclaAtual == "E") teclaE.SetActive(true);
        if (teclaAtual == "Q") teclaQ.SetActive(true);
    }

    void Update()
    {
        if (!minigameAtivo) return;

        tempoRestante -= Time.deltaTime;
        slider.value = 1 - (tempoRestante / tempoTotal);

        // PC → tecla pressionada
        if (Input.anyKeyDown && !androidClick)
        {
            RuntimeManager.PlayOneShot(clickSound);

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
            else
            {
                RuntimeManager.PlayOneShot(errorSound);
            }
        }

        // Reseta flag do Android
        androidClick = false;

        if (tempoRestante < 0)
        {
            RuntimeManager.PlayOneShot(errorSound);
            ReiniciarFase();
        }
    }

    IEnumerator PassarDeFase()
    {
        minigameAtivo = false;
        slider.gameObject.SetActive(false);

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
        if (Venceu == 3)
        {
            TerceiraParte.SetActive(false);
            MapaParte.SetActive(true);
        }

        yield return new WaitForSeconds(2f);

        animator.SetBool("Oi-zuki", false);
        Senseianimator.SetBool("Oi-zuki", false);
    }

    void ReiniciarFase()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ANDROID CLICK
    public void OnclickAndroid()
    {
        androidClick = true; // ← evita som duplicado
        RuntimeManager.PlayOneShot(clickSound);

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
