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
    public GameObject keyEImage; // ícone "E" na tela
    public Button actionButton; // BOTÃO COMPARTILHADO NO PAINEL

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
    public EventReference SomDialog;
    private EventInstance dialogSound; // Instância do som atual (inicializada vazia por default)

    void Start()
    {
        dialogPanel.SetActive(false);
        keyEImage.SetActive(false);
    }

    void Update()
    {
        // Mostrar dica "E" quando o player está perto e o diálogo ainda não começou
        if (playerInRange && !dialogActive)
        {
            keyEImage.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                RuntimeManager.PlayOneShot(eventoInteragirObjeto, transform.position);
                StartDialog();
            }
        }
        else
        {
            if (!dialogActive)
                keyEImage.SetActive(false);
        }

        // Teclado (PC): espaço faz a mesma coisa que o botão/click
        if (dialogActive && Input.GetKeyDown(KeyCode.Space))
        {
            OnDialogClick();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (!dialogActive)
                keyEImage.SetActive(false);
        }
    }

    // 👉 ESSA É A FUNÇÃO QUE O BOTÃO COMPARTILHADO VAI CHAMAR
    public void OnDialogClick()
    {
        if (!dialogActive)
            return;

        if (isTyping)
        {
            // se ainda está escrevendo, completa a frase
            cancelTyping = true;
        }
        else
        {
            // se a frase já terminou, vai pra próxima
            NextLine();
        }
    }

    public void StartDialog()
    {
        dialogActive = true;
        dialogPanel.SetActive(true);
        keyEImage.SetActive(false);

        characterImage.sprite = characterSprite;
        characterNameText.text = characterName;

        currentLine = 0;

        StartCoroutine(TypeDialog(dialogLines[currentLine]));
    }

    IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        cancelTyping = false;
        dialogText.text = "";

        PlayDialogSound(); // SOM COMEÇA QUANDO COMEÇA A DIGITAÇÃO

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

        StopDialogSound(); // SOM PARA AO TERMINAR A FRASE

        isTyping = false;
        cancelTyping = false;
    }

    void NextLine()
    {
        // Garante que a corrotina anterior foi interrompida antes de começar a próxima
        if (isTyping)
            StopCoroutine("TypeDialog");

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

    void EndDialog()
    {
        // Garante que qualquer corrotina de digitação pendente seja parada
        StopAllCoroutines();
        StopDialogSound(); // Garante que o som pare ao fim do diálogo

        dialogActive = false;
        dialogPanel.SetActive(false);

        Destroy(gameObject); // O OnDestroy() será chamado logo após esta linha
    }

    // -----------------------------
    // CONTROLE DO SOM
    // -----------------------------
    private void PlayDialogSound()
    {
        StopDialogSound(); // Para e libera o anterior para evitar acumulação de instâncias

        dialogSound = RuntimeManager.CreateInstance(SomDialog);
        dialogSound.start();
    }

    private void StopDialogSound()
    {
        // Verifica se a instância é válida antes de tentar parar e liberar
        if (dialogSound.isValid())
        {
            dialogSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            dialogSound.release();

            // Limpa a referência após liberar
            dialogSound = new EventInstance();
        }
    }

    // 🛑 MÉTODO CRUCIAL PARA PREVENIR BUGS NA TROCA DE CENA 🛑
    private void OnDestroy()
    {
        // Garante que o som pare e seja liberado se o objeto for destruído
        StopDialogSound();
    }
}
