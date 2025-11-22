using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using FMOD.Studio;

public class DialogTriggerButton : MonoBehaviour
{
    [Header("UI da Dialog")]
    public GameObject dialogPanel;
    public Image characterImage;
    public TMP_Text characterNameText;
    public TMP_Text dialogText;
    public GameObject keyEImage;
    public Button actionButton;

    [Header("Configuração do diálogo")]
    public Sprite characterSprite;
    public string characterName;

    [TextArea(3, 10)]
    public string[] dialogLines;

    public float typingSpeed = 0.05f;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool cancelTyping = false;
    private bool playerInRange = false;
    private bool dialogActive = false;

    [Header("Som")]
    public string eventoInteragirObjeto;
    public string SomDialog;

    private EventInstance somDialogInstance;

    private void Start()
    {
        dialogPanel.SetActive(false);
        keyEImage.SetActive(false);
        actionButton.gameObject.SetActive(false);

        actionButton.onClick.AddListener(OnActionButtonClicked);

        // Criar instância do som de digitação
        somDialogInstance = RuntimeManager.CreateInstance(SomDialog);
    }

    private void Update()
    {
        if (playerInRange && !dialogActive)
        {
            keyEImage.SetActive(true);
            actionButton.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                RuntimeManager.PlayOneShot(eventoInteragirObjeto, transform.position);
                StartDialog();
            }
        }
        else
        {
            keyEImage.SetActive(false);
            if (!dialogActive)
                actionButton.gameObject.SetActive(false);
        }

        if (dialogActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTyping)
                {
                    // Para som ao pular uma fala
                    somDialogInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    cancelTyping = true;
                }
                else
                {
                    NextLine();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (!dialogActive)
            {
                keyEImage.SetActive(false);
                actionButton.gameObject.SetActive(false);
            }
        }
    }

    private void OnActionButtonClicked()
    {
        if (!dialogActive && playerInRange)
        {
            StartDialog();
        }
        else if (dialogActive)
        {
            if (isTyping)
            {
                somDialogInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                cancelTyping = true;
            }
            else
                NextLine();
        }
    }

    private void StartDialog()
    {
        dialogActive = true;
        dialogPanel.SetActive(true);
        keyEImage.SetActive(false);
        actionButton.gameObject.SetActive(true);

        characterImage.sprite = characterSprite;
        characterNameText.text = characterName;

        currentLine = 0;
        StartCoroutine(TypeDialog(dialogLines[currentLine]));
    }

    IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";

        // Inicia som ao começar a escrever
        somDialogInstance.start();

        foreach (char letter in line.ToCharArray())
        {
            if (cancelTyping)
            {
                dialogText.text = line;
                break;
            }

            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Para o som após terminar de escrever
        somDialogInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        isTyping = false;
        cancelTyping = false;
    }

    private void NextLine()
    {
        if (currentLine < dialogLines.Length - 1)
        {
            currentLine++;
            StartCoroutine(TypeDialog(dialogLines[currentLine]));
        }
        else
        {
            EndDialog();
        }
    }

    private void EndDialog()
    {
        dialogActive = false;
        dialogPanel.SetActive(false);
        actionButton.gameObject.SetActive(false);

        
        somDialogInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        Destroy(gameObject);
    }
}
