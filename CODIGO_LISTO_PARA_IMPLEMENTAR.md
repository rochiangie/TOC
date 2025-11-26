# 💻 CÓDIGO LISTO PARA IMPLEMENTAR

## 📋 Instrucciones
Este documento contiene código completo y listo para copiar y pegar. Cada sección incluye:
- ✅ Nombre del archivo
- ✅ Ubicación exacta
- ✅ Código completo
- ✅ Instrucciones de configuración

---

## 🔴 PRIORIDAD 1: ScoreManager.cs

**Ubicación**: `Assets/Scripts/Systems/Managers/ScoreManager.cs`

```csharp
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    [Header("Score Settings")]
    [Tooltip("Puntos base por clasificación correcta")]
    public int correctTrashPoints = 100;
    
    [Tooltip("Puntos adicionales por cada nivel de combo")]
    public int comboMultiplier = 50;
    
    [Tooltip("Segundos para mantener el combo activo")]
    public float comboTimeWindow = 3f;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    
    private int currentScore = 0;
    private int currentCombo = 0;
    private float lastCorrectTime = 0f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void OnEnable()
    {
        GameEvents.OnTrashSorted += HandleTrashSorted;
    }
    
    void OnDisable()
    {
        GameEvents.OnTrashSorted -= HandleTrashSorted;
    }
    
    void Update()
    {
        // Resetear combo si pasa mucho tiempo sin clasificar correctamente
        if (currentCombo > 0 && Time.time - lastCorrectTime > comboTimeWindow)
        {
            ResetCombo();
        }
    }
    
    private void HandleTrashSorted(bool isCorrect, TrashCan.TrashType binType)
    {
        if (isCorrect)
        {
            // Incrementar combo
            currentCombo++;
            lastCorrectTime = Time.time;
            
            // Calcular puntos con bonus de combo
            int points = correctTrashPoints + ((currentCombo - 1) * comboMultiplier);
            AddScore(points);
            
            // Notificar evento de combo
            GameEvents.ComboIncreased(currentCombo);
            
            if (showDebugLogs)
            {
                Debug.Log($"[SCORE] ✅ +{points} puntos | Combo x{currentCombo} | Total: {currentScore}");
            }
        }
        else
        {
            // Clasificación incorrecta rompe el combo
            if (currentCombo > 0)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"[SCORE] ❌ Combo perdido (era x{currentCombo})");
                }
                ResetCombo();
            }
        }
    }
    
    private void AddScore(int points)
    {
        currentScore += points;
        GameEvents.ScoreChanged(currentScore);
    }
    
    private void ResetCombo()
    {
        currentCombo = 0;
        GameEvents.ComboReset();
    }
    
    // Métodos públicos para acceder a los datos
    public int GetScore() => currentScore;
    public int GetCombo() => currentCombo;
    
    // Método para resetear el score (útil para reiniciar nivel)
    public void ResetScore()
    {
        currentScore = 0;
        currentCombo = 0;
        GameEvents.ScoreChanged(currentScore);
        GameEvents.ComboReset();
        
        if (showDebugLogs)
        {
            Debug.Log("[SCORE] 🔄 Score reseteado");
        }
    }
}
```

**Configuración en Unity**:
1. Crear GameObject vacío llamado "ScoreManager"
2. Agregar componente ScoreManager
3. Configurar valores:
   - Correct Trash Points: 100
   - Combo Multiplier: 50
   - Combo Time Window: 3

---

## 🔴 PRIORIDAD 2: Corrección de PlayerAnimation.cs

**Ubicación**: `Assets/Scripts/Player/PlayerAnimation.cs`

**REEMPLAZAR TODO EL CONTENIDO** con:

