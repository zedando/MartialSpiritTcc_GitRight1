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
    // =========================
    //  NOVA ESTRUTURA DE DIÁLOGO
    // =========================
    [System.Serializable]
    public class DialogueEntry
    {
        [TextArea(2, 4)]
        public string text;          // fala do personagem
        public string characterName; // nome do personagem
        public Sprite characterSprite; // fotinha do personagem
    }

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

    // =========================
    //     UI DE DIÁLOGO NOVA
    // =========================
    [Header("Dialogues")]
    public GameObject dialoguePanel;     // painel fixo atrás do diálogo (deixa aqui o painel de fundo) // NEW
    public Image characterImage;         // imagem do personagem (fotinha)                            // NEW
    public TMP_Text nameText;            // nome do personagem                                       // NEW
    public TMP_Text dialogueText;        // texto do diálogo (já existia, só mantive aqui)
    
    public DialogueEntry[] dialogues;    // AGORA é um array de estruturas, não mais string[]         // CHANGED

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

    [Header("Overlay da imagem da cutscene")]
    public Image canvasImageOverlay;

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

        // painel de diálogo começa desativado
        if (dialoguePanel != null)       // NEW
            dialoguePanel.SetActive(false);
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
        // MODO 1: NORMAL (DIÁLOGOS)
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

        if (dialoguePanel != null)   // garante que o painel de diálogo fique off no modo só vídeo // NEW
            dialoguePanel.SetActive(false);

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

        if (dialoguePanel != null)   // garante que painel de diálogo não apareça aqui           // NEW
            dialoguePanel.SetActive(false);

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

    // =========================
    //   NOVO SISTEMA DE DIÁLOGO
    // =========================
    IEnumerator ShowDialogues()
    {
        if (dialogues == null || dialogues.Length == 0)
            yield break;

        // liga o painel de diálogo enquanto estiver falando
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        foreach (DialogueEntry entry in dialogues)
        {
            // seta nome
            if (nameText != null)
                nameText.text = entry.characterName;

            // seta imagem do personagem
            if (characterImage != null)
                characterImage.sprite = entry.characterSprite;

            // texto da fala
            if (typewriterEffect)
                yield return StartCoroutine(TypeText(entry.text));
            else if (dialogueText != null)
                dialogueText.text = entry.text;

            yield return new WaitForSeconds(dialogueDelay);
        }

        // desliga painel depois de terminar todas as falas
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    IEnumerator TypeText(string line)
    {
        if (dialogueText == null)
            yield break;

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
