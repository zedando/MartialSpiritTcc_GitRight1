using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GD_EnemyManager : MonoBehaviour
{
    public static GD_EnemyManager instance;
    private int totalEnemies = 0;
    private int deadEnemies = 0;

    [Header("Fade Config")]
    public CanvasGroup fadeCanvasGroup;      // Arraste um painel preto com CanvasGroup
    public float fadeDuration = 1.5f;

    [Header("Mensagem de Vitória")]
    public TextMeshProUGUI mensagemVitoriaText;  // Arraste o TMP no centro da tela
    public string textoVitoria = "Você derrotou os valentões!";
    public float tempoMensagem = 2f;

    [Header("Cena de Destino")]
    public string nomeCena = "DojoReflexao_BRIGA";

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEnemy()
    {
        totalEnemies++;
    }

    public void EnemyDied()
    {
        deadEnemies++;
        Debug.Log("Inimigos mortos: " + deadEnemies + "/" + totalEnemies);

        if (deadEnemies >= totalEnemies)
        {
            Debug.Log("Todos os inimigos morreram!");
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        // Mostra texto de vitória
        if (mensagemVitoriaText != null)
        {
            mensagemVitoriaText.text = textoVitoria;
            mensagemVitoriaText.gameObject.SetActive(true);
        }

        // Fade para preto
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.gameObject.SetActive(true);
            float t = 0f;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
                yield return null;
            }
        }

        // Espera a mensagem aparecer tempoMensagem segundos
        yield return new WaitForSeconds(tempoMensagem);

        // Troca de cena
        SceneManager.LoadScene(nomeCena);
    }
}
