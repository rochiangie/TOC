# 📚 Patrones y Mejores Prácticas del Proyecto Anterior
## Análisis y Aplicación al Proyecto TOC

---

## 🎯 PATRONES PRINCIPALES IDENTIFICADOS

### 1. **SINGLETON PATTERN** ⭐⭐⭐
**Uso**: Managers globales que persisten entre escenas

**Ejemplo del proyecto anterior (AudioManager.cs)**:
```csharp
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                Debug.LogWarning("Instancia duplicada destruida.");
            }
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

**✅ APLICAR A**: GameManager, AudioManager, UIManager

---

### 2. **EVENT SYSTEM (Desacoplamiento)** ⭐⭐⭐
**Uso**: Comunicación entre sistemas sin dependencias directas

**Ejemplo del proyecto anterior (GameEvents.cs)**:
```csharp
public static class GameEvents
{
    // Declaración de eventos
    public static event Action OnAnyDirtCleaned;
    public static event Action<int, int> OnProgressUpdate;
    public static event Action<bool> OnGameResult;
    
    // Métodos de invocación
    public static void DirtCleaned()
    {
        OnAnyDirtCleaned?.Invoke();
    }
    
    public static void Progress(int cleaned, int total)
    {
        OnProgressUpdate?.Invoke(cleaned, total);
    }
}
```

**Suscripción**:
```csharp
void OnEnable()
{
    GameEvents.OnAnyDirtCleaned += HandleDirtCleaned;
    GameEvents.OnProgressUpdate += UpdateUI;
}

void OnDisable()
{
    GameEvents.OnAnyDirtCleaned -= HandleDirtCleaned;
    GameEvents.OnProgressUpdate -= UpdateUI;
}
```

**✅ APLICAR A**: 
- Eventos de basura recogida
- Eventos de puntuación
- Eventos de tiempo
- Eventos de UI

---

### 3. **PROPERTIES (Encapsulación)** ⭐⭐
**Uso**: Acceso controlado a variables privadas

**Ejemplo del proyecto anterior (HeldItemSlot.cs)**:
```csharp
private GameObject currentToolObject;
private ToolDescriptor currentToolDescriptor;

// Propiedades públicas de solo lectura
public ToolDescriptor CurrentTool => currentToolDescriptor;
public bool HasTool => currentToolObject != null;
```

**Ventajas**:
- ✅ Encapsulación
- ✅ Solo lectura desde fuera
- ✅ Sintaxis limpia

**✅ APLICAR A**: PlayerInteraction, TrashManager

---

### 4. **SERIALIZED CLASSES** ⭐⭐
**Uso**: Configuración en el Inspector

**Ejemplo del proyecto anterior (AudioManager.cs)**:
```csharp
[System.Serializable]
public class CharacterMusicPair
{
    public string characterID;
    public AudioClip musicClip;
}

[Header("Música")]
public List<CharacterMusicPair> characterMusicList = new List<CharacterMusicPair>();
```

**✅ APLICAR A**: 
- Configuración de tipos de basura
- Pares de basurero-tipo
- Configuración de audio

---

### 5. **TOOLTIPS Y HEADERS** ⭐
**Uso**: Documentación en el Inspector

**Ejemplo del proyecto anterior**:
```csharp
[Header("UI References")]
[Tooltip("El GameObject del panel de selección de herramientas")]
public GameObject selectionPanelUI;

[Header("Interoperabilidad UI")]
[Tooltip("El script que controla el movimiento de la cámara/mouse")]
public MonoBehaviour mouseLook;
```

**✅ APLICAR A**: Todos los scripts públicos

---

### 6. **SENDMESSAGE PATTERN** ⭐
**Uso**: Comunicación flexible entre componentes

**Ejemplo del proyecto anterior (ToolHandler.cs)**:
```csharp
if (mouseLook != null)
    mouseLook.SendMessage("SetControlsActive", false, SendMessageOptions.DontRequireReceiver);
```

**Ventajas**:
- ✅ No requiere conocer el tipo exacto
- ✅ Flexible
- ⚠️ Menos performante que eventos

**✅ APLICAR A**: Comunicación opcional entre sistemas

---

### 7. **COROUTINES PARA DELAYS** ⭐⭐
**Uso**: Esperas y animaciones

**Ejemplo del proyecto anterior (AudioManager.cs)**:
```csharp
private IEnumerator CheckForCharacterMusicDelayed()
{
    yield return new WaitForSeconds(CHECK_CHARACTER_DELAY);
    
    string characterID = GetSelectedCharacterID();
    if (!string.IsNullOrEmpty(characterID))
    {
        PlayCharacterMusic(characterID);
    }
}
```

**✅ YA APLICADO**: PlayerInteraction.PickUpWithDelay()

---

### 8. **DICTIONARY PARA MAPEO** ⭐⭐
**Uso**: Búsqueda rápida de datos

**Ejemplo del proyecto anterior (AudioManager.cs)**:
```csharp
private Dictionary<string, AudioClip> characterMusicMap = new Dictionary<string, AudioClip>();

