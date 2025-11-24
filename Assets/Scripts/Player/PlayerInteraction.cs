using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;
    
    [Header("References")]
    public Transform cameraTransform;
    public Transform holdPoint; // Asigna esto en el Inspector (un hijo del Player/Camera)
    public Animator animator;

    private PickupableObject currentHeldObject;

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

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionDistance, interactableLayer))
        {
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

            // 2. Soporte Legacy para IInteractable (si aún lo usas)
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(false); // Asumimos false para isBagFull por ahora
            }
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
                // Tirar al basurero
                currentHeldObject.OnPlaceInTrash();
                currentHeldObject = null;
                
                if (animator != null) animator.SetTrigger("Drop");
                return;
            }
        }

        // Si no es basurero, simplemente soltar
        currentHeldObject.OnDrop();
        currentHeldObject = null;
        if (animator != null) animator.SetTrigger("Drop");
    }
}
