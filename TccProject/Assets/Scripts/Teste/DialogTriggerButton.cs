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

    private EventInstance dialogSound; // <-- SOM CONTROLADO

    private void Start()
    {
        dialogPanel.SetActive(false);
        keyEImage.SetActive(false);
        actionButton.gameObject.SetActive(false);

        actionButton.onClick.AddListener(OnActionButtonClicked);
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
                StopDialogSound(); // <-- PARA O SOM AO PULAR

                if (isTyping)
                {
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
            StopDialogSound(); // <-- PARA O SOM AO PULAR

            if (isTyping)
                cancelTyping = true;
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

        PlayDialogSound(); // <-- SOM COMEÇA

        StartCoroutine(TypeDialog(dialogLines[currentLine]));
    }

    IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";

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

        isTyping = false;
        cancelTyping = false;
    }

    private void NextLine()
    {
        if (currentLine < dialogLines.Length - 1)
        {
            currentLine++;

            PlayDialogSound(); // <-- SOM RECOMEÇA PARA A PRÓXIMA FALA

            StartCoroutine(TypeDialog(dialogLines[currentLine]));
        }
        else
        {
            EndDialog();
        }
    }

    private void EndDialog()
    {
        StopDialogSound(); // <-- SOM PARA AO TERMINAR

        dialogActive = false;
        dialogPanel.SetActive(false);
        actionButton.gameObject.SetActive(false);

        Destroy(gameObject);
    }

    // -----------------------------
    // CONTROLE DO SOM
    // -----------------------------

    private void PlayDialogSound()
    {
        StopDialogSound();  // Garante que não duplique

        dialogSound = RuntimeManager.CreateInstance(SomDialog);
        dialogSound.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        dialogSound.start();
    }

    private void StopDialogSound()
    {
        if (dialogSound.isValid())
        {
            dialogSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            dialogSound.release();
        }
    }
}
