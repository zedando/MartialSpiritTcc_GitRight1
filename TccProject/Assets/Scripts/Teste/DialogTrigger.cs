using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class DialogTrigger : MonoBehaviour
{
    [Header("UI da Dialog")]
    public GameObject dialogPanel;
    public Image characterImage;
    public TMP_Text characterNameText;
    public TMP_Text dialogText;

    [Header("Configuração do diálogo")]
    public Sprite characterSprite;
    public string characterName;
    [TextArea(3, 10)]
    public string[] dialogLines;
    public float typingSpeed = 0.05f;

    [Header("Som de Diálogo (FMOD)")]
    public EventReference dialogTypingEvent;

    private EventInstance dialogTypingInstance;
    private bool dialogSoundPlaying = false;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool cancelTyping = false;
    private Coroutine typingCoroutine;
    private bool lineJustFinished = false;

    private void Start()
    {
        dialogPanel.SetActive(false);

        // Cria instância do som de diálogo
        dialogTypingInstance = RuntimeManager.CreateInstance(dialogTypingEvent);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartDialog();
        }
    }

    private void Update()
    {
        if (dialogPanel.activeSelf)
        {
            // Teclado (PC): Espaço faz a mesma coisa que o clique/toque
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnDialogClick();
            }
        }
    }

    private void StartDialog()
    {
        dialogPanel.SetActive(true);
        characterImage.sprite = characterSprite;
        characterNameText.text = characterName;

        currentLine = 0;
        string cleanedLine = CleanText(dialogLines[currentLine]);

        typingCoroutine = StartCoroutine(TypeDialog(cleanedLine));
    }

    IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        cancelTyping = false;
        dialogText.text = "";

        // Iniciar som
        StartDialogSound();

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

        // Parar som ao terminar
        StopDialogSound();

        isTyping = false;
        cancelTyping = false;
        lineJustFinished = true;
    }

    public void NextLine()
    {
        if (currentLine < dialogLines.Length - 1)
        {
            currentLine++;

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            string cleanedLine = CleanText(dialogLines[currentLine]);
            typingCoroutine = StartCoroutine(TypeDialog(cleanedLine));
        }
        else
        {
            EndDialog();
        }
    }

    private void EndDialog()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // A parada e liberação do FMOD agora são gerenciadas primariamente 
        // pelo OnDestroy() para cobrir todos os cenários (fim ou troca de cena).
        
        dialogPanel.SetActive(false);
        Destroy(gameObject);
    }

    // 👉 FUNÇÃO QUE O BOTÃO / TOQUE VAI CHAMAR
    public void OnDialogClick()
    {
        if (!dialogPanel.activeSelf) return;

        if (isTyping)
        {
            // Se ainda está digitando, completa a frase
            cancelTyping = true;
        }
        else if (lineJustFinished)
        {
            // Se a linha já acabou, vai pra próxima
            lineJustFinished = false;
            NextLine();
        }
    }

    // ⚡ SOM

    private void StartDialogSound()
    {
        if (!dialogSoundPlaying)
        {
            dialogTypingInstance.start();
            dialogSoundPlaying = true;
        }
    }

    private void StopDialogSound()
    {
        if (dialogSoundPlaying)
        {
            dialogTypingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            dialogSoundPlaying = false;
        }
    }

    private string CleanText(string input)
    {
        return input.Replace("\n", " ").Replace("\r", "").Trim();
    }

    // 🛑 MÉTODO CRUCIAL: Garante que o som pare e seja liberado
    // quando o objeto é destruído (fim da cena, fim do diálogo, etc.).
    private void OnDestroy()
    {
        // 1. Parar o som se estiver tocando
        StopDialogSound(); 

        // 2. Liberar a instância do FMOD (MUITO IMPORTANTE!)
        dialogTypingInstance.release();
    }
}