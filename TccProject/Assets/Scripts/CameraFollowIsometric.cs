using UnityEngine;

public class CameraFollowIsometric : MonoBehaviour
{
    public Transform player;            // Referência ao jogador
    public float smoothSpeed = 0.02f;   // Velocidade de suavização, ajuste para mais ou menos suave
    public Vector3 offset;              // Deslocamento da câmera em relação ao jogador (ex: posição fixa no céu)
    private Vector3 currentVelocity = Vector3.zero;

    private void LateUpdate()
    {
        if (player == null) return;

        // Alvo horizontal da câmera (posição do jogador + offset)
        Vector3 targetPosition = new Vector3(player.position.x + offset.x, player.position.y + offset.y, player.position.z + offset.z);

        // Interpolação suave da posição atual para a posição alvo (somente horizontal)
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothSpeed);
    }
}
