using UnityEngine;

/// <summary>
/// Visualiza el punto de impacto del raycast en el mundo 3D.
/// Muestra una esfera en el punto exacto donde el raycast golpea.
/// </summary>
public class RaycastVisualizer : MonoBehaviour
{
    [Header("Configuración Visual")]
    [Tooltip("Color del punto cuando NO hay impacto")]
    public Color missColor = Color.red;
    
    [Tooltip("Color del punto cuando HAY impacto")]
    public Color hitColor = Color.green;
    
    [Tooltip("Tamaño de la esfera del punto")]
    [Range(0.05f, 0.5f)]
    public float sphereSize = 0.1f;
    
    [Header("Configuración de Raycast")]
    [Tooltip("Distancia del raycast (debe coincidir con PlayerInteraction)")]
    public float raycastDistance = 3f;
    
    [Tooltip("Capas con las que interactuar")]
    public LayerMask interactableLayers = ~0;
    
    [Header("Opciones de Visualización")]
    [Tooltip("Mostrar línea del raycast")]
    public bool showRayLine = true;
    
    [Tooltip("Mostrar esfera en el punto de impacto")]
    public bool showHitSphere = true;
    
    [Tooltip("Mostrar información del objeto golpeado")]
    public bool showObjectInfo = true;
    
    private Transform cameraTransform;
    private Vector3 currentHitPoint;
    private bool isHitting = false;
    private string hitObjectName = "";

    void Start()
    {
        // Obtener referencia a la cámara
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            Debug.Log("[RAYCAST VIZ] ✅ Visualizador de raycast inicializado");
        }
        else
        {
            Debug.LogError("[RAYCAST VIZ] ❌ No se encontró Main Camera");
        }
    }

    void Update()
    {
        if (cameraTransform == null) return;
        
        // Hacer raycast
        RaycastHit hit;
        isHitting = Physics.Raycast(
            cameraTransform.position, 
            cameraTransform.forward, 
            out hit, 
            raycastDistance, 
            interactableLayers
        );
        
        if (isHitting)
        {
            currentHitPoint = hit.point;
            hitObjectName = hit.collider.name;
        }
        else
        {
            // Si no golpea nada, poner el punto al final del raycast
            currentHitPoint = cameraTransform.position + cameraTransform.forward * raycastDistance;
            hitObjectName = "";
        }
    }

    void OnDrawGizmos()
    {
        if (cameraTransform == null) return;
        
        // Dibujar línea del raycast
        if (showRayLine)
        {
            Gizmos.color = isHitting ? hitColor : missColor;
            Gizmos.DrawLine(cameraTransform.position, currentHitPoint);
        }
        
        // Dibujar esfera en el punto de impacto
        if (showHitSphere)
        {
            Gizmos.color = isHitting ? hitColor : missColor;
            Gizmos.DrawSphere(currentHitPoint, sphereSize);
            
            // Dibujar esfera wireframe para mejor visibilidad
            Gizmos.DrawWireSphere(currentHitPoint, sphereSize);
        }
    }

    void OnGUI()
    {
        if (!showObjectInfo || cameraTransform == null) return;
        
        // Mostrar información en pantalla
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = isHitting ? Color.green : Color.red;
        style.alignment = TextAnchor.UpperLeft;
        
        string info = $"Raycast Debug:\n";
        info += $"Estado: {(isHitting ? "GOLPEANDO ✓" : "SIN IMPACTO ✗")}\n";
        info += $"Distancia: {Vector3.Distance(cameraTransform.position, currentHitPoint):F2}m\n";
        
        if (isHitting)
        {
            info += $"Objeto: {hitObjectName}\n";
            info += $"Posición: {currentHitPoint}\n";
        }
        
        GUI.Label(new Rect(10, 10, 400, 150), info, style);
    }
}
