using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using echo17.EndlessBook;
using System.Collections;
public class GD_PlayerInteract : MonoBehaviour
{
    [Header("Interação")]
    public float InteractionDistance = 2f;
    public GameObject interactionText;
    [SerializeField] GD_InteractObject currentInteractable;
    private List<GD_InteractObject> interactablesInRange = new List<GD_InteractObject>();

    [Header("Combate")]
    public Animator animator;
    private List<GameObject> enemiesInRange = new List<GameObject>();

    [Header("Cooldowns")]
    public float punchCooldown = 0.5f;
    public float kickCooldown = 0.7f;
    private float nextPunchTime = 0f;
    private float nextKickTime = 0f;

    [Header("Movimento")]
    public float moveThreshold = 0.1f;

    [Header("Defesa")]
    public bool isDefending = false;

    [Header("Vida do Player")]
    public int playerLives = 3;
    public TextMeshProUGUI playerLivesText; // Referência no Canvas

    void Start()
    {
        if (interactionText != null) interactionText.SetActive(false);
        UpdatePlayerLivesUI();
    }

    void Update()
    {
        // Interação (E)
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            Interact();
        }

        // Defesa (L no teclado)
        HandleDefense();

        // Ataques só se NÃO estiver defendendo
        if (!isDefending)
        {
            if (Input.GetKeyDown(KeyCode.J) && Time.time >= nextPunchTime)
                AttackPunch();

            if (Input.GetKeyDown(KeyCode.K) && Time.time >= nextKickTime)
                AttackKick();
        }

        HandleMovement();
    }

    // ---- Defesa ----
    private void HandleDefense()
    {
        if (Input.GetKeyDown(KeyCode.L))
            StartDefense();

        if (Input.GetKeyUp(KeyCode.L))
            StopDefense();
    }

    public void StartDefense()
    {
        isDefending = true;
        if (animator != null)
            animator.SetBool("Defendendo", true);
    }

    public void StopDefense()
    {
        isDefending = false;
        if (animator != null)
            animator.SetBool("Defendendo", false);
    }

    // ---- Movimento ----
    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > moveThreshold || Mathf.Abs(vertical) > moveThreshold)
        {
            if (animator != null)
            {
                animator.ResetTrigger("Oi-zuki");
                animator.ResetTrigger("MaeGeri");
                animator.SetBool("Andando", true);
            }
        }
        else if (animator != null)
        {
            animator.SetBool("Andando", false);
        }
    }

    // ---- Ataques ----
    public void AttackPunch()
    {
        if (animator != null) animator.SetTrigger("Oi-zuki");
        nextPunchTime = Time.time + punchCooldown;

        if (enemiesInRange.Count > 0)
        {
            foreach (GameObject enemy in enemiesInRange)
            {
                GD_Enemy enemyScript = enemy.GetComponent<GD_Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(1);
                    break;
                }
            }
        }
    }

    public void AttackKick()
    {
        if (animator != null) animator.SetTrigger("MaeGeri");
        nextKickTime = Time.time + kickCooldown;

        if (enemiesInRange.Count > 0)
        {
            foreach (GameObject enemy in enemiesInRange)
            {
                GD_Enemy enemyScript = enemy.GetComponent<GD_Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(1);
                    break;
                }
            }
        }
    }

    // ---- Interação ----
    private void OnTriggerEnter(Collider other)
    {
        GD_InteractObject interactableObject = other.GetComponent<GD_InteractObject>();
        if (interactableObject != null)
        {
            interactablesInRange.Add(interactableObject);
            if (currentInteractable == null)
            {
                SetCurrentInteractable(interactableObject);
                if (other.gameObject.CompareTag("NPC"))
                {
                    interactableObject.GetStartDialog();
                }
            }
        }

        if (other.CompareTag("Enemy") && !enemiesInRange.Contains(other.gameObject))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GD_InteractObject interactableObject = other.GetComponent<GD_InteractObject>();
        if (interactableObject != null)
        {
            interactablesInRange.Remove(interactableObject);
            if (interactableObject == currentInteractable)
            {
                RemoveCurrentInteractable();
            }
        }

        if (other.CompareTag("Enemy") && enemiesInRange.Contains(other.gameObject))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    private void SetCurrentInteractable(GD_InteractObject interactable)
    {
        currentInteractable = interactable;
        ShowInteractionText();
    }

    private void RemoveCurrentInteractable()
    {
        currentInteractable = null;
        interactionText.SetActive(false);
    }

    private void ShowInteractionText()
    {
        if (interactionText != null && currentInteractable != null)
        {
            interactionText.SetActive(true);
            TextMeshProUGUI textComponent = interactionText.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = currentInteractable.GetInteractionText();
            }
        }
        else if (interactionText != null)
        {
            interactionText.SetActive(false);
        }
    }

    public void Interact()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
            RemoveCurrentInteractable();
        }
    }

    // ---- Vida ----
    public void TakeDamage(int damage)
    {
        if (isDefending)
        {
            Debug.Log("Player defendeu o ataque!");
            return;
        }

        playerLives -= damage;
        if (playerLives < 0) playerLives = 0;

        UpdatePlayerLivesUI();

        if (playerLives <= 0)
        {
            Debug.Log("Player morreu!");
            // aqui pode reiniciar cena ou game over
            SceneManager.LoadScene("FightMiniGame");
        }
    }

    private void UpdatePlayerLivesUI()
    {
        if (playerLivesText != null)
        {
            playerLivesText.text = "Haruki: " + playerLives;
        }
    }
}