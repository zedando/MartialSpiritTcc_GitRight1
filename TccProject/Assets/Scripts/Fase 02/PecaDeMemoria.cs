using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;   // FMOD

public class PecaDeMemoria : MonoBehaviour
{
    [Header("Alvo da Peça")]
    public RectTransform alvoCorreto;
    public float tolerancia = 40f;

    [Header("Som (FMOD)")]
    [Tooltip("Evento de som de papel colando (ex: event:/UI/PapelColando)")]
    public EventReference somPapelColando;

    private RectTransform rectTransform;
    private Canvas canvas;

    private PlayerInput controls;
    private bool segurando = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        controls = new PlayerInput();
        controls.Player.Click.performed += ctx => DetectarClique();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        if (segurando)
        {
            Vector2 posTela = controls.Player.PointerPosition.ReadValue<Vector2>();

            // Para Canvas em Screen Space - Overlay, usamos null como câmera
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                posTela,
                null,
                out var local
            );

            rectTransform.anchoredPosition = local;
        }
    }

    void DetectarClique()
    {
        Vector2 pointerPos = controls.Player.PointerPosition.ReadValue<Vector2>();

        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pointerPos, null))
        {
            segurando = !segurando;

            if (!segurando)
                VerificarPosicao();
        }
    }

    void VerificarPosicao()
    {
        float distancia = Vector2.Distance(rectTransform.anchoredPosition, alvoCorreto.anchoredPosition);

        if (distancia < tolerancia)
        {
            // Snap na posição certa
            rectTransform.anchoredPosition = alvoCorreto.anchoredPosition;
            segurando = false;
            enabled = false;

            // 🔊 TOCA O SOM DE PAPEL COLANDO
            RuntimeManager.PlayOneShot(somPapelColando, transform.position);

            // Notifica o minigame que a peça foi colocada
            MinigameMemoria.Instance.PecaCorreta();
        }
    }
}
