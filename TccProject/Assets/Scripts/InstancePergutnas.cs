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
        SceneManager.LoadScene(13);
    }

    private IEnumerator AnimaçãoSenseiChute()
    {

        Perguntas.SetActive(false);
        animator.SetBool("MaeGeri", true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Interior 3");
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
 public void cutscen33()
    {
SceneManager.LoadScene("ct-fase3");    }

public void cutscene5() 
    {
         SceneManager.LoadScene("ct-fase5");
    }
     
    public void Load()
    {
        SceneManager.LoadScene(3);
    }
     public void Loadfase3()
    {
        SceneManager.LoadScene(4);
    }

    public void DojoMiniGame()
    {
        SceneManager.LoadScene(5);
    }
    public void MapaFase3()
    {
        SceneManager.LoadScene(6);
    }
    public void quarto()
    {
        SceneManager.LoadScene(7);
    }
    public void Fase4() 
    {
         SceneManager.LoadScene(9);
    }
    public void Fase5() 
    {
         SceneManager.LoadScene("Interior 4");
    }
     
     
     
}
