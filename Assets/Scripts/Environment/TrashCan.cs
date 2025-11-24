using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    [Header("Animation Settings")]
    public Animator animator;
    public string openParamName = "IsOpened";
    public string closeParamName = "IsClosed";

    public bool IsOpen { get; private set; } = false;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        
        if (animator == null) Debug.LogError($"❌ TrashCan: NO se encontró Animator en {name}");
        else Debug.Log($"✅ TrashCan: Animator encontrado en {name}");
    }

    // Implementación de la interfaz IInteractable
    public void Interact(bool isBagFull)
    {
        Debug.Log($"TrashCan: Interact llamado en {name}");
        ToggleLid();
    }

    public void ToggleLid()
    {
        IsOpen = !IsOpen;
        Debug.Log($"TrashCan: ToggleLid -> Nuevo estado IsOpen: {IsOpen}");

        if (animator != null)
        {
            if (IsOpen)
            {
                // ABRIR
                animator.SetBool(openParamName, true);
                animator.SetBool(closeParamName, false);
                Debug.Log($"TrashCan: Animación ABRIR ({openParamName}=true, {closeParamName}=false)");
            }
            else
            {
                // CERRAR
                animator.SetBool(openParamName, false);
                animator.SetBool(closeParamName, true);
                Debug.Log($"TrashCan: Animación CERRAR ({openParamName}=false, {closeParamName}=true)");
            }
        }
        else
        {
            Debug.LogWarning($"TrashCan: {name} no tiene Animator asignado. No se puede reproducir animación.");
        }
    }

    // Función extra por si quieres abrirlo automáticamente al tirar basura desde otro script
    public void Open()
    {
        if (!IsOpen)
        {
            IsOpen = true;
            if (animator != null)
            {
                animator.SetBool(openParamName, true);
                animator.SetBool(closeParamName, false);
            }
        }
    }

    public void Close()
    {
        if (IsOpen)
        {
            IsOpen = false;
            if (animator != null)
            {
                animator.SetBool(openParamName, false);
                animator.SetBool(closeParamName, true);
            }
        }
    }
}
