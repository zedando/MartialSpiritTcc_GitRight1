using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    
    private void Update()
    {

    }

    public void Load()
    {
        SceneManager.LoadScene(2);
    }


}
