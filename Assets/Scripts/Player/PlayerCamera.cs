using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Settings")]
    public float mouseSensitivity = 100f; // Reducido de 150 a 100 para menos sensibilidad
    public Transform playerBody;
    // Usamos un offset "plano" para que la rotación sea esférica perfecta alrededor del FocusPoint
    public Vector3 offset = new Vector3(0, 0, -3f);
    
    [Header("Camera Rotation Limits")]
    [Tooltip("Ángulo mínimo de rotación vertical (mirar hacia abajo). Valor negativo.")]
    public float minPitch = -40f; // Mirar hacia abajo (típico: -30 a -45)
    [Tooltip("Ángulo máximo de rotación vertical (mirar hacia arriba). Valor positivo.")]
    public float maxPitch = 60f;  // Mirar hacia arriba (típico: 50 a 70) 

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
        pitch = NormalizeAngle(angles.x);
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

        // CAMBIO IMPORTANTE: El mouse horizontal rota al JUGADOR, no la cámara
        // Rotar el cuerpo del jugador con el mouse horizontal
        playerBody.Rotate(Vector3.up * mouseX);
        
        // El yaw ahora viene del jugador
        yaw = playerBody.eulerAngles.y;
        
        // Solo el pitch (vertical) se controla con el mouse
        pitch -= mouseY;
        // Normalizar el ángulo para mantenerlo en el rango -180 a 180
        pitch = NormalizeAngle(pitch);
        // Limitar rotación vertical para evitar que la cámara se voltee
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); 

        // Rotación deseada (combina el yaw del jugador con el pitch de la cámara)
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

    /// <summary>
    /// Normaliza un ángulo para que esté en el rango de -180 a 180 grados.
    /// Esto previene problemas cuando los ángulos se acumulan más allá de 360 grados.
    /// </summary>
    private float NormalizeAngle(float angle)
    {
        while (angle > 180f)
            angle -= 360f;
        while (angle < -180f)
            angle += 360f;
        return angle;
    }
}
