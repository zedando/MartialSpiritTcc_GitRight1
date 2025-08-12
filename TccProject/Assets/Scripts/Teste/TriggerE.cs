using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerE : MonoBehaviour
{
    [Header("Objeto ativado ou desativado")]
    public GameObject objetoParaAtivar; // Arraste aqui no Inspector
    public GameObject botao;

    [Header("Tag do Player")]
    public string tagDoPlayer = "Player";

    private void Start()
    {
        // Garante que o objeto começa desativado
        if (objetoParaAtivar != null)
            objetoParaAtivar.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagDoPlayer))
        {
            if (objetoParaAtivar != null)
                objetoParaAtivar.SetActive(true);
                botao.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagDoPlayer))
        {
            if (objetoParaAtivar != null)
                objetoParaAtivar.SetActive(false);
               botao.SetActive(false);
        }
    }
}
