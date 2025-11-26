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
        
        // DEBUG: Mostrar la jerarquía
        if (rb != null)
        {
            Debug.Log($"🔍 PickupableObject en '{gameObject.name}': Rigidbody encontrado en '{rb.gameObject.name}'");
        }
    }

    public virtual void OnPickUp(Transform holder)
    {
        isHeld = true;
        
        // MEJORADO: Determinar el objeto raíz a mover
        // Prioridad: 1) Objeto con Rigidbody, 2) Objeto raíz de la jerarquía, 3) Este objeto
        Transform objectToMove = null;
        
        if (rb != null)
        {
            // Si hay Rigidbody, usar ese transform (es el objeto físico principal)
            objectToMove = rb.transform;
        }
        else
        {
            // Si no hay Rigidbody, buscar el objeto raíz de la jerarquía
            objectToMove = transform.root;
            
            // Si el root es el mismo que este objeto, usar este
            if (objectToMove == transform)
            {
                objectToMove = transform;
            }
        }
        
        Debug.Log($"📦 PickupableObject: Script en '{gameObject.name}' → Moviendo objeto raíz '{objectToMove.name}' al HoldPoint");
        Debug.Log($"   Jerarquía: {GetHierarchyPath(transform)}");
        
        if (rb)
        {
            rb.isKinematic = true; // Desactivar física al sostener
            rb.interpolation = RigidbodyInterpolation.None;
        }
        
        // Desactivar TODOS los colliders en el objeto raíz y sus hijos
        Collider[] allColliders = objectToMove.GetComponentsInChildren<Collider>();
        foreach (Collider col in allColliders)
        {
            col.enabled = false;
        }
        
        // Parentar el objeto raíz completo al HoldPoint
        objectToMove.SetParent(holder);
        objectToMove.localPosition = Vector3.zero;
        objectToMove.localRotation = Quaternion.identity;
    }
    
    /// <summary>
    /// Helper para debugging: muestra la ruta completa en la jerarquía
    /// </summary>
    private string GetHierarchyPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    public virtual void OnDrop(bool enablePhysics = true)
    {
        isHeld = false;
        
        // Usar la misma lógica que OnPickUp para encontrar el objeto raíz
        Transform objectToMove = null;
        
        if (rb != null)
        {
            objectToMove = rb.transform;
        }
        else
        {
            objectToMove = transform.root;
            if (objectToMove == transform)
            {
                objectToMove = transform;
            }
        }
        
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
            
            // Reactivar TODOS los colliders
            Collider[] allColliders = objectToMove.GetComponentsInChildren<Collider>();
            foreach (Collider col in allColliders)
            {
                col.enabled = true;
            }
        }
        else
        {
            // Mantener física desactivada (para absorción)
            if (rb)
            {
                rb.isKinematic = true;
            }
            
            // Mantener colliders desactivados
            Collider[] allColliders = objectToMove.GetComponentsInChildren<Collider>();
            foreach (Collider col in allColliders)
            {
                col.enabled = false;
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
