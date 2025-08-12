using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public Slider volumeSlider;
    private EventInstance musicInstance;
    private VCA vcaMusica;

    void Start()
    {
        
        musicInstance = RuntimeManager.CreateInstance("event:/musicMenu"); 
        musicInstance.start();

        // Pega o VCA da música para controlar volume
        vcaMusica = RuntimeManager.GetVCA("vca:/VCA-musica");

        // Ajusta o slider para o volume atual do VCA
        float volumeAtual;
        vcaMusica.getVolume(out volumeAtual);
        volumeSlider.value = volumeAtual;

        // Adiciona o listener para o slider controlar o volume em tempo real
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    void SetVolume(float volume)
    {
         Debug.Log("Volume ajustado para: " + volume);
        vcaMusica.setVolume(volume);
    }

    private void OnDestroy()
    {
        // Para a música ao destruir o objeto para evitar vazamentos de som
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}
