using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class PlayerMoviment : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;
    private PlayerInput controls;
    private Vector2 moveInput;
    private float verticalVelocity;

    [Header("Troca de Material ao pegar o tênis")]
    public Renderer objectToChangeMaterial;   // objeto cujo material será trocado
    public Material newMaterial;              // novo material

    [Header("FMOD")]
    public string footstepEventPath = "event:/ambiente/madeiraa";
    private float stepTimer = 0f;
    public float stepInterval = 0.4f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        controls = new PlayerInput();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        // Gravidade
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        Vector3 moveDir = Vector3.zero;

        if (inputDirection.magnitude >= 0.1f)
        {
            moveDir = cameraTransform.TransformDirection(inputDirection);
            moveDir.y = 0f;
            moveDir.Normalize();
            transform.forward = moveDir;

            animator.SetBool("Andando", true);
            animator.SetBool("Tenis", false);

            // Som de passos
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                RuntimeManager.PlayOneShot(footstepEventPath, transform.position);
                stepTimer = 0f;
            }
        }
        else
        {
            animator.SetBool("Andando", false);
            stepTimer = stepInterval;
        }

        Vector3 finalVelocity = moveDir * speed + Vector3.up * verticalVelocity;
        controller.Move(finalVelocity * Time.deltaTime);
    }

    public void TakeShoes()
    {
        animator.SetBool("Tenis", true);

        // CHAMA A TROCA DE MATERIAL AQUI
        ChangeMaterial();
    }

    private void ChangeMaterial()
    {
        if (objectToChangeMaterial != null && newMaterial != null)
        {
            objectToChangeMaterial.material = newMaterial;
        }
        else
        {
            Debug.LogWarning("PlayerMoviment: objeto ou material não atribuídos no Inspector!");
        }
    }
}
