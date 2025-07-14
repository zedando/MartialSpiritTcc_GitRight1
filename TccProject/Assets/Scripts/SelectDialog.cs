using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectDialog : MonoBehaviour
{
    // Start is called before the first frame update
    public Button FirstButton, SecondButton, ThirdButton;
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
        //Output this to console when Button1 or Button3 is clicked
        Debug.Log("You have clicked the button!");
    }

}
