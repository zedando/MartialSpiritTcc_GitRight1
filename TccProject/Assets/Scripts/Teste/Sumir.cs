using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sumir : MonoBehaviour
{
    [Header("Objeto que aparece (botão, painel, UI etc.)")]
    public GameObject botaoUI;

    private bool playerDentro = false;

    private void Start()
    {
        if (botaoUI != null)
            botaoUI.SetActive(false); // começa oculto
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDentro = true;

            if (botaoUI != null)
                botaoUI.SetActive(true); // aparece ao entrar
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDentro = false;

            if (botaoUI != null)
                botaoUI.SetActive(false); // some ao sair
        }
    }

    // Método chamado pelo botão (opcional)
    public void EsconderBotao()
    {
        if (botaoUI != null)
            botaoUI.SetActive(false); // some ao clicar
    }
}
