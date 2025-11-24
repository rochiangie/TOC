using UnityEngine;

public abstract class PickupableObject : MonoBehaviour
{
    protected Rigidbody rb;
    protected Collider coll;
    protected bool isHeld = false;

    protected virtual void Awake()
    {
        // Buscar Rigidbody: primero en este objeto, luego en padres
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = GetComponentInParent<Rigidbody>();
        
        // Buscar Collider: primero en este objeto, luego en padres, luego en hijos
        coll = GetComponent<Collider>();
        if (coll == null) coll = GetComponentInParent<Collider>();
        if (coll == null) coll = GetComponentInChildren<Collider>();
        
        if (rb == null)
            Debug.LogWarning($"⚠️ PickupableObject en '{gameObject.name}': No se encontró Rigidbody (ni en padres).");
        if (coll == null)
            Debug.LogWarning($"⚠️ PickupableObject en '{gameObject.name}': No se encontró Collider (ni en padres/hijos).");
    }

    public virtual void OnPickUp(Transform holder)
    {
        isHeld = true;
        
        // Determinar qué transform mover: el que tiene el Rigidbody (objeto raíz) o este transform
        Transform objectToMove = (rb != null) ? rb.transform : transform;
        
        if (rb)
        {
            rb.isKinematic = true; // Desactivar física al sostener
            rb.interpolation = RigidbodyInterpolation.None;
        }
        if (coll)
        {
            coll.enabled = false; // Desactivar colisión para no chocar con el jugador
        }

        // Parentar el objeto raíz completo al HoldPoint
        objectToMove.SetParent(holder);
        objectToMove.localPosition = Vector3.zero;
        objectToMove.localRotation = Quaternion.identity;
        
        Debug.Log($"📦 PickupableObject: Moviendo '{objectToMove.name}' al HoldPoint (script está en '{gameObject.name}')");
    }

    public virtual void OnDrop(bool enablePhysics = true)
    {
        isHeld = false;
        
        // Usar el mismo transform que se usó en OnPickUp
        Transform objectToMove = (rb != null) ? rb.transform : transform;
        objectToMove.SetParent(null);
        
        if (enablePhysics)
        {
            if (rb)
            {
                rb.isKinematic = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                // Opcional: Dar un pequeño empujón hacia adelante
                // rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
            }
            
            if (coll)
            {
                coll.enabled = true;
            }
        }
        else
        {
            // Mantener física desactivada (para absorción)
            if (rb)
            {
                rb.isKinematic = true;
            }
            if (coll)
            {
                coll.enabled = false;
            }
        }
    }

    public virtual void OnPlaceInTrash()
    {
        // Lógica base al tirar a la basura (destruir por defecto)
        Debug.Log($"Objeto {name} tirado a la basura.");
        Destroy(gameObject);
    }
}
