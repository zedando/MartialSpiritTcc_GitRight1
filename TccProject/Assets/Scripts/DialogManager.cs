using System.Collections;
using System.Collections.Generic;
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

    [Header("Perguntas na Cena")]
    [SerializeField] private List<GameObject> perguntasObjs; // arraste todas as caixas da cena

    void Start()
    {
        // no começo, deixa todas desativadas
        foreach (var obj in perguntasObjs)
            if (obj != null) obj.SetActive(false);

        GameEvents.Instance.OnStartDialog += HandleStartDialog;
    }

    public void HandleStartDialog(DialogoSo dialogData)
    {
        StartCoroutine(StartDialog(dialogData));
    }

    public IEnumerator StartDialog(DialogoSo dialogData)
    {
        charImage.enabled = false;
        nameText.SetText("");

        yield return dialogBar.ShowBar();
        charImage.enabled = true;

        foreach (var sentence in dialogData.Sentence)
        {
            // Mostra quem está falando
            nameText.SetText(sentence.ActorData.CharName);
            charImage.sprite = sentence.ActorData.Sprite;

            // Mostra o texto
            yield return dialogText.ShowText(sentence.Content);

            // Ativa a caixa de perguntas se houver ID configurado
            if (!string.IsNullOrEmpty(sentence.perguntasID))
            {
                foreach (var obj in perguntasObjs)
                {
                    if (obj != null && obj.name == sentence.perguntasID)
                    {
                        obj.SetActive(true);
                    }
                }
            }

            yield return new WaitForSeconds(intervalBetweenSentences);
        }

        yield return dialogBar.HideBar();
        dialogText.HideText();

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