using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogTriggerButton : MonoBehaviour
{
    [Header("UI da Dialog")]
    public GameObject dialogPanel;      // Painel do diálogo (desativado no início)
    public Image characterImage;        // Foto do personagem
    public TMP_Text characterNameText;  // Nome do personagem
    public TMP_Text dialogText;         // Texto do diálogo
    public GameObject keyEImage;        // Imagem pequena do 'E' para aparecer quando o player estiver no trigger
    public Button actionButton;         // Botão grande para abrir/avançar diálogo (na tela)

    [Header("Configuração do diálogo")]
    public Sprite characterSprite;
    public string characterName;
    [TextArea(3, 10)]
    public string[] dialogLines;

    public float typingSpeed = 0.05f;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool cancelTyping = false;
    private bool playerInRange = false;  // Controla se player está no trigger
    private bool dialogActive = false;   // Se o diálogo está aberto

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
            // Mostra a imagem do E e o botão para abrir diálogo
            keyEImage.SetActive(true);
            actionButton.gameObject.SetActive(true);

            // Tecla E para iniciar diálogo
            if (Input.GetKeyDown(KeyCode.E))
            {
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
            // No diálogo aberto, tecla E avança o texto
            if (Input.GetKeyDown(KeyCode.Space))
            {
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
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // Se o player sair do trigger, fecha UI 'E' e botão se diálogo não estiver aberto
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
                cancelTyping = true;
            }
            else
            {
                NextLine();
            }
        }
    }

    private void StartDialog()
    {
        dialogActive = true;
        dialogPanel.SetActive(true);
        keyEImage.SetActive(false);
        actionButton.gameObject.SetActive(true); // mantém botão para avançar diálogo
        characterImage.sprite = characterSprite;
        characterNameText.text = characterName;
        currentLine = 0;
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
        Destroy(gameObject);
    }
}
