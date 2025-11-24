using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public LayerMask interactableLayer = ~0; // Por defecto detecta todo
    public KeyCode interactKey = KeyCode.E;
    
    [Header("References")]
    public Transform cameraTransform;
    public Transform holdPoint; // Asigna esto en el Inspector (un hijo del Player/Camera)
    public Animator animator;

    private PickupableObject currentHeldObject;

    private void Start()
    {
        // 1. Usar la Cámara Principal (Normal)
        if (cameraTransform == null)
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
                Debug.Log($"✅ PlayerInteraction: Usando Camera.main ('{cameraTransform.name}').");
            }
            else
            {
                Debug.LogError("❌ PlayerInteraction: No se encontró ninguna cámara etiquetada como 'MainCamera'.");
            }
        }

        // 2. Crear HoldPoint si no existe
        if (holdPoint == null)
        {
            GameObject hp = new GameObject("HoldPoint");
            // Si tenemos cámara, lo ponemos hijo de la cámara para que gire con ella
            hp.transform.SetParent(cameraTransform != null ? cameraTransform : transform);
            hp.transform.localPosition = new Vector3(0.5f, -0.5f, 1f); // Posición mano derecha aprox
            holdPoint = hp.transform;
            Debug.Log("🔧 PlayerInteraction: Se creó un 'HoldPoint' automático.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey) || Input.GetButtonDown("Fire1"))
        {
            if (currentHeldObject != null)
            {
                TryDropOrTrash();
            }
            else
            {
                TryPickUp();
            }
        }
    }

    private void TryPickUp()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        // DEBUG VISUAL: Dibuja una línea roja en la escena para ver hacia dónde apunta el rayo
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * interactionDistance, Color.red, 2f);

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionDistance, interactableLayer))
        {
            Debug.Log($"PlayerInteraction: Raycast golpeó a '{hit.collider.name}' (Tag: {hit.collider.tag})");

            // 1. Verificar si es un objeto Recogible (por Tag o Componente)
            if (hit.collider.CompareTag("Recogible") || hit.collider.GetComponent<PickupableObject>() != null)
            {
                PickupableObject pickup = hit.collider.GetComponent<PickupableObject>();
                if (pickup != null)
                {
                    currentHeldObject = pickup;
                    currentHeldObject.OnPickUp(holdPoint);
                    
                    if (animator != null) animator.SetTrigger("PickUp");
                    return;
                }
            }

            // 2. Verificar si es un objeto Interactuable (como el Basurero para abrir/cerrar)
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null) interactable = hit.collider.GetComponentInChildren<IInteractable>(); // Buscar en hijos
            if (interactable == null) interactable = hit.collider.GetComponentInParent<IInteractable>();   // Buscar en padres

            if (interactable != null)
            {
                Debug.Log($"PlayerInteraction: Interactuando con '{interactable.GetType().Name}' en objeto '{hit.collider.name}'.");
                interactable.Interact(false); 
            }
            else
            {
                Debug.Log("PlayerInteraction: El objeto golpeado NO es interactuable ni recogible (ni en hijos/padres).");
            }
        }
        else
        {
            Debug.Log("PlayerInteraction: Raycast NO golpeó nada (aire).");
        }
    }

    private void TryDropOrTrash()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        // Verificar si estamos mirando a un Basurero
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Basurero"))
            {
                Debug.Log("PlayerInteraction: Mirando objeto con tag 'Basurero'");
                
                // Verificar si tiene el script TrashCan (Búsqueda inteligente)
                TrashCan bin = hit.collider.GetComponent<TrashCan>();
                if (bin == null) bin = hit.collider.GetComponentInChildren<TrashCan>();
                if (bin == null) bin = hit.collider.GetComponentInParent<TrashCan>();
                
                if (bin != null)
                {
                    Debug.Log($"PlayerInteraction: Script TrashCan encontrado en '{bin.name}'. Estado isOpen: {bin.isOpen}");
                    
                    if (bin.isOpen)
                    {
                        Debug.Log("PlayerInteraction: El tacho está ABIERTO. Intentando tirar basura...");
                        currentHeldObject.OnPlaceInTrash();
                        currentHeldObject = null;
                        if (animator != null) animator.SetTrigger("Drop");
                    }
                    else
                    {
                        Debug.Log("PlayerInteraction: El tacho está CERRADO. Intentando ABRIRLO...");
                        bin.Interact(false);
                    }
                }
                else
                {
                    Debug.LogWarning("PlayerInteraction: Objeto tiene tag 'Basurero' pero NO tiene script 'TrashCan' (ni en hijos/padres).");
                    // Comportamiento legacy
                    currentHeldObject.OnPlaceInTrash();
                    currentHeldObject = null;
                    if (animator != null) animator.SetTrigger("Drop");
                }
                return;
            }
        }

        // Si no es basurero, simplemente soltar
        currentHeldObject.OnDrop();
        currentHeldObject = null;
        if (animator != null) animator.SetTrigger("Drop");
    }
}
