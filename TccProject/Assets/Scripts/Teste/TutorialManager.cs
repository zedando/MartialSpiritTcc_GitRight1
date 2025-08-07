using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI dialogText;
    public GameObject wasdImage;
    public GameObject spacebarImage;
    public Transform playerHead;

    private PlayerInput controls;
    private bool pressedW, pressedA, pressedS, pressedD;
    public bool pressedSpace = false;
    public bool spaces;
    public int step = 0;
    public bool canSkipDialog = false;

    void Awake()
    {
        controls = new PlayerInput();

        controls.Player.Move.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            if (input.y > 0) pressedW = true;
            if (input.x < 0) pressedA = true;
            if (input.y < 0) pressedS = true;
            if (input.x > 0) pressedD = true;
        };
    }
    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        wasdImage.SetActive(false);
        spacebarImage.SetActive(false);
        step = 0;
        ShowStep();
    }

    void Update()
    {
        if (step == 2 && canSkipDialog == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                pressedSpace = true;
                spaces = true;
            }     
        }
        Vector3 headPos = playerHead.position + Vector3.up * 3;
        wasdImage.transform.position = Camera.main.WorldToScreenPoint(headPos);
        spacebarImage.transform.position = Camera.main.WorldToScreenPoint(headPos);

        switch (step)
        {
            case 1:
                if (pressedW && pressedA && pressedS && pressedD)
                {
                    NextStep();
                }
                break;
            case 2:
                canSkipDialog = true;
                if (pressedSpace)
                {
                    NextStep();
                }
                break;
        }
    }

    void ShowStep()
    {
        switch (step)
        {
            case 0:
                dialogText.text = "A jornada começa agora... Vamos aprender os primeiros passos!";
                Invoke(nameof(NextStep), 2f);
                break;
            case 1:
                dialogText.text = "Use as teclas <b>W</b>, <b>A</b>, <b>S</b> e <b>D</b> para caminhar pelo mundo.";
                wasdImage.SetActive(true);
                break;
            case 2:
                dialogText.text = "Aperte <b>Espaço</b> para avançar os diálogos. Assim você segue sua jornada no seu ritmo.";
                wasdImage.SetActive(false);
                spacebarImage.SetActive(true);
                break;
            case 3:
                dialogText.text = "Excelente, guerreiro! Você está pronto para enfrentar o que vier. Boa sorte!";
                spacebarImage.SetActive(false);
                Invoke(nameof(EndTutorial), 3f);
                break;
        }
    }

    void NextStep()
    {
        step++;
        ShowStep();
    }

    void EndTutorial()
    {
        dialogText.text = "";
    }
}
