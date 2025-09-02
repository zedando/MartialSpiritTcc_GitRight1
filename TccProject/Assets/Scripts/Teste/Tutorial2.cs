using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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

    [Header("Player")]
    public Transform playerHead;

    private PlayerInput controls;
    private bool movedUp, movedLeft, movedDown, movedRight;
    private bool pressedSpace = false;
    private int step = 0;
    private bool canSkipDialog = false;
    private bool isAndroid;
    private bool isTyping = false;  

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
        ShowStep();
    }

    void Update()
    {
        Vector3 headPos = playerHead.position + Vector3.up * 3;
        if (wasdImage.activeSelf) wasdImage.transform.position = Camera.main.WorldToScreenPoint(headPos);
        if (spacebarImage.activeSelf) spacebarImage.transform.position = Camera.main.WorldToScreenPoint(headPos);
        if (analogImage.activeSelf) analogImage.transform.position = Camera.main.WorldToScreenPoint(headPos);
        if (tapImage.activeSelf) tapImage.transform.position = Camera.main.WorldToScreenPoint(headPos);

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
        }
    }

    public void OnDialogClick()
    {
        if (isAndroid && step == 2)
            SkipOrNext();
    }

    void ShowStep()
    {
        HideAllImages();
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        switch (step)
        {
            case 0:
                typingCoroutine = StartCoroutine(TypeText("A jornada come�a agora... Vamos aprender os primeiros passos!", 0.055f, autoNextDelay: 3.0f));
                break;
            case 1:
                if (!isAndroid)
                {
                    wasdImage.SetActive(true);
                    typingCoroutine = StartCoroutine(TypeText("Use as teclas W, A, S e D para caminhar pelo mundo.", 0.055f));
                }
                else
                {
                    analogImage.SetActive(true);
                    typingCoroutine = StartCoroutine(TypeText("Mova o anal�gico para todos os lados para explorar o mundo.", 0.055f));
                }
                break;
            case 2:
                canSkipDialog = true;
                if (!isAndroid)
                {
                    spacebarImage.SetActive(true);
                    typingCoroutine = StartCoroutine(TypeText("Aperte Espa�o para avan�ar os di�logos. Assim voc� segue sua jornada no seu ritmo.", 0.055f));
                }
                else
                {
                    tapImage.SetActive(true);
                    typingCoroutine = StartCoroutine(TypeText("Toque na caixa de di�logo para avan�ar os di�logos e seguir sua aventura no seu ritmo.", 0.055f));
                }
                break;
            case 3:
                typingCoroutine = StartCoroutine(TypeText("Excelente, guerreiro! Voc� est� pronto para enfrentar o que vier. Boa sorte!", 0.055f, autoNextDelay: 4.5f, endTutorial: true));
                break;
        }
    }

    void SkipOrNext()
    {
        if (isTyping)
        {
            // Mostra o texto inteiro de imediato
            StopCoroutine(typingCoroutine);
            dialogText.maxVisibleCharacters = int.MaxValue;
            isTyping = false;
        }
        else
        {
            // Passa para o pr�ximo passo
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
    }

    void HideAllImages()
    {
        wasdImage.SetActive(false);
        spacebarImage.SetActive(false);
        analogImage.SetActive(false);
        tapImage.SetActive(false);
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
