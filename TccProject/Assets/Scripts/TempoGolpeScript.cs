using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManager;
using System.Collections;
using TMPro;
using FMODUnity;

public class TempoGolpeScript : MonoBehaviour
{

     public GameObject SegundaParteasdad;
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
    public string clickEvent = "event:/ui/click";
    public string ola;
    public GameObject SegundaParte1;

    private string teclaAtual;
    private bool minigameAtivo = false;
    
    private float tempoRestante;
    private int teclasCorretas = 0;
    private string[] teclas = { "J", "E", "Q" };

    public Animator animator;
    public Animator Senseianimator;

    [Header("FMOD Events")]
    public string clickEvent = "event:/ui/click"; 
    public string doorEvent = "event:/ambiente/porta";

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

        // ===============================
        //   🔊 QUALQUER TECLA → SOM CLICK
        // ===============================
        if (Input.anyKeyDown)
        {
            // tocar som de clique sempre que apertar algo
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
                // tecla errada → som de erro
                RuntimeManager.PlayOneShot(errorSound);
            }
        }

        if (tempoRestante < 0)
        {
            // tempo acabou → som de erro
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

    // ===============================
    //     ANDROID CLICK → SOM TBM
    // ===============================
    public void OnclickAndroid()
    {
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
