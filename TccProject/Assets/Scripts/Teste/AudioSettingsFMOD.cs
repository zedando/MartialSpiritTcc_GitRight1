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

    // Chaves do PlayerPrefs
    private const string MASTER_KEY = "Volume_Master";
    private const string MUSIC_KEY = "Volume_Music";
    private const string SFX_KEY = "Volume_SFX";

    void Start()
    {
        // Pega as VCAs
        masterVCA = RuntimeManager.GetVCA(masterVCAPath);
        musicVCA = RuntimeManager.GetVCA(musicVCAPath);
        sfxVCA = RuntimeManager.GetVCA(sfxVCAPath);

        // Carrega volumes salvos (ou 1f se nunca salvou)
        float master = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        float music = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        // Aplica nos sliders (isso já reflete visualmente)
        if (masterSlider != null) masterSlider.value = master;
        if (musicSlider != null) musicSlider.value = music;
        if (sfxSlider != null) sfxSlider.value = sfx;

        // Aplica nos VCAs
        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSFXVolume(sfx);

        // Liga os listeners DEPOIS de setar os valores pra não dar loop
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
    }

    // Esses métodos são chamados pelo slider
    private void OnMasterSliderChanged(float value)
    {
        SetMasterVolume(value);
        PlayerPrefs.SetFloat(MASTER_KEY, value);
        PlayerPrefs.Save();
    }

    private void OnMusicSliderChanged(float value)
    {
        SetMusicVolume(value);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
        PlayerPrefs.Save();
    }

    private void OnSFXSliderChanged(float value)
    {
        SetSFXVolume(value);
        PlayerPrefs.SetFloat(SFX_KEY, value);
        PlayerPrefs.Save();
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
