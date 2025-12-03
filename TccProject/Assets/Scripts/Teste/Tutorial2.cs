using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class Tutorial2 : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogText;
    public GameObject wasdImage;
    public GameObject spacebarImage;
    public GameObject analogImage;
    public GameObject tapImage;
    public Button tapButton;
    public GameObject dialogbar;

    [Header("UI – Teclas M / Q / E")]
    public GameObject mapKeyImage;        // sprite da tecla M
    public GameObject missionKeyImage;    // sprite da tecla Q
    public GameObject interactKeyImage;   // sprite da tecla E

    [Header("Player")]
    public Transform playerHead;

    [Header("Mini mapa / Missões / Interação")]
    public GameObject minimapPanel;
    public GameObject missionsPanel;
    public UnityEvent onInteract;

    private PlayerInput controls;

    private bool movedUp, movedLeft, movedDown, movedRight;
    private bool pressedSpace = false;

    private bool pressedM = false;
    private bool pressedQ = false;
    private bool pressedE = false;

    private int step = 0;
    private bool canSkipDialog = false;
    private bool isAndroid;
    private bool isTyping = false;
    private bool tutorialActive = false;

    private Coroutine typingCoroutine;

    void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        isAndroid = true;
#else
        isAndroid = false;
#endif

        controls = new PlayerInput();

        controls.Player.Move.performed += ctx =>
        {
            if (!tutorialActive) return;

            Vector2 input = ctx.ReadValue<Vector2>();
            if (input.y > 0) movedUp = true;
            if (input.x < 0) movedLeft = true;
            if (input.y < 0) movedDown = true;
            if (input.x > 0) movedRight = true;
        };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        HideAllImages();
        tapButton.gameObject.SetActive(isAndroid);

        step = 0;
        dialogbar.SetActive(false);
        dialogText.text = "";
    }

    void Update()
    {
        // -------- COMANDOS GERAIS (M / Q / E) --------
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (minimapPanel != null)
                minimapPanel.SetActive(!minimapPanel.activeSelf);

            pressedM = true;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (missionsPanel != null)
                missionsPanel.SetActive(!missionsPanel.activeSelf);

            pressedQ = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            onInteract?.Invoke();
            pressedE = true;
        }

        // -------- LÓGICA DO TUTORIAL --------
        if (!tutorialActive) return;

        // posiciona os ícones acima da cabeça
        Vector3 headPos = playerHead.position + Vector3.up * 3;
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(headPos);

            if (wasdImage.activeSelf) wasdImage.transform.position = screenPos;
            if (spacebarImage.activeSelf) spacebarImage.transform.position = screenPos;
            if (analogImage.activeSelf) analogImage.transform.position = screenPos;
            if (tapImage.activeSelf) tapImage.transform.position = screenPos;

            if (mapKeyImage != null && mapKeyImage.activeSelf) mapKeyImage.transform.position = screenPos;
            if (missionKeyImage != null && missionKeyImage.activeSelf) missionKeyImage.transform.position = screenPos;
            if (interactKeyImage != null && interactKeyImage.activeSelf) interactKeyImage.transform.position = screenPos;
        }

        switch (step)
        {
            case 1:
                if (movedUp && movedLeft && movedDown && movedRight)
                    SkipOrNext();
                break;

            case 2:
                if (!isAndroid && Input.GetKeyDown(KeyCode.Space))
                    SkipOrNext();
                break;

            /*case 3:
                if (pressedM)
                    SkipOrNext();
                break;

            case 4:
                if (pressedQ)
                    SkipOrNext();
                break;

            case 5:
                if (pressedE)
                    SkipOrNext();
                break;*/
        }
    }

    // Chamado pelo fade depois da intro
    public void BeginTutorial()
    {
        if (tutorialActive) return;

        tutorialActive = true;
        step = 0;
        dialogText.text = "";
        HideAllImages();
        tapButton.gameObject.SetActive(isAndroid);

        // reseta os estados para não pular passos se já tiver apertado antes
        movedUp = movedLeft = movedDown = movedRight = false;
        pressedSpace = false;
        pressedM = pressedQ = pressedE = false;

        ShowStep();
    }

    public void OnDialogClick()
    {
        if (!tutorialActive) return;

        if (isAndroid && step >= 2)
            SkipOrNext();
    }

    void ShowStep()
    {
        HideAllImages();
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        switch (step)
        {
            case 0:
                typingCoroutine = StartCoroutine(TypeText(
                    "A jornada começa agora... Vamos aprender os primeiros passos!",
                    0.055f,
                    autoNextDelay: 3f
                ));
                break;

            case 1:
                if (!isAndroid)
                    wasdImage.SetActive(true);
                else
                    analogImage.SetActive(true);

                typingCoroutine = StartCoroutine(TypeText(
                    "Use as teclas W, A, S e D para caminhar pelo mundo. Caminhe para todas as direções para avançar.",
                    0.055f
                ));
                break;

            case 2:
                if (!isAndroid)
                    spacebarImage.SetActive(true);
                else
                    tapImage.SetActive(true);

                typingCoroutine = StartCoroutine(TypeText(
                    "Aperte ESPAÇO (ou toque no diálogo) para avançar as falas. Tente agora.",
                    0.055f
                ));
                break;

            /*case 3:
                if (mapKeyImage != null)
                    mapKeyImage.SetActive(true);

                typingCoroutine = StartCoroutine(TypeText(
                    "Agora, um comando importante: aperte M para abrir o mini mapa e se orientar pela vila.",
                    0.055f
                ));
                break;

            case 4:
                if (missionKeyImage != null)
                    missionKeyImage.SetActive(true);

                typingCoroutine = StartCoroutine(TypeText(
                    "Ótimo! Agora aperte Q para abrir o painel de missões e ver o resumo dos seus objetivos.",
                    0.055f
                ));
                break;

            case 5:
                if (interactKeyImage != null)
                    interactKeyImage.SetActive(true);

                typingCoroutine = StartCoroutine(TypeText(
                    "Por fim, aperte E para interagir com personagens e objetos importantes pelo caminho. Tente agora.",
                    0.055f
                ));
                break;*/

            case 3:
                typingCoroutine = StartCoroutine(TypeText(
                    "Excelente, guerreiro. Agora você domina os controles essenciais. Boa jornada!",
                    0.055f,
                    autoNextDelay: 4f,
                    endTutorial: true
                ));
                break;
        }
    }

    void SkipOrNext()
    {
        if (!tutorialActive) return;

        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogText.maxVisibleCharacters = int.MaxValue;
            isTyping = false;
        }
        else
        {
            NextStep();
        }
    }

    void NextStep()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogText.text = "";
        step++;
        ShowStep();
    }

    void EndTutorial()
    {
        dialogText.text = "";
        HideAllImages();
        tapButton.gameObject.SetActive(false);
        dialogbar.SetActive(false);
        tutorialActive = false;
    }

    void HideAllImages()
    {
        wasdImage.SetActive(false);
        spacebarImage.SetActive(false);
        analogImage.SetActive(false);
        tapImage.SetActive(false);

        if (mapKeyImage != null) mapKeyImage.SetActive(false);
        if (missionKeyImage != null) missionKeyImage.SetActive(false);
        if (interactKeyImage != null) interactKeyImage.SetActive(false);
    }

    IEnumerator TypeText(string text, float letterDelay, float autoNextDelay = 0f, bool endTutorial = false)
    {
        dialogbar.SetActive(true);

        dialogText.text = text;
        dialogText.maxVisibleCharacters = 0;
        isTyping = true;

        for (int i = 0; i < text.Length; i++)
        {
            dialogText.maxVisibleCharacters = i + 1;
            yield return new WaitForSeconds(letterDelay);
        }

        isTyping = false;

        if (autoNextDelay > 0)
        {
            yield return new WaitForSeconds(autoNextDelay);
            if (endTutorial)
                EndTutorial();
            else
                NextStep();
        }
    }
}
