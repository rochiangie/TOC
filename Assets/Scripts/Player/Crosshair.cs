using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra un punto de mira (crosshair) en el centro de la pantalla.
/// Indica dónde apunta el raycast de interacción del jugador.
/// </summary>
public class Crosshair : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Color del punto de mira")]
    public Color crosshairColor = Color.white;
    
    [Tooltip("Tamaño del punto de mira")]
    [Range(2f, 20f)]
    public float size = 8f;
    
    [Tooltip("Opacidad del punto de mira")]
    [Range(0f, 1f)]
    public float alpha = 0.8f;
    
    [Header("Color Dinámico")]
    [Tooltip("Cambiar color cuando se puede interactuar")]
    public bool dynamicColor = true;
    
    [Tooltip("Color cuando se puede interactuar")]
    public Color interactableColor = Color.green;
    
    [Tooltip("Distancia de raycast (debe coincidir con PlayerInteraction)")]
    public float raycastDistance = 3f;
    
    [Tooltip("Capas con las que interactuar")]
    public LayerMask interactableLayers = ~0;
    
    private Image crosshairImage;
    private Transform cameraTransform;
    private Color currentColor;

    void Start()
    {
        // Crear el crosshair UI
        CreateCrosshair();
        
        // Obtener referencia a la cámara
        cameraTransform = Camera.main.transform;
        
        // Color inicial
        currentColor = crosshairColor;
        currentColor.a = alpha;
    }

    void Update()
    {
        if (crosshairImage == null || cameraTransform == null) return;
        
        // Actualizar color dinámico si está activado
        if (dynamicColor)
        {
            UpdateDynamicColor();
        }
        else
        {
            crosshairImage.color = currentColor;
        }
    }

    private void CreateCrosshair()
    {
        // Buscar o crear Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("[CROSSHAIR] ✅ Canvas creado automáticamente");
        }

        // Crear GameObject para el crosshair
        GameObject crosshairObj = new GameObject("Crosshair");
        crosshairObj.transform.SetParent(canvas.transform, false);

        // Agregar componente Image
        crosshairImage = crosshairObj.AddComponent<Image>();
        
        // Crear sprite circular
        crosshairImage.sprite = CreateCircleSprite();
        crosshairImage.color = crosshairColor;
        
        // Configurar RectTransform (centrado en pantalla)
        RectTransform rectTransform = crosshairObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(size, size);

        Debug.Log("[CROSSHAIR] ✅ Crosshair creado en el centro de la pantalla");
    }

    private void UpdateDynamicColor()
    {
        // Hacer raycast para detectar objetos interactuables
        RaycastHit hit;
        bool canInteract = false;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, raycastDistance, interactableLayers))
        {
            // Verificar si es interactuable
            if (hit.collider.CompareTag("Recogible") || 
                hit.collider.CompareTag("Basurero") ||
                hit.collider.GetComponent<IInteractable>() != null ||
                hit.collider.GetComponent<PickupableObject>() != null)
            {
                canInteract = true;
            }
        }

        // Cambiar color suavemente
        Color targetColor = canInteract ? interactableColor : crosshairColor;
        targetColor.a = alpha;
        
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * 10f);
        crosshairImage.color = currentColor;
    }

    private Sprite CreateCircleSprite()
    {
        // Crear una textura simple para el punto
        int textureSize = 32;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        // Dibujar un círculo
        Vector2 center = new Vector2(textureSize / 2f, textureSize / 2f);
        float radius = textureSize / 2f - 2f;
        
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                
                if (distance <= radius)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        texture.Apply();
        
        // Crear sprite desde la textura
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
    }

    // Métodos públicos para control externo
    
    /// <summary>
    /// Muestra u oculta el crosshair
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (crosshairImage != null)
        {
            crosshairImage.enabled = visible;
        }
    }
    
    /// <summary>
    /// Cambia el color del crosshair
    /// </summary>
    public void SetColor(Color color)
    {
        crosshairColor = color;
        currentColor = color;
        currentColor.a = alpha;
    }
    
    /// <summary>
    /// Cambia el tamaño del crosshair
    /// </summary>
    public void SetSize(float newSize)
    {
        size = newSize;
        if (crosshairImage != null)
        {
            RectTransform rectTransform = crosshairImage.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(size, size);
        }
    }
}
