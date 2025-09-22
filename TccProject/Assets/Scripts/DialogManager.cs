using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image charImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private DialogBar dialogBar;
    [SerializeField] private DialogText dialogText;

    [Header("Settings")]
    [SerializeField] private float intervalBetweenSentences = 1;

    [Header("Perguntas")]
    [SerializeField] private GameObject perguntasObj; // <-- arraste o objeto já existente da cena

    void Start()
    {
        GameEvents.Instance.OnStartDialog += HandleStartDialog;
    }

    public void HandleStartDialog(DialogoSo dialogData)
    {
        StartCoroutine(StartDialog(dialogData));
        Debug.Log("oi");
    }

    public IEnumerator StartDialog(DialogoSo dialogData)
    {
        charImage.enabled = false;
        nameText.SetText("");

        yield return dialogBar.ShowBar();
        charImage.enabled = true;

        foreach (var sentence in dialogData.Sentence)
        {
            nameText.SetText(sentence.ActorData.CharName);
            charImage.sprite = sentence.ActorData.Sprite;
            yield return dialogText.ShowText(sentence.Content);
            yield return new WaitForSeconds(intervalBetweenSentences);
        }

        yield return dialogBar.HideBar();
        dialogText.HideText();

        // Só ativa o objeto se a flag do ScriptableObject estiver marcada
        if (dialogData.ativaPerguntasNoFinal && perguntasObj != null)
        {
            perguntasObj.SetActive(true);
        }

        GameEvents.Instance.FinishDialog();
    }

    void OnDestroy()
    {
        GameEvents.Instance.OnStartDialog -= HandleStartDialog;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) dialogText.SkipAnimation();
    }
}