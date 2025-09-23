using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName ="ScriptableObject/Dialogue")]
public class DialogoSo : ScriptableObject
{
    public List<DialogSentence> Sentence;
}

[Serializable]
public class DialogSentence
{
    public DialogoScript ActorData;
    [TextArea(3, 5)]
    public string Content;

    [Header("Perguntas")]
    public string perguntasID; // um identificador para escolher a caixa na cena
}