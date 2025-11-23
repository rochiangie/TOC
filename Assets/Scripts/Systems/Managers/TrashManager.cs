using UnityEngine;

public class TrashManager : MonoBehaviour
{
    [Header("Settings")]
    public int bagCapacity = 5;

    private int totalTrashInLevel;
    private int totalTrashCollected;
    private int currentTrashInBag;
    private bool isCarryingBag;

    private void Start()
    {
        // Count all trash in scene (requires TrashItem to be implemented)
        // We will use FindObjectsOfType for simplicity in this prototype
        // In a larger game, we might register them manually
        TrashItem[] trashItems = FindObjectsOfType<TrashItem>();
        totalTrashInLevel = trashItems.Length;
        
        GameEvents.OnTrashCountUpdated?.Invoke(totalTrashCollected, totalTrashInLevel);
    }

    private void OnEnable()
    {
        GameEvents.OnTrashCollected += HandleTrashCollected;
        GameEvents.OnBagDisposed += HandleBagDisposed;
    }

    private void OnDisable()
    {
        GameEvents.OnTrashCollected -= HandleTrashCollected;
        GameEvents.OnBagDisposed -= HandleBagDisposed;
    }

    private void HandleTrashCollected()
    {
        currentTrashInBag++;
        totalTrashCollected++;

        GameEvents.OnTrashCountUpdated?.Invoke(totalTrashCollected, totalTrashInLevel);

        if (currentTrashInBag >= bagCapacity)
        {
            isCarryingBag = true;
            GameEvents.OnBagFilled?.Invoke();
        }
    }

    private void HandleBagDisposed()
    {
        currentTrashInBag = 0;
        isCarryingBag = false;

        if (totalTrashCollected >= totalTrashInLevel)
        {
            GameManager.Instance.SetState(GameState.Won);
        }
    }

    public bool IsCarryingBag()
    {
        return isCarryingBag;
    }
    
    public bool IsBagFull()
    {
        return currentTrashInBag >= bagCapacity;
    }
}
