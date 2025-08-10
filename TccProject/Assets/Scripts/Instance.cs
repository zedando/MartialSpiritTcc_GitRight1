using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Pergunta")]
public class Instance : ScriptableObject
{
    public GameObject Perguntas;
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
}
