using UnityEngine;
using System.Collections;

public class TrashObject : PickupableObject
{
    public enum TrashType
    {
        Amarillo,  // Plástico/Envases
        Azul,      // Papel/Cartón
        Verde,     // Vidrio
        Rojo       // Residuos peligrosos
    }

    [Header("Trash Properties")]
    public TrashType trashType = TrashType.Amarillo;
    public int scoreValue = 10;

    [Header("Absorption Effect")]
    public float absorptionDuration = 0.5f; // Duración de la animación de absorción
    public AnimationCurve absorptionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isBeingAbsorbed = false;

    public override void OnPickUp(Transform holder)
    {
        base.OnPickUp(holder);
        Debug.Log($"Recogiste: {gameObject.name} (Tipo: {trashType})");
    }

    public override void OnPlaceInTrash()
    {
        // No destruir inmediatamente, iniciar absorción
        if (!isBeingAbsorbed)
        {
            StartCoroutine(AbsorbIntoTrash());
        }
    }

    private IEnumerator AbsorbIntoTrash()
    {
        isBeingAbsorbed = true;
        Debug.Log($"🌀 Absorbiendo {gameObject.name} hacia el basurero...");

        // Obtener el transform del objeto raíz
        Transform objectTransform = (rb != null) ? rb.transform : transform;
        
        // Encontrar el basurero más cercano
        TrashCan nearestBin = FindNearestTrashCan();
        if (nearestBin == null)
        {
            Debug.LogWarning("No se encontró basurero cercano, destruyendo objeto directamente.");
            Destroy(objectTransform.gameObject);
            yield break;
        }

        Vector3 startPosition = objectTransform.position;
        Vector3 targetPosition = nearestBin.transform.position + Vector3.up * 0.5f; // Un poco arriba del basurero
        Vector3 startScale = objectTransform.localScale;
        Vector3 targetScale = startScale * 0.3f; // Reducir a 30% del tamaño original
        Quaternion startRotation = objectTransform.rotation;

        float elapsed = 0f;
        float rotationSpeed = 720f; // Grados por segundo (2 rotaciones completas)

        while (elapsed < absorptionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / absorptionDuration;
            float curveValue = absorptionCurve.Evaluate(t);

            // Mover hacia el basurero
            objectTransform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);
            
            // Reducir tamaño
            objectTransform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);

            // Rotar para efecto de vórtice
            objectTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            yield return null;
        }

        // Destruir el objeto después de la absorción
        Debug.Log($"✅ {gameObject.name} absorbido exitosamente!");
        Destroy(objectTransform.gameObject);
    }

    private TrashCan FindNearestTrashCan()
    {
        TrashCan[] allBins = FindObjectsOfType<TrashCan>();
        TrashCan nearest = null;
        float minDistance = float.MaxValue;

        Transform objectTransform = (rb != null) ? rb.transform : transform;

        foreach (TrashCan bin in allBins)
        {
            float distance = Vector3.Distance(objectTransform.position, bin.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = bin;
            }
        }

        return nearest;
    }

    // Método para verificar si este objeto puede ir en un basurero específico
    public bool CanGoInTrashCan(TrashCan.TrashType binType)
    {
        // Convertir nuestro tipo al tipo del basurero (son el mismo enum)
        return (int)trashType == (int)binType;
    }
}
