using UnityEngine;

/// <summary>
/// Control de cámara en tercera persona.
/// El mouse horizontal rota al jugador, el mouse vertical mueve la cámara arriba/abajo.
/// Basado en MouseLookController del proyecto anterior.
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    // ===================================
    // CONFIGURACIÓN
    // ===================================
    
    [Header("Sensibilidad")]
    [Tooltip("Sensibilidad del mouse (menor = más lento, mayor = más rápido)")]
    [Range(50f, 300f)]
    public float mouseSensitivity = 100f;
    
    [Header("Límites de Rotación Vertical")]
    [Tooltip("Ángulo máximo para mirar hacia arriba (positivo)")]
    [Range(0f, 90f)]
    public float upLimit = 60f;
    
    [Tooltip("Ángulo máximo para mirar hacia abajo (negativo)")]
    [Range(-90f, 0f)]
    public float downLimit = -40f;
    
    [Header("Referencias")]
    [Tooltip("El cuerpo del jugador que rotará horizontalmente")]
    public Transform playerBody;
    
    [Tooltip("Offset de la cámara respecto al jugador")]
    public Vector3 cameraOffset = new Vector3(0, 1.5f, -3f);
    
    [Header("Colisión de Cámara")]
    [Tooltip("Capas con las que la cámara puede colisionar")]
    public LayerMask collisionLayers = ~0;
    
    [Tooltip("Radio de la esfera de colisión de la cámara")]
    public float cameraRadius = 0.2f;
    
    // ===================================
    // CONTROL DE ESTADO
    // ===================================
    
    [Header("Control de Estado")]
    [SerializeField] private bool _controlsActive = true;
    [SerializeField] private bool _isCameraLocked = false;
    
    // Properties públicas
    public bool ControlsActive => _controlsActive;
    public bool IsCameraLocked => _isCameraLocked;
    
    // ===================================
    // VARIABLES PRIVADAS
    // ===================================
    
    private float pitch = 0f; // Rotación vertical (arriba/abajo)
    private Vector3 focusPoint; // Punto al que mira la cámara

    // ===================================
    // INICIALIZACIÓN
    // ===================================
    
    void Start()
    {
        // Buscar playerBody si no está asignado
        if (playerBody == null)
        {
            // Intentar encontrar el objeto del jugador
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerBody = player.transform;
                Debug.Log($"[CAMERA] ✅ PlayerBody encontrado: {playerBody.name}");
            }
            else
            {
                Debug.LogError("[CAMERA] ❌ No se encontró PlayerBody. Asigna manualmente en el Inspector.");
            }
        }
        
        // Configurar cursor inicial
        SetControlsActive(_controlsActive);
        
        // Inicializar pitch desde la rotación actual de la cámara
        pitch = transform.eulerAngles.x;
        if (pitch > 180f) pitch -= 360f;
        
        Debug.Log("[CAMERA] ✅ PlayerCamera inicializada");
    }

    // ===================================
    // UPDATE
    // ===================================
    
    void LateUpdate()
    {
        // Bloqueo de controles
        if (!_controlsActive || _isCameraLocked || playerBody == null) return;
        
        // Obtener input del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        // HORIZONTAL: Rotar el cuerpo del jugador
        playerBody.Rotate(Vector3.up * mouseX);
        
        // VERTICAL: Rotar la cámara arriba/abajo
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, downLimit, upLimit);
        
        // Calcular posición de la cámara
        UpdateCameraPosition();
    }

    // ===================================
    // POSICIONAMIENTO DE CÁMARA
    // ===================================
    
    private void UpdateCameraPosition()
    {
        // Punto focal (cabeza/cuello del jugador)
        focusPoint = playerBody.position + Vector3.up * 1.5f;
        
        // Calcular rotación de la cámara
        Quaternion rotation = Quaternion.Euler(pitch, playerBody.eulerAngles.y, 0f);
        
        // Posición deseada de la cámara
        Vector3 desiredPosition = focusPoint + rotation * cameraOffset;
        
        // Detección de colisiones
        Vector3 direction = (desiredPosition - focusPoint).normalized;
        float distance = Vector3.Distance(focusPoint, desiredPosition);
        
        RaycastHit hit;
        if (Physics.SphereCast(focusPoint, cameraRadius, direction, out hit, distance, collisionLayers))
        {
            // Si hay colisión, acercar la cámara
            transform.position = focusPoint + direction * (hit.distance - 0.2f);
        }
        else
        {
            // Sin colisión, usar posición deseada
            transform.position = desiredPosition;
        }
        
        // Mirar siempre al punto focal
        transform.LookAt(focusPoint);
    }

    // ===================================
    // FUNCIONES PÚBLICAS DE CONTROL
    // ===================================
    
    /// <summary>
    /// Activa o desactiva el control de la cámara.
    /// Usado para pausas, menús, etc.
    /// </summary>
    public void SetControlsActive(bool active)
    {
        _controlsActive = active;
        
        if (active)
        {
            // MODO JUEGO: Bloquear cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("[CAMERA] 🎮 Controles activados");
        }
        else
        {
            // MODO PAUSA: Liberar cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("[CAMERA] ⏸️ Controles desactivados");
        }
    }
    
    /// <summary>
    /// Bloquea o desbloquea la rotación de la cámara.
    /// Usado para menús flotantes donde el juego sigue corriendo.
    /// </summary>
    public void SetLockState(bool isLocked)
    {
        _isCameraLocked = isLocked;
        
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("[CAMERA] 🔒 Cámara bloqueada");
        }
        else
        {
            // Solo bloquear cursor si los controles están activos
            if (_controlsActive)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("[CAMERA] 🔓 Cámara desbloqueada");
            }
        }
    }
    
    /// <summary>
    /// Ajusta la sensibilidad del mouse en tiempo de ejecución.
    /// </summary>
    public void SetSensitivity(float newSensitivity)
    {
        mouseSensitivity = Mathf.Clamp(newSensitivity, 50f, 300f);
        Debug.Log($"[CAMERA] 🎯 Sensibilidad ajustada a: {mouseSensitivity}");
    }

    // ===================================
    // GIZMOS (Para debugging en el editor)
    // ===================================
    
    void OnDrawGizmosSelected()
    {
        if (playerBody == null) return;
        
        // Dibujar punto focal
        Vector3 focus = playerBody.position + Vector3.up * 1.5f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(focus, 0.1f);
        
        // Dibujar línea de la cámara al foco
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, focus);
    }
}
