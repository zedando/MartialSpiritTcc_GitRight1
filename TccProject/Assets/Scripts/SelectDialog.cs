using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectDialog : MonoBehaviour
{
    // Start is called before the first frame update
    public Button FirstButton, SecondButton, ThirdButton;
    public GameObject Respost1;
    public GameObject Respost2;
    public GameObject Respost3;
    public GameObject DestroyOld;
    public GameObject DestroyCaixa;
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
        DestroyCaixa.SetActive(false);
        Respost1.SetActive(true);
        Debug.Log("You have clicked the button!");
    }
    void TaskOnClick1()
    {
        Destroy(DestroyOld);
        DestroyCaixa.SetActive(false);
        Respost2.SetActive(true);
        Debug.Log("You have clicked the button!");
    }
    void TaskOnClick2()
    {
        Destroy(DestroyOld);
        DestroyCaixa.SetActive(false);
        Respost3.SetActive(true);
        Debug.Log("You have clicked the button!");
    }

}
