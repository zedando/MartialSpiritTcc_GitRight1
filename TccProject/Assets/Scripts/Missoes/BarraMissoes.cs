using UnityEngine;
using TMPro;

public class BarraMissoes : MonoBehaviour
{
    [Header("Configuração da Missão")]
    public MissaoData missaoAtual;
    public TMP_Text tituloText;
    public TMP_Text descricaoText;

    [Header("Configuração da UI")]
    public RectTransform painel; // painel da barra que expande
    public float alturaFechada = 50f;
    public float alturaAberta = 200f;
    public float velocidadeAnimacao = 8f;

    private bool estaAberta = false;
    private float alturaAlvo;

    void Start()
    {
        AtualizarUI();
        alturaAlvo = alturaFechada;
        SetAlturaInstant(alturaFechada);

        if (descricaoText != null)
            descricaoText.gameObject.SetActive(false); // começa desligado
    }

    void Update()
    {
        Vector2 sd = painel.sizeDelta;
        sd.y = Mathf.Lerp(sd.y, alturaAlvo, Time.unscaledDeltaTime * velocidadeAnimacao);
        painel.sizeDelta = sd;
    }

    public void OnMostrarMissao()
    {
        Debug.Log("OnMostrarMissao chamado!");
        estaAberta = !estaAberta;
        alturaAlvo = estaAberta ? alturaAberta : alturaFechada;

        if (descricaoText != null)
            descricaoText.gameObject.SetActive(estaAberta); // liga/desliga descrição
    }

    void AtualizarUI()
    {
        if (missaoAtual != null)
        {
            if (tituloText != null)
                tituloText.text = missaoAtual.titulo;
            if (descricaoText != null)
                descricaoText.text = missaoAtual.descricao;
        }
    }

    void SetAlturaInstant(float altura)
    {
        Vector2 sd = painel.sizeDelta;
        sd.y = altura;
        painel.sizeDelta = sd;
    }
}