private void MapCharacterMusic()
{
    characterMusicMap.Clear();
    foreach (var pair in characterMusicList)
    {
        if (!string.IsNullOrEmpty(pair.characterID) && pair.musicClip != null)
        {
            if (!characterMusicMap.ContainsKey(pair.characterID))
                characterMusicMap.Add(pair.characterID, pair.musicClip);
        }
    }
}
```

**✅ APLICAR A**: Mapeo de tipos de basura a basureros

---

### 9. **PLAYERPREFS PARA PERSISTENCIA** ⭐
**Uso**: Guardar configuración entre sesiones

**Ejemplo del proyecto anterior (AudioManager.cs)**:
```csharp
private const string MUSIC_TOGGLE_KEY = "MusicMuted";

public void ToggleMusic(bool musicOn)
{
    musicSource.mute = !musicOn;
    PlayerPrefs.SetInt(MUSIC_TOGGLE_KEY, musicSource.mute ? 1 : 0);
    PlayerPrefs.Save();
}

public bool IsMusicEnabled()
{
    return PlayerPrefs.GetInt(MUSIC_TOGGLE_KEY, 0) == 0;
}
```

**✅ APLICAR A**: Configuración de audio, sensibilidad, etc.

---

### 10. **DEBUG LOGS ESTRUCTURADOS** ⭐⭐
**Uso**: Debugging efectivo

**Ejemplo del proyecto anterior**:
```csharp
Debug.Log($"[AUDIO] ✅ AudioManager inicializado");
Debug.LogWarning($"[AUDIO] ⚠️ No se pudo encontrar personaje");
Debug.LogError($"[AUDIO] ❌ No se puede reproducir música");
```

**Formato**:
- `[SISTEMA]` - Identificador del sistema
- `✅` - Éxito
- `⚠️` - Advertencia
- `❌` - Error
- `🔥` - Acción importante
- `🎵` - Audio
- `📦` - Objetos

**✅ YA APLICADO PARCIALMENTE**: Mejorar consistencia

---

## 🎨 PATRONES DE ARQUITECTURA

### **SEPARACIÓN DE RESPONSABILIDADES**

**Proyecto Anterior**:
```
Player/
├── PlayerMovement.cs      → Solo movimiento
├── PlayerLook.cs          → Solo cámara
├── PlayerInteraction.cs   → Solo interacciones
├── PlayerAnimation.cs     → Solo animaciones
└── HeldItemSlot.cs        → Solo gestión de items

Systems/
├── GameEvents.cs          → Solo eventos
├── AudioManager.cs        → Solo audio
├── GameUIController.cs    → Solo UI
└── TaskManager.cs         → Solo lógica de juego
```

**✅ APLICAR**: Mantener esta estructura en TOC

---

## 🔧 MEJORAS ESPECÍFICAS PARA TOC

### 1. **Mejorar GameEvents.cs**

**ACTUAL**:
```csharp
public static class GameEvents
{
    public static event Action<int, int> OnTrashCountUpdated;
    public static event Action<bool> OnGameOver;
}
```

**MEJORADO** (basado en proyecto anterior):
```csharp
public static class GameEvents
{
    // ===================================
    // 1. EVENTOS DE BASURA
    // ===================================
    public static event Action OnTrashPickedUp;
    public static event Action<TrashObject.TrashType> OnTrashDisposed;
    public static event Action<int, int> OnTrashCountUpdated; // (current, total)
    
    // ===================================
    // 2. EVENTOS DE PUNTUACIÓN
    // ===================================
    public static event Action<int> OnScoreChanged;
    public static event Action<bool, TrashCan.TrashType> OnTrashSorted; // (correct, type)
    
    // ===================================
    // 3. EVENTOS DE TIEMPO
    // ===================================
    public static event Action<float> OnTimeUpdate;
    public static event Action OnTimeWarning; // Cuando queda poco tiempo
    
    // ===================================
    // 4. EVENTOS DE JUEGO
    // ===================================
    public static event Action<bool> OnGameOver; // (won)
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;
    
