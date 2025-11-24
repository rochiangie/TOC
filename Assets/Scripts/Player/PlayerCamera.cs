using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Settings")]
    public float mouseSensitivity = 150f;
    public Transform playerBody;
    // Usamos un offset "plano" para que la rotación sea esférica perfecta alrededor del FocusPoint
    public Vector3 offset = new Vector3(0, 0, -3f); 

    [Header("Collision")]
    public LayerMask collisionLayers = ~0;
    public float cameraRadius = 0.1f; // Radio reducido para acercarse más al suelo/paredes

    private float pitch = 0f;
    private float yaw = 0f;

    void Awake()
    {
        // 🚨 DIAGNÓSTICO DE ERROR DE USUARIO 🚨
        if (GetComponent<PlayerMovement>() != null)
        {
            Debug.LogError("❌❌❌ ¡ERROR FATAL! ❌❌❌\n" +
                           "Has puesto el script 'PlayerCamera' en el JUGADOR.\n" +
                           "Este script debe ir en la MAIN CAMERA.\n" +
                           "El script se autodestruirá para evitar que salgas volando.");
            Destroy(this);
            return;
        }

        // 🚨 AUTO-FIX CRÍTICO: Eliminar Collider y Rigidbody
        Collider camCol = GetComponent<Collider>();
        if (camCol != null) Destroy(camCol);

        Rigidbody camRb = GetComponent<Rigidbody>();
        if (camRb != null) Destroy(camRb);

        // 🚨 AUTO-FIX: Desactivar CinemachineBrain si existe
        MonoBehaviour brain = GetComponent("CinemachineBrain") as MonoBehaviour;
        if (brain != null && brain.enabled)
        {
            brain.enabled = false;
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 🚨 AUTO-CONFIG: Excluir al Player de la colisión de la cámara
        if (playerBody != null)
        {
            int playerLayer = playerBody.gameObject.layer;
            collisionLayers &= ~(1 << playerLayer);
        }

        // Inicializar ángulos
        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
    }

    void Update()
    {
        // Asegurar que el cursor se bloquee si hacemos clic
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        if (!playerBody) return;

        // Input del Mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        // Rango ampliado casi al máximo vertical (-85 a 85 grados)
        pitch = Mathf.Clamp(pitch, -85f, 85f); 

        // Rotación deseada
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Posición deseada (Orbitando al jugador)
        // Focus Point: El pivote central (la cabeza/cuello del jugador)
        Vector3 focusPoint = playerBody.position + Vector3.up * 1.5f;
        
        // Al rotar un vector (0,0,-Z), obtenemos una órbita perfecta
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
