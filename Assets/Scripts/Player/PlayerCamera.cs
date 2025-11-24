using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Settings")]
    public float mouseSensitivity = 150f;
    public Transform playerBody;
    public Vector3 offset = new Vector3(0, 2, -3.5f); // Posición detrás del jugador

    [Header("Collision")]
    public LayerMask collisionLayers = ~0; // Capas con las que choca la cámara
    public float cameraRadius = 0.2f;

    private float pitch = 0f;
    private float yaw = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Inicializar ángulos
        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
    }

    void LateUpdate()
    {
        if (!playerBody) return;

        // Input del Mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // Limitar mirar arriba/abajo

        // Rotación deseada
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Posición deseada (Orbitando al jugador)
        // Focus Point: Miramos a la cabeza del jugador (aprox 1.5m arriba)
        Vector3 focusPoint = playerBody.position + Vector3.up * 1.5f;
        Vector3 desiredPos = focusPoint + rotation * offset;

        // Detección de colisiones (Para que la cámara no atraviese paredes)
        Vector3 direction = (desiredPos - focusPoint).normalized;
        float distance = Vector3.Distance(focusPoint, desiredPos);

        if (Physics.SphereCast(focusPoint, cameraRadius, direction, out RaycastHit hit, distance, collisionLayers))
        {
            // Si chocamos, ponemos la cámara en el punto de choque (un poco antes)
            transform.position = focusPoint + direction * (hit.distance - 0.1f);
        }
        else
        {
            transform.position = desiredPos;
        }

        // Mirar siempre al punto de foco
        transform.LookAt(focusPoint);
    }
}
