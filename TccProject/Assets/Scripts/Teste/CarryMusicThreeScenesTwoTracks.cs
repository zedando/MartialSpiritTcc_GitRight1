using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class CarryMusicThreeScenesTwoTracks : MonoBehaviour
{
    [Header("Música 1 (FMOD)")]
    public EventReference musicEvent1;

    [Tooltip("Cenas onde a Música 1 deve continuar")]
    public string scene1_M1 = "Cena1";
    public string scene2_M1 = "Cena2";
    public string scene3_M1 = "Cena3";

    [Header("Música 2 (FMOD)")]
    public EventReference musicEvent2;

    [Tooltip("Cenas onde a Música 2 deve continuar")]
    public string scene1_M2 = "Cena4";
    public string scene2_M2 = "Cena5";
    public string scene3_M2 = "Cena6";

    private static CarryMusicThreeScenesTwoTracks instance;

    private EventInstance musicInstance1;
    private EventInstance musicInstance2;

    private bool isPlaying1 = false;
    private bool isPlaying2 = false;

    private void Awake()
    {
        // Evita duplicados
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Cria instâncias
        musicInstance1 = RuntimeManager.CreateInstance(musicEvent1);
        musicInstance2 = RuntimeManager.CreateInstance(musicEvent2);

        // Começa as músicas (você pode escolher só 1 também)
        PlayMusic1();
        PlayMusic2();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // ---------------- MUSIC 1 ----------------

    private void PlayMusic1()
    {
        if (!isPlaying1)
        {
            musicInstance1.start();
            isPlaying1 = true;
        }
    }

    private void StopMusic1()
    {
        if (isPlaying1)
        {
            musicInstance1.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying1 = false;
        }
    }

    private bool IsSceneAllowedForMusic1(string sceneName)
    {
        return sceneName == scene1_M1 ||
               sceneName == scene2_M1 ||
               sceneName == scene3_M1;
    }

    // ---------------- MUSIC 2 ----------------

    private void PlayMusic2()
    {
        if (!isPlaying2)
        {
            musicInstance2.start();
            isPlaying2 = true;
        }
    }

    private void StopMusic2()
    {
        if (isPlaying2)
        {
            musicInstance2.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying2 = false;
        }
    }

    private bool IsSceneAllowedForMusic2(string sceneName)
    {
        return sceneName == scene1_M2 ||
               sceneName == scene2_M2 ||
               sceneName == scene3_M2;
    }

    // ---------------- SCENE CONTROL ----------------

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        // Música 1
        if (IsSceneAllowedForMusic1(sceneName))
        {
            PlayMusic1();
        }
        else
        {
            StopMusic1();
            musicInstance1.release();
        }

        // Música 2
        if (IsSceneAllowedForMusic2(sceneName))
        {
            PlayMusic2();
        }
        else
        {
            StopMusic2();
            musicInstance2.release();
        }

        // Se nenhuma das duas pode continuar, destruir o objeto
        if (!IsSceneAllowedForMusic1(sceneName) && !IsSceneAllowedForMusic2(sceneName))
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
