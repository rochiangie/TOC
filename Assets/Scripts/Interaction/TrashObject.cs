using UnityEngine;

public class TrashObject : PickupableObject
{
    [Header("Trash Settings")]
    public int scoreValue = 10;

    public override void OnPickUp(Transform holder)
    {
        base.OnPickUp(holder);
        // Aquí podrías reproducir un sonido de "bolsa recogida"
        Debug.Log("Recogiste basura.");
    }

    public override void OnPlaceInTrash()
    {
        // Aquí podrías sumar puntos al TrashManager
        // TrashManager.Instance.AddScore(scoreValue);
        
        base.OnPlaceInTrash(); // Destruye el objeto
    }
}
