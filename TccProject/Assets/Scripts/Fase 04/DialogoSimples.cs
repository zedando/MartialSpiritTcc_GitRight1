using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogoSimples : MonoBehaviour
{
    [Header("Componentes UI")]
    public CanvasGroup painelDialogo;
    public Image imgPersonagem;
    public TextMeshProUGUI txtNome;
    public TextMeshProUGUI txtFala;

    [Header("Configurações")]
    public float tempoPorLetra = 0.03f;

    private Coroutine rotinaEscrevendo;

    void Start()
    {
        painelDialogo.alpha = 0f;
        painelDialogo.interactable = false;
        painelDialogo.blocksRaycasts = false;
    }

    public void MostrarDialogo(string nome, Sprite foto, string fala)
    {
        // Se estiver escrevendo algo, para a rotina antiga
        if (rotinaEscrevendo != null)
        {
            StopCoroutine(rotinaEscrevendo);
        }

        painelDialogo.alpha = 1f;
        painelDialogo.interactable = true;
        painelDialogo.blocksRaycasts = true;

        imgPersonagem.sprite = foto;
        txtNome.text = nome;

        rotinaEscrevendo = StartCoroutine(EscreverTexto(fala));
    }

    public void FecharDialogo()
    {
        if (rotinaEscrevendo != null)
        {
            StopCoroutine(rotinaEscrevendo);
            rotinaEscrevendo = null;
        }
        painelDialogo.alpha = 0f;
        painelDialogo.interactable = false;
        painelDialogo.blocksRaycasts = false;
    }

    IEnumerator EscreverTexto(string fala)
    {
        txtFala.text = "";
        foreach (char letra in fala)
        {
            txtFala.text += letra;
            yield return new WaitForSeconds(tempoPorLetra);
        }
        rotinaEscrevendo = null;
    }
}
