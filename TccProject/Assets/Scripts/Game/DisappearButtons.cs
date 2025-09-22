using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearButtons : MonoBehaviour
{
    [SerializeField] private GameObject androidButton;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            androidButton.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
