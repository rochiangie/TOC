using UnityEngine;

public class TrashItem : MonoBehaviour, IInteractable
{
    public void Interact(bool isBagFull)
    {
        if (isBagFull)
        {
            // Cannot collect if bag is full
            // Optional: Play "Inventory Full" sound or UI shake
            return;
        }

        // Collect logic
        GameEvents.OnTrashCollected?.Invoke();
        GameEvents.OnPlaySFX?.Invoke("Pickup");
        
        // Destroy object
        Destroy(gameObject);
    }
}
