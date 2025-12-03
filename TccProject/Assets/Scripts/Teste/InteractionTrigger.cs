using UnityEngine;
using UnityEngine.UI;

public class InteractionTrigger : MonoBehaviour
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
                botaoUI.SetActive(true); // aparece apenas ao entrar
        }
    }

    // Não escondemos ao sair
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDentro = false;
        }
    }

    // Método chamado pelo botão
    public void EsconderBotao()
    {
        if (botaoUI != null)
            botaoUI.SetActive(false); // some apenas ao clicar
    }
}