```csharp
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Animator del personaje")]
    public Animator animator;
    
    private Rigidbody rb;
    
    // Hash de parámetros del Animator (más eficiente que strings)
    private readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private readonly int IsCarryingHash = Animator.StringToHash("IsCarrying");
    
    void Start()
    {
        // Cachear referencias
        rb = GetComponent<Rigidbody>();
        
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        // Validación
        if (rb == null)
        {
            Debug.LogError("[PLAYER-ANIM] ❌ No se encontró Rigidbody en el jugador");
        }
        
        if (animator == null)
        {
            Debug.LogError("[PLAYER-ANIM] ❌ No se encontró Animator en el jugador");
        }
    }

    private void OnEnable()
    {
        GameEvents.OnBagFilled += OnBagFilled;
        GameEvents.OnBagDisposed += OnBagDisposed;
    }

    private void OnDisable()
    {
        GameEvents.OnBagFilled -= OnBagFilled;
        GameEvents.OnBagDisposed -= OnBagDisposed;
    }

    private void Update()
    {
        if (animator == null || rb == null) return;

        // Usar velocidad del Rigidbody para determinar si está caminando
        bool isWalking = rb.linearVelocity.magnitude > 0.1f;
        animator.SetBool(IsWalkingHash, isWalking);
    }

    private void OnBagFilled()
    {
        if (animator != null)
        {
            animator.SetBool(IsCarryingHash, true);
            Debug.Log("[PLAYER-ANIM] 🎒 Animación de carga activada");
        }
    }

    private void OnBagDisposed()
    {
        if (animator != null)
        {
            animator.SetBool(IsCarryingHash, false);
            Debug.Log("[PLAYER-ANIM] 🎒 Animación de carga desactivada");
        }
    }
}
```

---

## 🟡 PRIORIDAD 3: Crosshair Mejorado

**Ubicación**: `Assets/Scripts/Player/Crosshair.cs`

