using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using FMODUnity;
using FMOD.Studio;

public class CutsceneCameraZoomWithDialogues : MonoBehaviour
{
    public enum VideoMode
    {
        None,               // MODO NORMAL (diálogos)
        ImageThenVideo,     // IMAGEM → VÍDEO
        VideoOnly           // SOMENTE VÍDEO
    }

    [Header("Video Mode")]
    public VideoMode videoMode = VideoMode.None;

    [Header("Cutscene Image (UI)")]
    public RectTransform imageTransform;

    [Header("Depth Zoom Settings")]
    public bool enableZoom = true;
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

    [Header("Video")]
    public VideoPlayer videoPlayer;
    public RawImage videoRawImage;
    public float imageOnScreenBeforeVideo = 3f;

    private bool videoFinished = false;


    void Awake()
    {
        // setup overlays
        if (redOverlay != null)
        {
            Color rc = redOverlay.color;
            rc.a = 0f;
            redOverlay.color = rc;
            redOverlay.gameObject.SetActive(true);
        }

        if (fadeOverlay != null)
        {
            Color fc = fadeOverlay.color;
            fc.a = 0f;
            fadeOverlay.color = fc;
            fadeOverlay.gameObject.SetActive(true);
        }

        if (videoRawImage != null)
        {
            videoRawImage.color = new Color(1, 1, 1, 0);
            videoRawImage.gameObject.SetActive(true);
        }

        if (canvasImageOverlay != null)
            canvasImageOverlay.gameObject.SetActive(true);
    }

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.isLooping = false;
        }

        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        // --------------------------------------------------
        // MODO 3: SOMENTE VÍDEO  (SEM FADE IN)
        // --------------------------------------------------
        if (videoMode == VideoMode.VideoOnly)
        {
            yield return StartCoroutine(PlayVideoOnly());
            yield break;
        }

        // --------------------------------------------------
        // MODO NORMAL → aplica fade in
        // --------------------------------------------------
        if (useFade)
            yield return StartCoroutine(FadeIn());

        // --------------------------------------------------
        // MODO 2: IMAGEM → VÍDEO
        // --------------------------------------------------
        if (videoMode == VideoMode.ImageThenVideo)
        {
            yield return StartCoroutine(PlayVideoMode());
            yield break;
        }

        // --------------------------------------------------
        // MODO 1: NORMAL
        // --------------------------------------------------

        if (enableZoom && imageTransform != null)
            StartCoroutine(DepthZoom());

        yield return StartCoroutine(ShowDialogues());

        yield return new WaitForSeconds(fadeOutDelay);

        if (enableRedFlash && redOverlay != null)
            yield return StartCoroutine(RedFlashEffect());

        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(nextSceneName);
    }

    // =====================================================
    //                     MODO 3: SÓ VÍDEO
    // =====================================================
    IEnumerator PlayVideoOnly()
    {
        // SEM FADE-IN — vídeo direto

        if (canvasImageOverlay != null)
            canvasImageOverlay.gameObject.SetActive(false);

        fadeOverlay.color = new Color(0, 0, 0, 0);

        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;

        videoRawImage.texture = videoPlayer.texture;
        videoRawImage.color = new Color(1, 1, 1, 1);

        videoFinished = false;
        videoPlayer.time = 0;
        videoPlayer.Play();

        while (!videoFinished)
            yield return null;

        yield return new WaitForSeconds(0.2f);

        if (enableRedFlash)
            yield return StartCoroutine(RedFlashEffect());

        yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene(nextSceneName);
    }

    // =====================================================
    //            MODO 2: IMAGEM → VÍDEO
    // =====================================================
    IEnumerator PlayVideoMode()
    {
        canvasImageOverlay.gameObject.SetActive(true);

        Color col = canvasImageOverlay.color;
        col.a = 1;
        canvasImageOverlay.color = col;

        yield return new WaitForSeconds(imageOnScreenBeforeVideo);

        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;

        videoRawImage.texture = videoPlayer.texture;
        videoRawImage.color = new Color(1, 1, 1, 0);

        videoFinished = false;
        videoPlayer.time = 0;
        videoPlayer.Play();

        // Fade da imagem para o vídeo
        float t = 0f;
        float fadeSpeed = 1.5f;

        while (t < 1f)
        {
            col.a = Mathf.Lerp(1, 0, t);
            canvasImageOverlay.color = col;
            t += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasImageOverlay.color = new Color(1, 1, 1, 0);
        canvasImageOverlay.gameObject.SetActive(false);

        videoRawImage.color = new Color(1, 1, 1, 1);

        while (!videoFinished)
            yield return null;

        yield return new WaitForSeconds(0.2f);

        if (enableRedFlash)
            yield return StartCoroutine(RedFlashEffect());

        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(nextSceneName);
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        videoFinished = true;
    }

    // =====================================================
    //                FUNÇÕES ORIGINAIS
    // =====================================================

    IEnumerator DepthZoom()
    {
        if (imageTransform == null)
            yield break;

        Vector3 startScale = imageTransform.localScale;
        Vector3 endScale = new Vector3(targetScale, targetScale, 1);

        float startZ = imageTransform.anchoredPosition3D.z;
        float endZ = startZ - depthAmount;

        float t = 0f;
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

        float t = 0f;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(1, 0, t);
            fadeOverlay.color = c;
            t += Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        Color c = fadeOverlay.color;
        c.a = 0;
        fadeOverlay.color = c;

        float t = 0f;
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

        float t = 0f;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(0, redIntensity, t);
            redOverlay.color = c;
            t += Time.deltaTime / redFlashDuration;
            yield return null;
        }

        yield return new WaitForSeconds(redFlashTime);

        t = 0f;
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
