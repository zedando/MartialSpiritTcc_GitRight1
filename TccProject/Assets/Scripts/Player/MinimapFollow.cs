using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 20f, 0); // altura da c�mera

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position + offset;
            transform.position = newPosition;

            // Garante que a c�mera esteja sempre olhando para baixo
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
