using UnityEngine;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public LayerMask interactableLayer = ~0;
    public KeyCode interactKey = KeyCode.E;
    
    [Header("Pickup Settings")]
    [Tooltip("Tiempo en segundos antes de que el objeto aparezca en la mano (para animación de agacharse)")]
    public float pickupDelay = 0.8f;
    
    [Header("References")]
    public Transform cameraTransform;
    public Transform holdPoint;
    public Animator animator;

    private PickupableObject currentHeldObject;
    private bool isPickingUp = false;

    // Properties públicas (solo lectura)
    public PickupableObject CurrentHeldObject => currentHeldObject;
    public bool HasObject => currentHeldObject != null;
    public bool IsPickingUp => isPickingUp;

    private void Start()
    {
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

        if (holdPoint == null)
        {
            GameObject hp = new GameObject("HoldPoint");
            hp.transform.SetParent(cameraTransform != null ? cameraTransform : transform);
            hp.transform.localPosition = new Vector3(0.5f, -0.5f, 1f);
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
            else if (!isPickingUp)
            {
                TryPickUp();
            }
        }
    }

    private void TryPickUp()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * interactionDistance, Color.red, 2f);

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionDistance, interactableLayer))
        {
            Debug.Log($"PlayerInteraction: Raycast golpeó a '{hit.collider.name}' (Tag: {hit.collider.tag})");

            PickupableObject pickup = hit.collider.GetComponent<PickupableObject>();
            if (pickup == null) pickup = hit.collider.GetComponentInParent<PickupableObject>();
            if (pickup == null) pickup = hit.collider.GetComponentInChildren<PickupableObject>();
            
            if (pickup != null || hit.collider.CompareTag("Recogible"))
            {
                if (pickup != null)
                {
                    Debug.Log($"✅ PlayerInteraction: Recogiendo '{pickup.gameObject.name}'");
                    StartCoroutine(PickUpWithDelay(pickup));
                    return;
                }
                else
                {
                    Debug.LogWarning($"⚠️ PlayerInteraction: Objeto '{hit.collider.name}' tiene tag 'Recogible' pero NO tiene script PickupableObject.");
                }
            }

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null) interactable = hit.collider.GetComponentInChildren<IInteractable>();
            if (interactable == null) interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                Debug.Log($"PlayerInteraction: Interactuando con '{interactable.GetType().Name}' en objeto '{hit.collider.name}'.");
                interactable.Interact(false); 
            }
            else
            {
                Debug.Log("PlayerInteraction: El objeto golpeado NO es interactuable ni recogible.");
            }
        }
        else
        {
            Debug.Log("PlayerInteraction: Raycast NO golpeó nada.");
        }
    }

    private IEnumerator PickUpWithDelay(PickupableObject pickup)
    {
        isPickingUp = true;
        
        if (animator != null) animator.SetTrigger("PickUp");
        
        Debug.Log($"🎬 Animación de agacharse iniciada. Esperando {pickupDelay} segundos...");
        
        yield return new WaitForSeconds(pickupDelay);
        
        currentHeldObject = pickup;
        currentHeldObject.OnPickUp(holdPoint);
        
        Debug.Log($"✅ Objeto '{pickup.gameObject.name}' ahora en la mano del jugador");
        
        isPickingUp = false;
    }

    private void TryDropOrTrash()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Basurero"))
            {
                Debug.Log("PlayerInteraction: Mirando objeto con tag 'Basurero'");
                
                TrashCan bin = hit.collider.GetComponent<TrashCan>();
                if (bin == null) bin = hit.collider.GetComponentInChildren<TrashCan>();
                if (bin == null) bin = hit.collider.GetComponentInParent<TrashCan>();
                
                if (bin != null)
                {
                    Debug.Log($"PlayerInteraction: Script TrashCan encontrado en '{bin.name}'.");
                    
                    TrashObject trashObj = currentHeldObject as TrashObject;
                    
                    if (trashObj != null)
                    {
                        if (!trashObj.CanGoInTrashCan(bin.trashType))
                        {
                            Debug.LogWarning($"❌ ¡Basura incorrecta! Este es un basurero {bin.trashType} y estás intentando tirar basura tipo {trashObj.trashType}");
                            
                            string binColorName = GetTrashTypeName(bin.trashType);
                            string trashColorName = GetTrashTypeName((TrashCan.TrashType)(int)trashObj.trashType);
                            
                            if (FeedbackMessageUI.Instance != null)
                            {
                                FeedbackMessageUI.Instance.ShowError(
                                    $"❌ ¡Basurero Incorrecto!\n" +
                                    $"Este basurero es para: {binColorName}\n" +
                                    $"Tu basura es: {trashColorName}",
                                    3f
                                );
                            }
                            
                            return;
                        }
                        
                        Debug.Log($"✅ ¡Correcto! Basura {trashObj.trashType} en basurero {bin.trashType}");
                        
                        if (FeedbackMessageUI.Instance != null)
                        {
                            FeedbackMessageUI.Instance.ShowSuccess("✅ ¡Excelente! Basura clasificada correctamente", 2f);
                        }
                    }
                    
                    if (animator != null) animator.SetTrigger("Throw");
                    
                    PickupableObject objectToTrash = currentHeldObject;
                    currentHeldObject.OnDrop(false);
                    currentHeldObject = null;
                    
                    bin.Open();
                    
                    objectToTrash.OnPlaceInTrash();
                }
                else
                {
                    Debug.LogWarning("PlayerInteraction: Objeto tiene tag 'Basurero' pero NO tiene script 'TrashCan'.");
                    if (animator != null) animator.SetTrigger("Throw");
                    PickupableObject objectToTrash = currentHeldObject;
                    currentHeldObject.OnDrop(false);
                    currentHeldObject = null;
                    objectToTrash.OnPlaceInTrash();
                }
                return;
            }
        }

        currentHeldObject.OnDrop();
        currentHeldObject = null;
        if (animator != null) animator.SetTrigger("Drop");
    }

    private string GetTrashTypeName(TrashCan.TrashType type)
    {
        switch (type)
        {
            case TrashCan.TrashType.Amarillo:
                return "AMARILLO (Plástico/Envases)";
            case TrashCan.TrashType.Azul:
                return "AZUL (Papel/Cartón)";
            case TrashCan.TrashType.Verde:
                return "VERDE (Vidrio)";
            case TrashCan.TrashType.Rojo:
                return "ROJO (Peligrosos)";
            default:
                return "Desconocido";
        }
    }
}
