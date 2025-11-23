using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class CarryMusicTwoScenes : MonoBehaviour
{
    [Header("Música (FMOD)")]
    [Tooltip("Evento de música do FMOD (ex: event:/Music/Fase1)")]
    public EventReference musicEvent;

    [Header("Cenas onde a música deve tocar")]
    [Tooltip("Nome da primeira cena (ex: Fase1)")]
    public string firstSceneName = "Fase1";
    
    [Tooltip("Nome da segunda cena (ex: Fase2)")]
    public string secondSceneName = "Fase2";

    private static CarryMusicTwoScenes instance;
    private EventInstance musicInstance;
    private bool isPlaying = false;

    private void Awake()
    {
        // Garante que só exista um objeto desses
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Cria a instância da música
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        PlayMusic();

        // Escuta mudanças de cena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void PlayMusic()
    {
        if (!isPlaying)
        {
            musicInstance.start();
            isPlaying = true;
        }
    }

    private void StopMusic()
    {
        if (isPlaying)
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying = false;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Se não estiver mais na cena 1 nem na cena 2, para e destrói
        if (scene.name != firstSceneName && scene.name != secondSceneName)
        {
            StopMusic();
            musicInstance.release();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
            Destroy(gameObject);
        }
        // Se estiver na cena 1 ou 2, só continua tocando normal
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
