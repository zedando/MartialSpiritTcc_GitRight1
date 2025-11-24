using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;


public class SelectDialog : MonoBehaviour
{
    // Start is called before the first frame update
    public Button FirstButton, SecondButton, ThirdButton;
    public GameObject Respost1;
    public GameObject Respost2;
    public GameObject Respost3;
    public GameObject DestroyOld;
    public GameObject DestroyCaixa;
     public GameObject DestroyOld1;
    void Start()
    {
        FirstButton.onClick.AddListener(TaskOnClick);
        SecondButton.onClick.AddListener(TaskOnClick1);
        ThirdButton.onClick.AddListener(TaskOnClick2);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TaskOnClick()
    {
        Destroy(DestroyOld);
        Destroy(DestroyOld1);
        DestroyCaixa.SetActive(false);
        Respost1.SetActive(true);
        Debug.Log("You have clicked the button!");



        
    }
    void TaskOnClick1()
    {
        Destroy(DestroyOld);
        Destroy(DestroyOld1);
        DestroyCaixa.SetActive(false);
        Respost2.SetActive(true);
        Debug.Log("You have clicked the button!");

        
    }
     public void TaskOnClick2()
    {
        Destroy(DestroyOld);
        Destroy(DestroyOld1);
        DestroyCaixa.SetActive(false);
        Respost3.SetActive(true);
        Debug.Log("You have clicked the button!");

        
    }

   

}
