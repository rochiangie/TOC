using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    [Header("Animation Settings")]
    public Animator animator;
    public string openParamName = "IsOpened";
    public string closeParamName = "IsClosed";

    [Header("Debug State")]
    public bool isOpen = false; // Ahora es visible en el Inspector

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        
        if (animator == null) Debug.LogError($"❌ TrashCan: NO se encontró Animator en {name}");
        else Debug.Log($"✅ TrashCan: Animator encontrado en {name}");
    }

    private void Start()
    {
        // Sincronizar estado inicial
        if (animator != null)
        {
            animator.SetBool(openParamName, isOpen);
            animator.SetBool(closeParamName, !isOpen);
        }
    }

    // Implementación de la interfaz IInteractable
    public void Interact(bool isBagFull)
    {
        Debug.Log($"TrashCan: Interact llamado en {name}. Estado actual isOpen: {isOpen}");
        ToggleLid();
    }

    [ContextMenu("Probar Abrir/Cerrar (Test Toggle)")]
    public void ToggleLid()
    {
        isOpen = !isOpen;
        Debug.Log($"TrashCan: ToggleLid ejecutado. Nuevo estado isOpen: {isOpen}");

        if (animator != null)
        {
            if (isOpen)
            {
                // ABRIR
                animator.SetBool(openParamName, true);
                animator.SetBool(closeParamName, false);
                Debug.Log($"TrashCan: Animator -> SetBool({openParamName}, true), SetBool({closeParamName}, false)");
            }
            else
            {
                // CERRAR
                animator.SetBool(openParamName, false);
                animator.SetBool(closeParamName, true);
                Debug.Log($"TrashCan: Animator -> SetBool({openParamName}, false), SetBool({closeParamName}, true)");
            }
        }
    }

    // Función extra por si quieres abrirlo automáticamente al tirar basura desde otro script
    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            if (animator != null)
            {
                animator.SetBool(openParamName, true);
                animator.SetBool(closeParamName, false);
            }
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            isOpen = false;
            if (animator != null)
            {
                animator.SetBool(openParamName, false);
                animator.SetBool(closeParamName, true);
            }
        }
    }

    void OnGUI()
    {
        // Solo mostrar si el jugador está cerca (opcional, por ahora mostramos siempre para debug)
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPos.z > 0 && screenPos.z < 10)
        {
            GUI.color = Color.black;
            GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 300, 20), $"Bin: {name}");
            
            GUI.color = isOpen ? Color.green : Color.red;
            GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y + 20, 300, 20), $"Script isOpen: {isOpen}");
            
            if (animator != null)
            {
                bool animOpen = animator.GetBool(openParamName);
                GUI.color = animOpen ? Color.green : Color.red;
                GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y + 40, 300, 20), $"Anim '{openParamName}': {animOpen}");
            }
        }
    }
}
