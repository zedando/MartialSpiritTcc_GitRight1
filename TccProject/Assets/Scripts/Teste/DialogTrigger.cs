using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    private int currentLine = 0;
    private bool isTyping = false;
    private bool cancelTyping = false;

    private void Start()
    {
        dialogPanel.SetActive(false);  
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

    private void StartDialog()
    {
        dialogPanel.SetActive(true);
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

    public void NextLine()
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
        dialogPanel.SetActive(false);
        Destroy(gameObject); 
    }
}
