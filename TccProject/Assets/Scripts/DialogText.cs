using System.Collections;
using UnityEngine;
using TMPro;
using FMODUnity;
using FMOD.Studio;

[RequireComponent(typeof(TMP_Text))]
public class DialogText : MonoBehaviour
{
    [Header("Configuração de texto")]
    [SerializeField] private float intervalBetweenChars = 0.2f;

    [Header("Som de diálogo (FMOD)")]
    [Tooltip("Evento de som de digitação / murmúrio")]
    public EventReference typingSound;

    private TMP_Text text;

    // FMOD
    private EventInstance typingInstance;
    private bool typingSoundPlaying = false;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    // -----------------------------
    // SOM
    // -----------------------------
    private void StartTypingSound()
    {
        if (typingSoundPlaying || typingSound.IsNull) return;

        typingInstance = RuntimeManager.CreateInstance(typingSound);
        typingInstance.start();
        typingSoundPlaying = true;
    }

    private void StopTypingSound()
    {
        if (!typingSoundPlaying) return;

        typingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        typingInstance.release();
        typingSoundPlaying = false;
    }

    private void OnDestroy()
    {
        StopTypingSound();
    }

    // -----------------------------
    // TEXTO
    // -----------------------------
    public IEnumerator ShowText(string content)
    {
        text.maxVisibleCharacters = 0;
        text.SetText(content);


        StartTypingSound();

        yield return RevealChars();


        StopTypingSound();
    }

    public void HideText()
    {
        StopTypingSound(); 
        text.SetText("");
        text.maxVisibleCharacters = 0;
    }

    public void SkipAnimation()
    {
        text.maxVisibleCharacters = text.textInfo.characterCount;
       
        StopTypingSound();
    }

    public IEnumerator RevealChars()
    {

        text.ForceMeshUpdate();

        while (text.maxVisibleCharacters < text.textInfo.characterCount)
        {
            yield return new WaitForSeconds(intervalBetweenChars);
            text.maxVisibleCharacters++;
        }
    }
}
