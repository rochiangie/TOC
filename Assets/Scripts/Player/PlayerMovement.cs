using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // === Movimiento y Salto ===
    [Header("Movimiento (WASD)")]
    public float moveSpeed = 5f;
    // 📢 NUEVO: Velocidad al correr/sprintar
    public float runSpeed = 10f;
    public float turnSmoothTime = 0.1f; // Suavizado de rotación

    [Header("Sprint")]
    // 📢 NUEVO: Tecla para correr
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Salto")]
    public float jumpForce = 6f;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Animación")]
    public Animator animator;

    private readonly int JumpTriggerHash = Animator.StringToHash("Jump");
    private readonly int SpeedFloatHash = Animator.StringToHash("Speed");

    // === Ground Check ===
    [Header("Ground Check (opcional)")]
    public Transform groundCheck;
    public float groundRadius = 0.1f; // Radio reducido para evitar colisión propia
    public LayerMask groundMask = ~0;

    // === Variables privadas ===
    Rigidbody rb;
    bool isGrounded;
    bool jumpScheduled = false;
    // 📢 NUEVO: Estado de sprint
    bool isRunning = false;
    float turnSmoothVelocity;
    Transform cameraTransform;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // 🚨 DIAGNÓSTICO DE CÁMARA 🚨
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;

            // 1. Advertencia de Jerarquía (Causa que el personaje gire loco)
            if (cameraTransform.parent == transform)
            {
                Debug.LogError("🚨 ¡ERROR CRÍTICO! La 'Main Camera' es hija del Player. Esto hace que la cámara gire con el personaje. ¡SÁCALA del Player en la Jerarquía! (Arrastra 'Main Camera' al espacio vacío).");
            }

            // 2. Advertencia de Collider (Causa que el personaje salga volando)
            if (cameraTransform.GetComponent<Collider>() != null)
            {
                Debug.LogError("🚨 ¡ERROR CRÍTICO! La cámara tiene un Collider. Esto empuja al jugador. ¡Elimina el componente Collider de la cámara!");
            }
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (!groundCheck)
        {
            var gc = new GameObject("GroundCheck").transform;
            gc.SetParent(transform);
            // Posición segura debajo de los pies para evitar colisión con el propio jugador
            // Asumiendo que el pivote del jugador está en los pies (Y=0)
            gc.localPosition = new Vector3(0f, -0.15f, 0f); 
            groundCheck = gc;
        }
    }

    void Update()
    {
        // 1. Detección de suelo mejorada
        // Usamos una máscara que intente ignorar al jugador si está en la misma capa
        int layerMask = groundMask;
        if (groundMask == ~0) // Si es "Everything"
        {
             // Intentamos ignorar la capa del jugador
             layerMask = ~(1 << gameObject.layer);
        }
        
        // Usamos QueryTriggerInteraction.Ignore para no saltar sobre triggers invisibles
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, layerMask, QueryTriggerInteraction.Ignore);

        // 2. Manejo de Input de salto
        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            jumpScheduled = true;
        }

        // 📢 NUEVO: Manejo de Input para correr
        isRunning = Input.GetKey(runKey);
    }

    void FixedUpdate()
    {
        // === Lógica de Salto y Animación ===
        if (jumpScheduled)
        {
            // Usamos rb.linearVelocity (Unity 6) o rb.velocity (Unity 2022)
            // Si tienes errores aquí, cambia 'linearVelocity' por 'velocity'
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (animator != null)
            {
                animator.SetTrigger(JumpTriggerHash);
            }

            jumpScheduled = false;
        }

        // === Lógica de Movimiento: Relativa a la CÁMARA ===

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(x, 0f, z).normalized;

        float currentMaxSpeed = isRunning ? runSpeed : moveSpeed;
        Vector3 targetVelocity = Vector3.zero;

        // Si hay movimiento y tenemos cámara, nos movemos relativo a ella
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            
            if (cameraTransform != null)
            {
                targetAngle += cameraTransform.eulerAngles.y;
            }

            // Rotar suavemente al personaje hacia esa dirección
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Calcular dirección de movimiento real
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            targetVelocity = moveDir.normalized * currentMaxSpeed;
        }
        else
        {
            currentMaxSpeed = 0f;
            isRunning = false;
        }

        // --- Aplicar Velocidad ---
        Vector3 v = rb.linearVelocity; // Si error, cambiar a rb.velocity
        Vector3 vertical = Vector3.up * v.y; // Mantener velocidad vertical (gravedad/salto)

        // Interpolación suave para el movimiento horizontal
        Vector3 currentHoriz = new Vector3(v.x, 0f, v.z);
        float accel = 15f; // Aceleración
        Vector3 newHoriz = Vector3.Lerp(currentHoriz, targetVelocity, accel * Time.fixedDeltaTime);

        rb.linearVelocity = newHoriz + vertical; // Si error, cambiar a rb.velocity

        // 3) Actualizar Animación
        if (animator != null)
        {
            animator.SetFloat(SpeedFloatHash, newHoriz.magnitude);
        }
    }

    // Opcional: Para depuración
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}
