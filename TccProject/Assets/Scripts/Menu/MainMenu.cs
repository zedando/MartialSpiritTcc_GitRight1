using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject settingsPanel;

    [Header("Botões")]
    public Button playButton;
    public Button settingsButton;
    public Button backButton;
    public Button exitButton;

    void Start()
    {
        settingsPanel.SetActive(false);
        backButton.gameObject.SetActive(false);

        playButton.onClick.AddListener(PlayGame);
        settingsButton.onClick.AddListener(OpenSettings);
        backButton.onClick.AddListener(CloseSettings);
        exitButton.onClick.AddListener(ExitGame);
    }

    void PlayGame()
    {
        SceneManager.LoadScene("Map");
    }

    void OpenSettings()
    {
        settingsPanel.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

    void CloseSettings()
    {
        settingsPanel.SetActive(false);
        backButton.gameObject.SetActive(false);
    }

    void ExitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}
