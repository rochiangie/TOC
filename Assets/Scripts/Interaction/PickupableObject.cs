using UnityEngine;

public abstract class PickupableObject : MonoBehaviour
{
    protected Rigidbody rb;
    protected Collider coll;
    protected bool isHeld = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    public virtual void OnPickUp(Transform holder)
    {
        isHeld = true;
        if (rb)
        {
            rb.isKinematic = true; // Desactivar física al sostener
            rb.interpolation = RigidbodyInterpolation.None;
        }
        if (coll)
        {
            coll.enabled = false; // Desactivar colisión para no chocar con el jugador
        }

        // Parentar al HoldPoint
        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public virtual void OnDrop()
    {
        isHeld = false;
        transform.SetParent(null);
        
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

    public virtual void OnPlaceInTrash()
    {
        // Lógica base al tirar a la basura (destruir por defecto)
        Debug.Log($"Objeto {name} tirado a la basura.");
        Destroy(gameObject);
    }
}
