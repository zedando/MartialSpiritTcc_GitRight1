using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectCollisions : MonoBehaviour
{
    public Text timerText;   // Arraste o TimerText do Canvas
    public Text livesText;   // Arraste o LivesText do Canvas

    private float timeRemaining = 30f;
    private bool timerIsRunning = true;
    private int lives = 3;

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
        }
    }
}