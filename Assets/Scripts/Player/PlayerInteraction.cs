using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;
    public Transform cameraTransform;

    private TrashManager trashManager;

    private void Start()
    {
        trashManager = FindObjectOfType<TrashManager>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1")) // Left Click
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                bool isBagFull = trashManager != null && trashManager.IsBagFull();
                interactable.Interact(isBagFull);
            }
        }
    }
}
