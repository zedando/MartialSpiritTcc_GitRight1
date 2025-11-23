using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Música de Fundo (FMOD)")]
    public EventReference musicEvent;   // ex: event:/Music/Fase1

    private EventInstance musicInstance;
    private bool isPlaying = false;

    private void Awake()
    {
        // Garante que só exista UM MusicManager (singleton)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Não destrói ao trocar de cena

        // Cria a instância da música
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (!isPlaying)
        {
            musicInstance.start();
            isPlaying = true;
        }
    }

    public void StopMusic()
    {
        if (isPlaying)
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying = false;
        }
    }

    private void OnDestroy()
    {
        // Se um dia o manager realmente for destruído, libera a instância
        musicInstance.release();
    }
}