**REEMPLAZAR TODO EL CONTENIDO** con:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [Header("Crosshair Sprites")]
    [Tooltip("Sprite normal (blanco)")]
    public Sprite normalCrosshair;
    
    [Tooltip("Sprite cuando puede interactuar (amarillo)")]
    public Sprite interactableCrosshair;
    
    [Tooltip("Sprite cuando basurero es correcto (verde)")]
    public Sprite correctBinCrosshair;
    
    [Tooltip("Sprite cuando basurero es incorrecto (rojo)")]
    public Sprite incorrectBinCrosshair;
    
    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color interactableColor = new Color(1f, 0.92f, 0.016f); // Amarillo
    public Color correctColor = new Color(0.3f, 0.8f, 0.3f); // Verde
    public Color incorrectColor = new Color(0.95f, 0.26f, 0.21f); // Rojo
    
    [Header("Animation")]
    [Tooltip("Velocidad de transición de color")]
    public float colorTransitionSpeed = 10f;
    
    private Image crosshairImage;
    private PlayerInteraction playerInteraction;
    private CrosshairState currentState = CrosshairState.Normal;
    private Color targetColor;
    
    void Start()
    {
        crosshairImage = GetComponent<Image>();
        playerInteraction = FindObjectOfType<PlayerInteraction>();
        
        if (crosshairImage == null)
        {
            Debug.LogError("[CROSSHAIR] ❌ No se encontró componente Image");
        }
        
        if (playerInteraction == null)
        {
            Debug.LogError("[CROSSHAIR] ❌ No se encontró PlayerInteraction en la escena");
        }
        
        targetColor = normalColor;
    }
    
    void Update()
    {
        UpdateCrosshairState();
        
        // Transición suave de color
        if (crosshairImage != null)
        {
            crosshairImage.color = Color.Lerp(
                crosshairImage.color, 
                targetColor, 
                Time.deltaTime * colorTransitionSpeed
            );
        }
    }
    
    private void UpdateCrosshairState()
    {
        if (playerInteraction == null || Camera.main == null) return;
        
        // Raycast para detectar qué está mirando el jugador
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(
            Camera.main.transform.position, 
            Camera.main.transform.forward, 
            out hit, 
            playerInteraction.interactionDistance,
            playerInteraction.interactableLayer
        );
        
        if (hitSomething)
        {
            // Si el jugador tiene un objeto en la mano
            if (playerInteraction.HasObject)
            {
                CheckBinCompatibility(hit);
            }
            // Si puede interactuar con algo
            else if (CanInteractWith(hit))
            {
                SetCrosshairState(CrosshairState.Interactable);
            }
            else
            {
                SetCrosshairState(CrosshairState.Normal);
            }
        }
        else
        {
            SetCrosshairState(CrosshairState.Normal);
        }
    }
    
    private void CheckBinCompatibility(RaycastHit hit)
    {
        // Buscar TrashCan en el objeto golpeado
        TrashCan bin = hit.collider.GetComponent<TrashCan>();
        if (bin == null) bin = hit.collider.GetComponentInParent<TrashCan>();
        if (bin == null) bin = hit.collider.GetComponentInChildren<TrashCan>();
        
        if (bin != null)
        {
            // Verificar si la basura es compatible con el basurero
            TrashObject trash = playerInteraction.CurrentHeldObject as TrashObject;
            
            if (trash != null)
            {
                bool isCorrect = trash.CanGoInTrashCan(bin.trashType);
                SetCrosshairState(isCorrect ? CrosshairState.CorrectBin : CrosshairState.IncorrectBin);
                return;
            }
        }
        
        SetCrosshairState(CrosshairState.Normal);
    }
    
    private bool CanInteractWith(RaycastHit hit)
    {
        // Verificar si tiene componente interactuable
        if (hit.collider.GetComponent<IInteractable>() != null) return true;
        if (hit.collider.GetComponentInParent<IInteractable>() != null) return true;
        if (hit.collider.GetComponentInChildren<IInteractable>() != null) return true;
        
        // Verificar si es un objeto recogible
        if (hit.collider.GetComponent<PickupableObject>() != null) return true;
        if (hit.collider.GetComponentInParent<PickupableObject>() != null) return true;
        if (hit.collider.GetComponentInChildren<PickupableObject>() != null) return true;
        
        return false;
    }
    
    private void SetCrosshairState(CrosshairState newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;
        
        switch (newState)
        {
            case CrosshairState.Normal:
                if (normalCrosshair != null) crosshairImage.sprite = normalCrosshair;
                targetColor = normalColor;
                break;
                
            case CrosshairState.Interactable:
                if (interactableCrosshair != null) crosshairImage.sprite = interactableCrosshair;
                targetColor = interactableColor;
                break;
                
            case CrosshairState.CorrectBin:
                if (correctBinCrosshair != null) crosshairImage.sprite = correctBinCrosshair;
                targetColor = correctColor;
                break;
                
            case CrosshairState.IncorrectBin:
                if (incorrectBinCrosshair != null) crosshairImage.sprite = incorrectBinCrosshair;
                targetColor = incorrectColor;
                break;
        }
    }
    
    private enum CrosshairState
    {
        Normal,
        Interactable,
        CorrectBin,
        IncorrectBin
    }
}
```

**Configuración en Unity**:
1. Seleccionar el GameObject del Crosshair en la UI
2. Asignar sprites (puedes usar el mismo sprite para todos si no tienes diferentes)
3. Ajustar colores si lo deseas
4. Color Transition Speed: 10

---

## 🟡 PRIORIDAD 4: UIManager Expandido

**Ubicación**: `Assets/Scripts/Systems/Managers/UIManager.cs`

**REEMPLAZAR TODO EL CONTENIDO** con:

```csharp
using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI References - Existentes")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI trashCountText;
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject bagFullIndicator;
    
    [Header("UI References - Nuevas")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public GameObject comboPanel;
    public Animator comboAnimator;
    
    [Header("Combo Settings")]
    public float comboPanelDuration = 2f;
    
    private Coroutine hideComboCoroutine;

    private void OnEnable()
    {
        // Eventos existentes
        GameEvents.OnTrashCountUpdated += UpdateTrashCount;
        GameEvents.OnGameOver += ShowGameOverScreen;
        GameEvents.OnBagFilled += ShowBagFull;
        GameEvents.OnBagDisposed += HideBagFull;
        
        // Eventos nuevos
        GameEvents.OnScoreChanged += UpdateScore;
        GameEvents.OnComboIncreased += ShowCombo;
        GameEvents.OnComboReset += HideCombo;
    }

    private void OnDisable()
    {
        // Eventos existentes
        GameEvents.OnTrashCountUpdated -= UpdateTrashCount;
        GameEvents.OnGameOver -= ShowGameOverScreen;
        GameEvents.OnBagFilled -= ShowBagFull;
        GameEvents.OnBagDisposed -= HideBagFull;
        
        // Eventos nuevos
        GameEvents.OnScoreChanged -= UpdateScore;
        GameEvents.OnComboIncreased -= ShowCombo;
        GameEvents.OnComboReset -= HideCombo;
    }

    private void Update()
    {
        UpdateTimer();
    }
    
    private void UpdateTimer()
    {
        if (GameManager.Instance != null && timerText != null)
        {
            float time = GameManager.Instance.GetTimeRemaining();
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
            
            // Cambiar color si queda poco tiempo
            if (time <= 30f)
            {
                timerText.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * 2, 1));
            }
            else
            {
                timerText.color = Color.white;
            }
        }
    }

    private void UpdateTrashCount(int current, int total)
    {
        if (trashCountText != null)
        {
            trashCountText.text = $"Basura: {current} / {total}";
        }
    }
    
    private void UpdateScore(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Puntos: {newScore:N0}";
        }
    }
    
    private void ShowCombo(int comboCount)
    {
        if (comboText != null)
        {
            comboText.text = $"COMBO x{comboCount}!";
        }
        
        if (comboPanel != null)
        {
            comboPanel.SetActive(true);
        }
        
        if (comboAnimator != null)
        {
            comboAnimator.SetTrigger("Pop");
        }
        
        // Cancelar corrutina anterior si existe
        if (hideComboCoroutine != null)
        {
            StopCoroutine(hideComboCoroutine);
        }
        
        // Ocultar panel después de un tiempo
        hideComboCoroutine = StartCoroutine(HideComboAfterDelay());
    }
    
    private IEnumerator HideComboAfterDelay()
    {
        yield return new WaitForSeconds(comboPanelDuration);
        HideCombo();
    }
    
    private void HideCombo()
    {
        if (comboPanel != null)
        {
            comboPanel.SetActive(false);
        }
    }

    private void ShowGameOverScreen(bool isWin)
    {
        if (isWin && winScreen != null)
        {
            winScreen.SetActive(true);
            
            // Mostrar score final en pantalla de victoria
            TextMeshProUGUI finalScoreText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
            if (finalScoreText != null && ScoreManager.Instance != null)
            {
                finalScoreText.text = $"¡Ganaste!\\nPuntuación Final: {ScoreManager.Instance.GetScore():N0}";
            }
        }
        else if (!isWin && loseScreen != null)
        {
            loseScreen.SetActive(true);
        }
    }

    private void ShowBagFull()
    {
        if (bagFullIndicator != null)
        {
            bagFullIndicator.SetActive(true);
        }
    }

    private void HideBagFull()
    {
        if (bagFullIndicator != null)
        {
            bagFullIndicator.SetActive(false);
        }
    }
}
```

**Configuración en Unity**:
1. Crear TextMeshProUGUI para Score (arriba a la izquierda)
2. Crear Panel para Combo (centro superior)
3. Crear TextMeshProUGUI para Combo dentro del panel
4. Crear Animator para el panel de combo con trigger "Pop"
5. Asignar todas las referencias en el Inspector

---

## 📝 INSTRUCCIONES DE USO

### Para cada archivo:

1. **Crear el archivo**:
   - Click derecho en la carpeta indicada
   - Create → C# Script
   - Nombrar exactamente como se indica

2. **Copiar el código**:
   - Abrir el archivo en tu editor
   - Seleccionar todo (Ctrl+A)
   - Pegar el código copiado

3. **Guardar**:
   - Guardar el archivo (Ctrl+S)
   - Volver a Unity y esperar a que compile

4. **Configurar en Unity**:
   - Seguir las instrucciones específicas de cada archivo
   - Asignar referencias en el Inspector

---

## ✅ ORDEN DE IMPLEMENTACIÓN RECOMENDADO

1. ✅ ScoreManager.cs (30 min)
2. ✅ PlayerAnimation.cs (5 min)
3. ✅ Crosshair.cs (15 min)
4. ✅ UIManager.cs (30 min)

**Total**: ~1.5 horas

---

*Código listo para implementar - 26 de Noviembre de 2025*
