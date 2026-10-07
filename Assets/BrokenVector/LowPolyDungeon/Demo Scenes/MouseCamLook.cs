using UnityEngine;

public class MouseCamLook : MonoBehaviour
{
    public Transform playerBody;
    public Vector3 shoulderOffset = new Vector3(0.5f, 1.6f, 0f); // نقطة ينظر إليها فوق الكتف
    public float distance = 2.5f;       // البعد عن اللاعب
    public float mouseSensitivity = 150f;
    public float minPitch = -20f;
    public float maxPitch = 60f;
    public float collisionRadius = 0.25f;
    public LayerMask collisionMask = ~0; // كل الطبقات

    float yaw;
    float pitch = 15f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yaw = playerBody.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (PauseMenu.GameIsPaused) return;

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        // النقطة اللي بتطلع عليها الكاميرا (فوق كتف اللاعب)
        Vector3 focus = playerBody.position + rot * Vector3.right * shoulderOffset.x
                        + Vector3.up * shoulderOffset.y;

        Vector3 desiredPos = focus - rot * Vector3.forward * distance;

        // منع الكاميرا تدخل بالجدران
        Vector3 dir = desiredPos - focus;
        if (Physics.SphereCast(focus, collisionRadius, dir.normalized, out RaycastHit hit,
                               dir.magnitude, collisionMask, QueryTriggerInteraction.Ignore))
        {
            desiredPos = focus + dir.normalized * Mathf.Max(hit.distance, 0.3f);
        }

        transform.position = desiredPos;
        transform.rotation = rot;
    }
}