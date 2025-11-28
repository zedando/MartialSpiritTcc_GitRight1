using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pulafase : MonoBehaviour
{
    // Start is called before the first frame updatepublic string nomeDaCena;
public string nomeDaCena;
    void Update()
    {
        // Se apertar a tecla B
        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene(nomeDaCena);
        }
    }
}
