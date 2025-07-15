using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectDialog : MonoBehaviour
{
    // Start is called before the first frame update
    public Button FirstButton, SecondButton, ThirdButton;
    public GameObject Respost1;
    public GameObject DestroyOld;
    void Start()
    {
        FirstButton.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void TaskOnClick()
    {
        Destroy(DestroyOld);
        Respost1.SetActive(true);
        Debug.Log("You have clicked the button!");
    }

}
