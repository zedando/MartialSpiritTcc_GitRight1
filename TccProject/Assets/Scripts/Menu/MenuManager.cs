using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]private GameObject panelPrincipal;
    [SerializeField]private GameObject panelOpcoes;
    [SerializeField]private GameObject panelSair;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        SceneManager.LoadScene("Level1");
    }
    public void OpenOptions()
    {
        panelPrincipal.SetActive(false);
        panelOpcoes.SetActive(true);

    }
    public void CloseOptions()
    {
        panelPrincipal.SetActive(true);
        panelOpcoes.SetActive(false);
    }
    public void Quit()
    {
        panelSair.SetActive(true);
       
    }

    public void YesQuit()
    {
        Debug.Log("Saiu do jogo"); 
        Application.Quit();
    }

    public void NoQuit()
    {
        panelSair.SetActive(false);
    }

}
