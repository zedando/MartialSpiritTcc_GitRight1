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
    // Mostra a imagem sobre o vídeo (para evitar flicker)
    canvasImageOverlay.gameObject.SetActive(true);
    canvasImageOverlay.color = new Color(1, 1, 1, 1);

    // Esconde o vídeo até estar pronto
    videoRawImage.color = new Color(1, 1, 1, 0);

    // Mantém imagem por 3s antes do vídeo
    yield return new WaitForSeconds(imageOnScreenBeforeVideo);

    // Prepara o vídeo
    videoPlayer.Prepare();
    while (!videoPlayer.isPrepared)
        yield return null;

    // Começa a tocar invisível
    videoPlayer.time = 0;
    videoPlayer.Play();

    // Espera o vídeo realmente renderizar
    while (videoPlayer.texture == null)
        yield return null;

    // Dá 1 frame extra para garantir que foi desenhado
    yield return new WaitForEndOfFrame();

    // Agora revela o vídeo
    videoRawImage.color = new Color(1, 1, 1, 1);

    // Faz fade da imagem da cutscene (MS sem flicker)
    float t = 0;
    while (t < 1f)
    {
        canvasImageOverlay.color = new Color(1, 1, 1, 1 - t);
        t += Time.deltaTime * 2; // velocidade do fade
        yield return null;
    }
    canvasImageOverlay.color = new Color(1, 1, 1, 0);
    canvasImageOverlay.gameObject.SetActive(false);

    // Espera o vídeo terminar
    while (videoPlayer.isPlaying)
        yield return null;

    yield return new WaitForSeconds(0.2f);

    if (enableRedFlash)
        yield return StartCoroutine(RedFlashEffect());

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
