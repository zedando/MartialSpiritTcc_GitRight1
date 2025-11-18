using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using echo17.EndlessBook;
using System.Collections;

public class DetectCollisions : MonoBehaviour
{
    public Text timerText;   // Arraste o TimerText do Canvas
    public Text livesText;   // Arraste o LivesText do Canvas

    private float timeRemaining = 30f;
    private bool timerIsRunning = true;
    private int lives = 3;
    public string cenaDestinoperdeu = "MiniGameAcontecendo";
    public string cenaDestinoganhou = "MiniGameAcontecendo";

    void Start()
    {
        UpdateTimerUI();
        UpdateLivesUI();
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI();
                
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                UpdateTimerUI();
                Debug.Log("Tempo acabou!");
                SceneManager.LoadScene(cenaDestinoganhou);
            }
        }
    }

    private void UpdateTimerUI()
    {
        timerText.text = "Tempo: " + Mathf.CeilToInt(timeRemaining).ToString();
    }

    private void UpdateLivesUI()
    {
        livesText.text = lives.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);

        // Perde uma vida
        lives--;

        UpdateLivesUI();

        if (lives <= 0)
        {
            Debug.Log("Game Over!");
            // Aqui você pode desativar o jogador, mostrar tela final, etc.
            SceneManager.LoadScene(cenaDestinoperdeu);
        }
    }
}