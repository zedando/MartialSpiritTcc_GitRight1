using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   public Transform target;
    public Vector3 offset = new Vector3(-6.5f, 6.5f, 6.5f);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}