    // ===================================
    // MÉTODOS DE INVOCACIÓN
    // ===================================
    public static void TrashPickedUp()
    {
        OnTrashPickedUp?.Invoke();
    }
    
    public static void TrashDisposed(TrashObject.TrashType type)
    {
        OnTrashDisposed?.Invoke(type);
    }
    
    public static void TrashSorted(bool correct, TrashCan.TrashType type)
    {
        OnTrashSorted?.Invoke(correct, type);
    }
    
    // ... etc
}
```

---

### 2. **Crear AudioManager Singleton**

```csharp
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("SFX Clips")]
    public AudioClip pickupSFX;
    public AudioClip dropSFX;
    public AudioClip correctTrashSFX;
    public AudioClip incorrectTrashSFX;
    public AudioClip trashAbsorbSFX;
    
    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1.0f;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeAudioSources();
    }
    
    private void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }
    
    public void PlayPickupSFX()
    {
        if (pickupSFX != null)
            sfxSource.PlayOneShot(pickupSFX, sfxVolume);
    }
    
    public void PlayCorrectTrashSFX()
    {
        if (correctTrashSFX != null)
            sfxSource.PlayOneShot(correctTrashSFX, sfxVolume);
    }
    
    // ... etc
}
```

---

### 3. **Mejorar PlayerInteraction con Properties**

```csharp
public class PlayerInteraction : MonoBehaviour
{
    private PickupableObject currentHeldObject;
    private bool isPickingUp = false;
    
    // Properties públicas
    public PickupableObject CurrentHeldObject => currentHeldObject;
    public bool HasObject => currentHeldObject != null;
    public bool IsPickingUp => isPickingUp;
    
    // ... resto del código
}
```

---

### 4. **Agregar Sistema de Configuración Persistente**

```csharp
public static class GameSettings
{
    private const string MOUSE_SENSITIVITY_KEY = "MouseSensitivity";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    
    public static float MouseSensitivity
    {
        get => PlayerPrefs.GetFloat(MOUSE_SENSITIVITY_KEY, 100f);
        set
        {
            PlayerPrefs.SetFloat(MOUSE_SENSITIVITY_KEY, value);
            PlayerPrefs.Save();
        }
    }
    
    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.5f);
        set
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
            PlayerPrefs.Save();
        }
    }
    
    // ... etc
}
```

---

## 📋 CHECKLIST DE IMPLEMENTACIÓN

### Prioridad Alta ⭐⭐⭐
- [ ] Expandir GameEvents.cs con más eventos
- [ ] Crear AudioManager Singleton
- [ ] Agregar Properties a PlayerInteraction
- [ ] Mejorar logs con formato estructurado
- [ ] Agregar Tooltips y Headers a todos los scripts

### Prioridad Media ⭐⭐
- [ ] Crear GameSettings para persistencia
- [ ] Implementar Dictionary para mapeo de basura
- [ ] Agregar más eventos de feedback
- [ ] Crear clases serializables para configuración

### Prioridad Baja ⭐
- [ ] Implementar sistema de debug avanzado
- [ ] Agregar más corrutinas para animaciones
- [ ] Mejorar sistema de SendMessage

---

## 💡 CONCEPTOS CLAVE APRENDIDOS

### 1. **Desacoplamiento**
- Usar eventos en lugar de referencias directas
- Permite cambiar sistemas sin romper otros

### 2. **Singleton para Managers**
- Un solo punto de acceso global
- Persiste entre escenas
- Fácil de usar: `AudioManager.Instance.PlaySFX()`

### 3. **Encapsulación**
- Variables privadas con properties públicas
- Control total sobre el acceso a datos

### 4. **Organización**
- Separar responsabilidades en scripts diferentes
- Usar carpetas lógicas (Player, Systems, Environment, etc.)

### 5. **Debugging**
- Logs estructurados con prefijos
- Emojis para identificar rápidamente
- Niveles de severidad claros

---

## 🎯 PRÓXIMOS PASOS RECOMENDADOS

1. **Implementar AudioManager** (30 min)
2. **Expandir GameEvents** (20 min)
3. **Agregar Properties a PlayerInteraction** (10 min)
4. **Mejorar logs en todos los scripts** (30 min)
5. **Crear GameSettings** (20 min)

**Total estimado**: ~2 horas

---

## 📚 RECURSOS Y REFERENCIAS

- **Singleton Pattern**: https://unity.com/how-to/create-modular-and-maintainable-code-unity
- **Event System**: https://docs.unity3d.com/Manual/UnityEvents.html
- **Properties en C#**: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties

---

*Documento generado automáticamente analizando el proyecto anterior*
*Fecha: 2025-11-25*
