using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObject/Pergunta")]
public class Instance : ScriptableObject
{
    [SerializeField] private InstancePergutnas perguntasSelect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        perguntasSelect.StartQuestion();
        Debug.Log("FALA");
    }

    
}
