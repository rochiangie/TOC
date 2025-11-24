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
            // Búsqueda inteligente: primero en el collider, luego en padres, luego en hijos
            PickupableObject pickup = hit.collider.GetComponent<PickupableObject>();
            if (pickup == null) pickup = hit.collider.GetComponentInParent<PickupableObject>();
            if (pickup == null) pickup = hit.collider.GetComponentInChildren<PickupableObject>();
            
            if (pickup != null || hit.collider.CompareTag("Recogible"))
            {
                if (pickup != null)
                {
                    Debug.Log($"✅ PlayerInteraction: Recogiendo '{pickup.gameObject.name}' (script encontrado en '{pickup.transform.name}')");
                    currentHeldObject = pickup;
                    currentHeldObject.OnPickUp(holdPoint);
                    
                    if (animator != null) animator.SetTrigger("PickUp");
                    return;
                }
                else
                {
                    Debug.LogWarning($"⚠️ PlayerInteraction: Objeto '{hit.collider.name}' tiene tag 'Recogible' pero NO tiene script PickupableObject.");
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
                    Debug.Log($"PlayerInteraction: Script TrashCan encontrado en '{bin.name}'.");
                    
                    // 1. Primero soltar el objeto de la mano del jugador (SIN activar física)
                    PickupableObject objectToTrash = currentHeldObject;
                    currentHeldObject.OnDrop(false); // false = no activar física, será absorbido
                    currentHeldObject = null;
                    if (animator != null) animator.SetTrigger("Drop");
                    
                    // 2. Abrir el basurero
                    bin.Open();
                    
                    // 3. Iniciar la absorción (ahora el objeto está libre en el mundo)
                    objectToTrash.OnPlaceInTrash();
                }
                else
                {
                    Debug.LogWarning("PlayerInteraction: Objeto tiene tag 'Basurero' pero NO tiene script 'TrashCan' (ni en hijos/padres).");
                    // Comportamiento legacy
                    PickupableObject objectToTrash = currentHeldObject;
                    currentHeldObject.OnDrop(false); // false = no activar física
                    currentHeldObject = null;
                    if (animator != null) animator.SetTrigger("Drop");
                    objectToTrash.OnPlaceInTrash();
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
