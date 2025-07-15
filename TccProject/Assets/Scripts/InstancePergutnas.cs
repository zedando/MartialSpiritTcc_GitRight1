using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstancePergutnas : MonoBehaviour
{
     public GameObject Perguntas;
     public GameObject AvisoNãoAbrir;
     
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
     public void Aviso()
     {
          AvisoNãoAbrir.SetActive(true);
     }
     
     
}
