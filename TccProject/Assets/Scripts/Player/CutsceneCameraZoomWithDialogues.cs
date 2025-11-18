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
    public Image fadeOverlay;         // DEVE estar acima de tudo no Canvas (ordenado no inspector)
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
    public bool playVideoInstead = false;
    public VideoPlayer videoPlayer;
    public RawImage videoRawImage;
    public float imageOnScreenBeforeVideo = 3f;

    private bool videoFinished = false;

    void Awake()
    {
        // Garantias iniciais - overlays com alpha 0 e ativos para evitar que Fade não apareça.
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
            fadeOverlay.gameObject.SetActive(true); // importante: ativo e acima de tudo no Canvas
        }

        if (videoRawImage != null)
        {
            videoRawImage.color = new Color(1f, 1f, 1f, 0f);
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
        if (useFade)
            yield return StartCoroutine(FadeIn());

        if (playVideoInstead)
        {
            yield return StartCoroutine(PlayVideoMode());
        }
        else
        {
            if (enableZoom && imageTransform != null)
                StartCoroutine(DepthZoom());

            yield return StartCoroutine(ShowDialogues());

            yield return new WaitForSeconds(fadeOutDelay);

            if (enableRedFlash && redOverlay != null)
                yield return StartCoroutine(RedFlashEffect());

            // Garantia: deixa fadeOverlay ativo e com alpha 0 antes de iniciar fade out
            if (fadeOverlay != null)
            {
                fadeOverlay.gameObject.SetActive(true);
                Color c = fadeOverlay.color;
                c.a = 0f;
                fadeOverlay.color = c;
            }

            yield return StartCoroutine(FadeOut());
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // -----------------------------------------
    //              MODO VÍDEO
    // -----------------------------------------
    IEnumerator PlayVideoMode()
    {
        // Mostra a imagem antes do vídeo
        if (canvasImageOverlay != null)
        {
            canvasImageOverlay.gameObject.SetActive(true);
            Color col = canvasImageOverlay.color;
            col.a = 1f;
            canvasImageOverlay.color = col;
        }

        // Tempo para mostrar a imagem
        yield return new WaitForSeconds(imageOnScreenBeforeVideo);

        // Prepara o vídeo
        if (videoPlayer == null || videoRawImage == null)
        {
            Debug.LogWarning("VideoPlayer ou VideoRawImage não atribuídos.");
            yield break;
        }

        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;

        // Atribui textura ao RawImage (essencial)
        if (videoPlayer.texture != null)
            videoRawImage.texture = videoPlayer.texture;

        // Garante invisibilidade do vídeo antes do fade
        videoRawImage.color = new Color(1f, 1f, 1f, 0f);

        videoPlayer.time = 0;
        videoPlayer.playbackSpeed = 1f;
        videoPlayer.Play();

        yield return new WaitForEndOfFrame();

        // Fade suave da imagem para o vídeo
        if (canvasImageOverlay != null)
        {
            float t = 0f;
            float fadeSpeed = 1.5f;
            Color imgCol = canvasImageOverlay.color;

            while (t < 1f)
            {
                imgCol.a = Mathf.Lerp(1f, 0f, t);
                canvasImageOverlay.color = imgCol;
                t += Time.deltaTime * fadeSpeed;
                yield return null;
            }

            imgCol.a = 0f;
            canvasImageOverlay.color = imgCol;
            canvasImageOverlay.gameObject.SetActive(false);
        }

        // Revela o RawImage do vídeo
        videoRawImage.color = new Color(1f, 1f, 1f, 1f);

        // Aguarda o evento de fim do vídeo
        while (!videoFinished)
            yield return null;

        // Pequena pausa
        yield return new WaitForSeconds(0.2f);

        if (enableRedFlash && redOverlay != null)
            yield return StartCoroutine(RedFlashEffect());

        // Garantia: ativa overlay de fade e zera alpha
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
        }

        // Fade final e troca de cena
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(nextSceneName);
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        videoFinished = true;
    }

    // -----------------------------------------
    //              PARTES ORIGINAIS
    // -----------------------------------------

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
        if (dialogueText == null || dialogues == null)
            yield break;

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
        if (fadeOverlay == null)
            yield break;

        // garante overlay ativo e alpha inicial = 1 (tela preta)
        fadeOverlay.gameObject.SetActive(true);
        Color c = fadeOverlay.color;
        c.a = 1f;
        fadeOverlay.color = c;

        float t = 0f;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(1f, 0f, t);
            fadeOverlay.color = c;
            t += Time.deltaTime / fadeDuration;
            yield return null;
        }

        c.a = 0f;
        fadeOverlay.color = c;
    }

    IEnumerator FadeOut()
    {
        if (fadeOverlay == null)
            yield break;

        // garante overlay ativo e alpha inicial = 0
        fadeOverlay.gameObject.SetActive(true);
        Color c = fadeOverlay.color;
        c.a = 0f;
        fadeOverlay.color = c;

        float t = 0f;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(0f, 1f, t);
            fadeOverlay.color = c;
            t += Time.deltaTime / fadeDuration;
            yield return null;
        }

        c.a = 1f;
        fadeOverlay.color = c;
    }

    IEnumerator RedFlashEffect()
    {
        if (redOverlay == null)
            yield break;

        Color c = redOverlay.color;

        float t = 0f;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(0f, redIntensity, t);
            redOverlay.color = c;
            t += Time.deltaTime / redFlashDuration;
            yield return null;
        }

        c.a = redIntensity;
        redOverlay.color = c;

        yield return new WaitForSeconds(redFlashTime);

        t = 0f;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(redIntensity, 0f, t);
            redOverlay.color = c;
            t += Time.deltaTime / redFlashDuration;
            yield return null;
        }

        c.a = 0f;
        redOverlay.color = c;
    }
}
