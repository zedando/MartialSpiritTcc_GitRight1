using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidButton : MonoBehaviour
{
    private bool isAndroid;
    public GameObject joyStick;

    void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        isAndroid = true;
#else
        isAndroid = false;
#endif

   
     
    }
    // Start is called before the first frame update
    void Start()
    {
        if (isAndroid == true)
        {
            joyStick.SetActive(true);
        }
        else
        {
            joyStick.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
