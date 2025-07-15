using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMoviment : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f; // força da gravidade
    public Transform cameraTransform; // Arraste a Main Camera ou CameraRig aqui

    private CharacterController controller;
    private Animator animator;

    private PlayerInput controls;
    private Vector2 moveInput;
    private float verticalVelocity; // velocidade no eixo Y

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
        {
            verticalVelocity = -2f; // manter o personagem no chão
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Movimento no plano XZ
        Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        Vector3 moveDir = Vector3.zero;

        if (inputDirection.magnitude >= 0.1f)
        {
            moveDir = cameraTransform.TransformDirection(inputDirection);
            moveDir.y = 0f;
            moveDir.Normalize();

            // Faz o player olhar na direção do movimento
            transform.forward = moveDir;
        }

        // Combina o movimento horizontal com a gravidade
        Vector3 finalVelocity = moveDir * speed + Vector3.up * verticalVelocity;
        controller.Move(finalVelocity * Time.deltaTime);

        if (inputDirection.magnitude > 0f)
        {
            animator.SetBool("Andando", true);
            animator.SetBool("Tenis", false);
        
        }
        if (inputDirection.magnitude <= 0f)
        {
            animator.SetBool("Andando", false);
        }

    }
    public void TakeShoes()
    {
        animator.SetBool("Tenis", true);
    }
     
}
