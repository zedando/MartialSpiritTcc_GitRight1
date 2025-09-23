using UnityEngine;

public class GD_EnemyManager : MonoBehaviour
{
    public static GD_EnemyManager instance;
    private int totalEnemies = 0;
    private int deadEnemies = 0;

    private void Awake()
    {
        // Singleton (garante que só exista um Manager na cena)
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
            // Aqui você pode chamar algo: abrir porta, liberar item, trocar cena etc.
        }
    }
}