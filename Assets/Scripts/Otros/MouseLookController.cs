using UnityEngine;

public class MouseLookController : MonoBehaviour
{
    // === Configuración ===
    [Header("Sensibilidad")]
    public float mouseSensitivity = 200f;

    [Header("Límites de Rotación Vertical")]
    public float upLimit = 85f;
    public float downLimit = -85f;

    [Header("Referencias")]
    [Tooltip("El objeto que recibirá la rotación vertical (Generalmente la cámara).")]
    public Transform headLookTarget;

    // === Control de Estado ===
    [Header("Control de Estado")]
    [Tooltip("La variable privada que almacena si los controles de juego (movimiento) están activos.")]
    [SerializeField] private bool _controlsActive = true;

    [Tooltip("La variable que bloquea la cámara por UI (e.g., panel de Tools).")]
    [SerializeField] private bool _isCameraLocked = false; 

    // 📢 PROPIEDAD PÚBLICA: Permite que PlayerMovement y otros scripts lean el estado sin errores.
    public bool ControlsActive
    {
        get { return _controlsActive; }
    }

    // 📢 NUEVA PROPIEDAD PÚBLICA para el estado de bloqueo de la cámara.
    public bool IsCameraLocked
    {
        get { return _isCameraLocked; }
    }

    // === Variables privadas ===
    private float rotationX = 0f;
    private bool hasLoggedError = false;

    // ================== Unity Lifecycle ==================

    void Start()
    {
        // El control inicial es determinado por '_controlsActive'
        SetControlsActive(_controlsActive);
    }

    void Update()
    {
        // 🛑 LÓGICA DE BLOQUEO CRÍTICO 🛑
        if (!_controlsActive || _isCameraLocked) return;

        // La rotación del personaje y la cámara ahora son manejadas por:
        // 1. PlayerMovement (Rotación del personaje hacia el movimiento)
        // 2. Cinemachine (Rotación de la cámara)
        
        // Este script solo gestiona el cursor.
    }

    // ================== Funciones de Comunicación y Control ==================

    /// <summary>
    /// Activa o desactiva el control de la cámara/cabeza (Usado por UIPauseController para PAUSA/DECISIÓN).
    /// También gestiona el cursor, asumiendo un estado de pausa total.
    /// </summary>
    public void SetControlsActive(bool active)
    {
        _controlsActive = active; // ⬅️ Actualiza la variable interna

        if (active)
        {
            // MODO JUEGO (General): Reactivar el control y bloquear el cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (headLookTarget == null)
            {
                TryAssignHeadTarget();
            }
        }
        else
        {
            // MODO PAUSA/MENÚ: Desactivar el control y liberar el cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Bloquea o desbloquea la rotación de la cámara, usado específicamente para Menús Flotantes (Tools)
    /// donde el juego sigue corriendo pero la cámara debe estar fija.
    /// </summary>
    public void SetLockState(bool isLocked)
    {
        _isCameraLocked = isLocked;

        // El MouseLookController también puede encargarse de la visibilidad del cursor para este caso
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Solo si el control de juego general está ACTIVO, volvemos a bloquear el cursor.
            if (_controlsActive)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }


    /// <summary>
    /// Función llamada por HeadLookRegistrar.cs para asignar la referencia de la cabeza.
    /// </summary>
    public void SetHeadTarget(Transform head)
    {
        if (head != null && headLookTarget == null)
        {
            headLookTarget = head;
            Debug.Log($"[MouseLook] ¡ASIGNACIÓN ÉXITO! Head Target asignado por SetHeadTarget a: {head.name}");

            rotationX = headLookTarget.localEulerAngles.x;
            if (rotationX > 180f) rotationX -= 360f;

            hasLoggedError = false;
        }
    }

    // ================== Fallback de Asignación ==================

    private const string HeadObjectName = "Head";

    private void TryAssignHeadTarget()
    {
        if (headLookTarget != null) return;

        Transform foundHead = transform.Find(HeadObjectName);

        if (foundHead != null)
        {
            SetHeadTarget(foundHead);
            return;
        }

        if (headLookTarget == null && hasLoggedError == false)
        {
            Debug.LogError($"[MouseLook] ¡Advertencia! No se encontró el objeto llamado '{HeadObjectName}'. Verifique que el HeadLookRegistrar está adjunto a la cabeza.");
            hasLoggedError = true;
        }
    }
}