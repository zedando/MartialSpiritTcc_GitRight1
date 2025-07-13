using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Camera playerCamera; // Apenas como referência para direção da movimentação
    public Transform player;            // Referência ao jogador
    public float smoothSpeed = 0.02f;   // Velocidade de suavização, ajuste para mais ou menos suave
    public Vector3 offset;
    private Vector3 currentVelocity = Vector3.zero;

    void Update()
    {
        // Recebe input do teclado
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Vetor normalizado do movimento
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // Movimento relativo à orientação da câmera fixa (sem rotação no mouse)
            Vector3 moveDir = playerCamera.transform.TransformDirection(moveDirection);
            moveDir.y = 0; // mantém no plano horizontal
            
            transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
        }
    }
    
}

