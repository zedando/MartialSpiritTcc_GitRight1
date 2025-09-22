using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections; // necessário para usar IEnumerator

public class GD_Enemy : MonoBehaviour
{
    [Header("Movimento")]
    public NavMeshAgent agent;
    public Transform player;
    public float attackRange = 2f;   // distância para começar a atacar
    public float attackCooldown = 3f;
    private float nextAttackTime = 0f;

    [Header("Vida do Inimigo")]
    public int enemyLives = 3;
    public TextMeshProUGUI enemyLivesText; // UI para mostrar a vida

    [Header("Animação")]
    public Animator animator;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        UpdateEnemyLivesUI();
    }

    private void Update()
    {
        if (player == null || enemyLives <= 0) return;

        // Seguir o player
        agent.SetDestination(player.position);

        // Verificar distância para ataque
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        // animação de ataque
        if (animator != null)
        {
            animator.SetBool("Atacar", true);
            StartCoroutine(ResetAttackAnimation());
        }

        GD_PlayerInteract playerScript = player.GetComponent<GD_PlayerInteract>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(1);
            Debug.Log("Inimigo atacou o player!");
        }

        nextAttackTime = Time.time + attackCooldown;
    }

    private IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(1f);
        if (animator != null)
        {
            animator.SetBool("Atacar", false);
        }
    }

    public void TakeDamage(int damage)
    {
        enemyLives -= damage;
        UpdateEnemyLivesUI();
        Debug.Log("Inimigo levou dano! Vidas restantes: " + enemyLives);

        if (enemyLives <= 0)
        {
            if (animator != null)
                animator.SetTrigger("Morrer"); // animação de morte

            Debug.Log("Inimigo morreu!");
            Destroy(gameObject, 2f); // espera 2s antes de sumir
        }
    }

    private void UpdateEnemyLivesUI()
    {
        if (enemyLivesText != null)
        {
            enemyLivesText.text = "Enemy HP: " + enemyLives;
        }
    }
}