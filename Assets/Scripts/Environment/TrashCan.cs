using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    public enum TrashType
    {
        Amarillo,  // Plástico/Envases
        Azul,      // Papel/Cartón
        Verde,     // Vidrio
        Rojo       // Residuos peligrosos
    }

    [Header("Trash Type")]
    public TrashType trashType = TrashType.Amarillo;
    
    [Header("Animation Settings")]
    public Animator animator;
    public string openParamName = "IsOpened";

    [Header("Label Settings")]
    public Vector3 labelOffset = new Vector3(0, 2, 0); // Altura del cartel sobre el basurero
    public float labelDistance = 5f; // Distancia máxima para ver el cartel

    private bool isOpen = false;
    private Color labelColor;
    private string labelText;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        
        // Configurar color y texto según el tipo
        switch (trashType)
        {
            case TrashType.Amarillo:
                labelColor = Color.yellow;
                labelText = "AMARILLO\nPlástico/Envases";
                break;
            case TrashType.Azul:
                labelColor = Color.blue;
                labelText = "AZUL\nPapel/Cartón";
                break;
            case TrashType.Verde:
                labelColor = Color.green;
                labelText = "VERDE\nVidrio";
                break;
            case TrashType.Rojo:
                labelColor = Color.red;
                labelText = "ROJO\nPeligrosos";
                break;
        }
    }

    // Implementación de la interfaz IInteractable
    public void Interact(bool isBagFull)
    {
        Open();
    }

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            if (animator != null)
            {
                animator.SetBool(openParamName, true);
            }
        }
    }

    void OnGUI()
    {
        // Mostrar cartel solo si el jugador está cerca
        if (Camera.main == null) return;
        
        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        if (distance > labelDistance) return;

        Vector3 labelWorldPos = transform.position + labelOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(labelWorldPos);
        
        if (screenPos.z > 0)
        {
            // Fondo del cartel
            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.Box(new Rect(screenPos.x - 60, Screen.height - screenPos.y - 30, 120, 60), "");
            
            // Texto del cartel
            GUI.color = labelColor;
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 14;
            style.fontStyle = FontStyle.Bold;
            
            GUI.Label(new Rect(screenPos.x - 60, Screen.height - screenPos.y - 30, 120, 60), labelText, style);
        }
    }
}
