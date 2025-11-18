using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneCameraZoomWithDialogues : MonoBehaviour
{
    [Header("Cutscene Image (UI)")]
    public RectTransform imageTransform;

    [Header("Depth Zoom Settings")]
    public bool enableZoom = true;        // <- NOVO: ativar/desativar zoom
    public float targetScale = 2f;
    public float depthAmount = 300f;
    public float zoomDuration = 10f;

    [Header("Dialogues")]
    public Image canvasImageOverlay;

    public TMP_Text dialogueText;
    [TextArea(2, 4)]
    public string[] dialogues;
    public float dialogueDelay = 2f;
    public bool typewriterEffect = true;
    public float typeSpeed = 0.04f;

    [Header("Fade Settings")]
    public Image fadeOverlay;
    public float fadeDuration = 1f;
    public bool useFade = true;

    [Header("Fade Out + Scene Transition")]
    public float fadeOutDelay = 3f;
    public string nextSceneName = "ProximaCena";

    [Header("Red Flash Before Fade Out")]
    public bool enableRedFlash = true;
    public Image redOverlay;
    public float redFlashTime = 1f;
    public float redFlashDuration = 1f;
    [Range(0f, 1f)]
    public float redIntensity = 0.35f;

    [Header("Video Cutscene Mode")]
    public bool playVideoInstead = false;          // <- NOVO
    public VideoPlayer videoPlayer;                // <- NOVO
    public RawImage videoRawImage;                 // <- NOVO
    public float imageOnScreenBeforeVideo = 3f;    // <- NOVO (3s antes do vídeo)

    void Start()
    {
        if (redOverlay != null)
        {
            Color rc = redOverlay.color;
            rc.a = 0;
            redOverlay.color = rc;
        }

        // Vídeo começa escondido
        if (videoRawImage != null)
            videoRawImage.color = new Color(1, 1, 1, 0);

        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        if (useFade)
            yield return StartCoroutine(FadeIn());

        // 🔥 MODO VÍDEO?
        if (playVideoInstead)
        {
            yield return StartCoroutine(PlayVideoMode());
        }
        else
        {
            // 🔥 MODO IMAGEM + DIÁLOGOS
            if (enableZoom)
                StartCoroutine(DepthZoom());

            yield return StartCoroutine(ShowDialogues());

            yield return new WaitForSeconds(fadeOutDelay);

            if (enableRedFlash)
                yield return StartCoroutine(RedFlashEffect());

            yield return StartCoroutine(FadeOut());
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // -----------------------------------------
    //  NOVO: SEÇÃO DO MODO VÍDEO
    // -----------------------------------------
IEnumerator PlayVideoMode()
{
    // --- GARANTE QUE A IMAGEM DA CUTSCENE ESTÁ VISÍVEL ---

    if (canvasImageOverlay != null)
    {
        canvasImageOverlay.gameObject.SetActive(true);

        // garante alpha 1 (totalmente visível)
        Color startColor = canvasImageOverlay.color;
        startColor.a = 1f;
        canvasImageOverlay.color = startColor;
    }

    // vídeo está invisível até estar pronto
    videoRawImage.color = new Color(1,1,1,0);


    // --- IMAGEM FICA 3s NA TELA ---

    yield return new WaitForSeconds(imageOnScreenBeforeVideo);


    // --- PREPARA O VÍDEO ---

    videoPlayer.Prepare();
    while (!videoPlayer.isPrepared)
        yield return null;
    videoPlayer.playbackSpeed = 0.8f;
    videoPlayer.time = 0;
    videoPlayer.Play();

    // espera o primeiro frame existir
    while (videoPlayer.texture == null)
        yield return null;

    yield return new WaitForEndOfFrame();


    // --- FADE SUAVE NA IMAGEM ANTES DO VÍDEO APARECER ---

    float fadeT = 0f;
    float fadeSpeed = 1.5f; // ajuste de velocidade do fade

    Color overlayColor = canvasImageOverlay.color;

    while (fadeT < 1f)
    {
        overlayColor.a = Mathf.Lerp(1f, 0f, fadeT);
        canvasImageOverlay.color = overlayColor;

        fadeT += Time.deltaTime * fadeSpeed;
        yield return null;
    }

    // garante invisível e desativa
    overlayColor.a = 0f;
    canvasImageOverlay.color = overlayColor;
    canvasImageOverlay.gameObject.SetActive(false);


    // --- REVELA O VÍDEO ---

    videoRawImage.color = new Color(1,1,1,1);

    // espera o vídeo terminar
    while (videoPlayer.isPlaying)
        yield return null;

    yield return new WaitForSeconds(0.2f);


    // --- EFEITO VERMELHO OPCIONAL ---
    if (enableRedFlash)
        yield return StartCoroutine(RedFlashEffect());


    // --- FADE FINAL + TROCA DE CENA ---
    yield return StartCoroutine(FadeOut());
    SceneManager.LoadScene(nextSceneName);
}





    // -----------------------------------------
    // PARTES ORIGINAIS
    // -----------------------------------------

    IEnumerator DepthZoom()
    {
        Vector3 startScale = imageTransform.localScale;
        Vector3 endScale = new Vector3(targetScale, targetScale, 1);

        float startZ = imageTransform.anchoredPosition3D.z;
        float endZ = startZ - depthAmount;

        float t = 0;
        while (t < 1f)
        {
            imageTransform.localScale = Vector3.Lerp(startScale, endScale, t);

            Vector3 pos = imageTransform.anchoredPosition3D;
            pos.z = Mathf.Lerp(startZ, endZ, t);
            imageTransform.anchoredPosition3D = pos;

            t += Time.deltaTime / zoomDuration;
            yield return null;
        }

        imageTransform.localScale = endScale;
        Vector3 finalPos = imageTransform.anchoredPosition3D;
        finalPos.z = endZ;
        imageTransform.anchoredPosition3D = finalPos;
    }

    IEnumerator ShowDialogues()
    {
        foreach (string line in dialogues)
        {
            if (typewriterEffect)
                yield return StartCoroutine(TypeText(line));
            else
                dialogueText.text = line;

            yield return new WaitForSeconds(dialogueDelay);
        }
    }

    IEnumerator TypeText(string line)
    {
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    IEnumerator FadeIn()
    {
        Color c = fadeOverlay.color;
        c.a = 1;
        fadeOverlay.color = c;

        float t = 0;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(1, 0, t);
            fadeOverlay.color = c;
            t += Time.deltaTime / fadeDuration;
            yield return null;
        }

        c.a = 0;
        fadeOverlay.color = c;
    }

    IEnumerator FadeOut()
    {
        Color c = fadeOverlay.color;
        c.a = 0;
        fadeOverlay.color = c;

        float t = 0;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(0, 1, t);
            fadeOverlay.color = c;
            t += Time.deltaTime / fadeDuration;
            yield return null;
        }

        c.a = 1;
        fadeOverlay.color = c;
    }

    IEnumerator RedFlashEffect()
    {
        Color c = redOverlay.color;

        float t = 0;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(0, redIntensity, t);
            redOverlay.color = c;
            t += Time.deltaTime / redFlashDuration;
            yield return null;
        }

        c.a = redIntensity;
        redOverlay.color = c;

        yield return new WaitForSeconds(redFlashTime);

        t = 0;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(redIntensity, 0, t);
            redOverlay.color = c;
            t += Time.deltaTime / redFlashDuration;
            yield return null;
        }

        c.a = 0;
        redOverlay.color = c;
    }
}
