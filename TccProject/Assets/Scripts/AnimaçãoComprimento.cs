using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimaçãoComprimento : MonoBehaviour
{

   [Header("Animator do Sensei")]
    public Animator senseiAnimator;

    [Header("Configuração")]
    public bool destruirDepois = true; // se quiser controlar futuramente

    private bool jaFez = false; // impede ativar mais de uma vez

    private void OnTriggerEnter(Collider other)
    {
        if (jaFez) return; // já fez uma vez, ignora

        if (other.CompareTag("Player"))
        {
            jaFez = true;

            // Faz o sensei sair do idle (cumprimento)
            senseiAnimator.SetBool("idle", false);

            // Destrói APENAS o trigger, não o sensei
            if (destruirDepois)
                Destroy(gameObject);
        }
    }
    
    
}


