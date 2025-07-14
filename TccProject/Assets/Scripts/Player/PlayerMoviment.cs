using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoviment : MonoBehaviour
{
    public float speed = 5f;
    public Transform cameraTransform; // Arraste a Main Camera ou CameraRig aqui

    private CharacterController controller;
    private PlayerInput controls;
    private Vector2 moveInput;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerInput();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            Vector3 moveDir = cameraTransform.TransformDirection(inputDirection);
            moveDir.y = 0f; // impede subir ou descer
            moveDir.Normalize();

            controller.Move(moveDir * speed * Time.deltaTime);

            // Faz o player olhar na direção do movimento
            transform.forward = moveDir;
        }
    }
}
