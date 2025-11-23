using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;

public class AudioSettingsFMOD : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("FMOD VCA Paths")]
    public string masterVCAPath = "vca:/MasterVCA";
    public string musicVCAPath = "vca:/MusicVCA";
    public string sfxVCAPath = "vca:/SFXVCA";

    private VCA masterVCA;
    private VCA musicVCA;
    private VCA sfxVCA;

    void Start()
    {
        masterVCA = RuntimeManager.GetVCA(masterVCAPath);
        musicVCA = RuntimeManager.GetVCA(musicVCAPath);
        sfxVCA = RuntimeManager.GetVCA(sfxVCAPath);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        masterVCA.setVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        musicVCA.setVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        sfxVCA.setVolume(value);
    }
}
