using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;


public class InstancePergutnas : MonoBehaviour
{
    public GameObject Perguntas;
    public GameObject AvisoNãoAbrir;
    public float tempoParaSumir = 2f;
    public GameObject AnimaçãoSensei;
    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartQuestion()
    {
        Perguntas.SetActive(true);
        Debug.Log("ola");
    }
    public void ChamarAnimação()
    {
        StartCoroutine(AnimaçãoSenseiChute());
    }
    public void StopQuestion()
    {
        Perguntas.SetActive(false);
        Debug.Log("ola");
    }

    private IEnumerator AnimaçãoSenseiChute()
    {

        Perguntas.SetActive(false);
        animator.SetBool("MaeGeri", true);
        yield return new WaitForSeconds(2f);

        animator.SetBool("MaeGeri", false);

    }

    public void Aviso()
    {
        StartCoroutine(MostrarAviso());
    }

    private IEnumerator MostrarAviso()
    {
        AvisoNãoAbrir.SetActive(true);
        yield return new WaitForSeconds(tempoParaSumir);
        AvisoNãoAbrir.SetActive(false);
    }

    public void Load()
    {
        SceneManager.LoadScene(3);
    }
    public void DojoMiniGame()
    {
        SceneManager.LoadScene(4);
    }
    public void MapaFase3()
    {
        SceneManager.LoadScene(5);
    }
     
     
     
}
