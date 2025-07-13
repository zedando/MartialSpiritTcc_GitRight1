using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



[RequireComponent(typeof(TMP_Text))]
public class DialogText : MonoBehaviour
{
    

    [SerializeField] private float intervalBetweenChars = 0.2F;

    private TMP_Text text;

    private void Awake() => text = GetComponent<TMP_Text>();

    public IEnumerator ShowText(string Content)
    {
        text.maxVisibleCharacters = 0;
        text.SetText(Content);
        yield return RevealChars();

    }
    public void HideText()
    {
        text.SetText("");
        text.maxVisibleCharacters = 0;
     }

    public void SkipAnimation() => text.maxVisibleCharacters = text.textInfo.characterCount;
         public IEnumerator RevealChars()
    {
        while (text.maxVisibleCharacters <= text.textInfo.characterCount)
        {
            yield return new WaitForSeconds(intervalBetweenChars);
            text.maxVisibleCharacters++;
        }
    }
    
}
