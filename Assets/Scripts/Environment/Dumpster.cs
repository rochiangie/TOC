using UnityEngine;

public class Dumpster : MonoBehaviour, IInteractable
{
    public void Interact(bool isBagFull)
    {
        if (isBagFull)
        {
            // Dispose logic
            GameEvents.OnBagDisposed?.Invoke();
            GameEvents.OnPlaySFX?.Invoke("Dumpster");
            
            // Optional: Play dumpster animation
            Debug.Log("Bag Disposed!");
        }
        else
        {
            // Optional: Feedback "Nothing to dispose"
        }
    }
}
