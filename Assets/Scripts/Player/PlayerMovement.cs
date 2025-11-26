using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // === Movimiento y Salto ===
    [Header("Movimiento (WASD)")]
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float turnSmoothTime = 0.1f;

    [Header("Sprint")]
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Salto")]
    public float jumpForce = 6f;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpCooldown = 0.5f; // Evitar saltos dobles accidentales

    [Header("Animación")]
    public Animator animator;

    private readonly int JumpTriggerHash = Animator.StringToHash("Jump");
    private readonly int SpeedFloatHash = Animator.StringToHash("Speed");

    // === Ground Check (Raycast) ===
    [Header("Ground Check")]
    public float rayLength = 1.1f; // Un poco más que la mitad de la altura (si altura es 2)
    public LayerMask groundMask = ~0;

    // === Variables privadas ===
    Rigidbody rb;
    bool isGrounded;
    bool jumpScheduled = false;
    bool isRunning = false;
    float turnSmoothVelocity;
    Transform cameraTransform;
    float lastJumpTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Evitar atravesar suelo

        // 🚨 AUTO-CORRECCIÓN: Eliminar CharacterController si existe para evitar conflictos
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            Debug.LogWarning("⚠️ Se detectó un CharacterController. Eliminándolo para usar Rigidbody...");
            Destroy(cc);
        }

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            // Advertencia si la cámara es hija
            if (cameraTransform.parent == transform)
            {
                Debug.LogError("🚨 LA CÁMARA ES HIJA DEL PLAYER. Por favor, desvincúlala (Unparent) en la jerarquía.");
            }
        }

        if (animator == null) animator = GetComponent<Animator>();
    }

    void Start()
    {
        // 🚨 SEGURIDAD: Detener cualquier inercia inicial loca
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        // 1. Detección de suelo por RAYCAST (Más preciso que Sphere)
        // Lanzamos un rayo desde el centro del personaje hacia abajo
        // Asumiendo pivote en los pies (0,0,0) o centro (0,1,0). 
        // Ajustamos el origen para que empiece un poco arriba de los pies.
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        
        // Ignorar al propio jugador
        int layerMask = groundMask;
        if (groundMask == ~0) layerMask = ~(1 << gameObject.layer);

        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, 0.6f, layerMask); // 0.5 (offset) + 0.1 (margen)

        // Debug Ray
        Debug.DrawRay(rayOrigin, Vector3.down * 0.6f, isGrounded ? Color.green : Color.red);

        // 2. Salto
        if (isGrounded && Input.GetKeyDown(jumpKey) && Time.time > lastJumpTime + jumpCooldown)
        {
            jumpScheduled = true;
            lastJumpTime = Time.time;
        }

        isRunning = Input.GetKey(runKey);
    }

    [Header("Configuración de Rotación")]
    public bool rotateWithCamera = false; // Ahora el mouse rota al jugador directamente (controlado por PlayerCamera)

    // ... (resto de variables)

    void FixedUpdate()
    {
        // === Salto ===
        if (jumpScheduled)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = 0;
            rb.linearVelocity = vel;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (animator != null) animator.SetTrigger(JumpTriggerHash);
            jumpScheduled = false;
        }

        // === Movimiento ===
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        float currentMaxSpeed = isRunning ? runSpeed : moveSpeed;
        Vector3 targetVelocity = Vector3.zero;

        if (rotateWithCamera && cameraTransform != null)
        {
            // MODO 1: STRAFING (El personaje gira con la cámara)
            // Rotamos el cuerpo para que coincida con la cámara
            float cameraYaw = cameraTransform.eulerAngles.y;
            float smoothYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, cameraYaw, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothYaw, 0f);

            // El movimiento es relativo al cuerpo (que ya mira a la cámara)
            // W = Adelante, S = Atrás, A = Izquierda, D = Derecha
            Vector3 moveDir = transform.right * x + transform.forward * z;
            
            // Solo nos movemos si hay input
            if (moveDir.magnitude >= 0.1f)
            {
                targetVelocity = moveDir.normalized * currentMaxSpeed;
            }
        }
        else
        {
            // MODO 2: ADVENTURE (El personaje mira hacia donde camina)
            Vector3 direction = new Vector3(x, 0f, z).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                if (cameraTransform != null) targetAngle += cameraTransform.eulerAngles.y;

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                targetVelocity = moveDir.normalized * currentMaxSpeed;
            }
        }

        // === Aplicar Velocidad (Preservando Gravedad) ===
        Vector3 currentVel = rb.linearVelocity;
        Vector3 targetVelHoriz = new Vector3(targetVelocity.x, 0, targetVelocity.z);
        Vector3 currentVelHoriz = new Vector3(currentVel.x, 0, currentVel.z);

        // Aceleración suave
        Vector3 newVelHoriz = Vector3.Lerp(currentVelHoriz, targetVelHoriz, 15f * Time.fixedDeltaTime);
        
        Vector3 finalVel = new Vector3(newVelHoriz.x, currentVel.y, newVelHoriz.z);
        if (finalVel.y > 20f) finalVel.y = 20f;

        rb.linearVelocity = finalVel;

        // Animación
        if (animator != null)
        {
            // Nota: Si usas Blend Trees para Strafing, aquí deberías pasar X y Z por separado.
            // Por ahora mantenemos "Speed" general.
            animator.SetFloat(SpeedFloatHash, newVelHoriz.magnitude);
        }
    }
}
