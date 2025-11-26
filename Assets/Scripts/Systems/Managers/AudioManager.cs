using UnityEngine;
using System.Collections;

/// <summary>
/// Gestiona toda la reproducción de audio (Música y SFX) y persiste entre escenas.
/// Patrón Singleton para acceso global.
/// </summary>
public class AudioManager : MonoBehaviour
{
    // ===================================
    // SINGLETON
    // ===================================
    public static AudioManager Instance { get; private set; }

    // ===================================
    // AUDIO SOURCES
    // ===================================
    [Header("Audio Sources")]
    [Tooltip("Fuente para la música (debe ser loop: true)")]
    public AudioSource musicSource;
    
    [Tooltip("Fuente para efectos de sonido (PlayOneShot)")]
    public AudioSource sfxSource;

    // ===================================
    // MÚSICA
    // ===================================
    [Header("Música")]
    [Tooltip("Música de fondo del juego")]
    public AudioClip backgroundMusic;

    // ===================================
    // SFX CLIPS
    // ===================================
    [Header("SFX - Interacción")]
    [Tooltip("Sonido al recoger basura")]
    public AudioClip pickupSFX;
    
    [Tooltip("Sonido al soltar objetos")]
    public AudioClip dropSFX;
    
    [Tooltip("Sonido al clasificar basura correctamente")]
    public AudioClip correctTrashSFX;
    
    [Tooltip("Sonido al clasificar basura incorrectamente")]
    public AudioClip incorrectTrashSFX;
    
    [Tooltip("Sonido de absorción de basura en el basurero")]
    public AudioClip trashAbsorbSFX;
    
    [Header("SFX - UI")]
    [Tooltip("Sonido de clic en botones")]
    public AudioClip buttonClickSFX;
    
    [Tooltip("Sonido de victoria")]
    public AudioClip victorySFX;
    
    [Tooltip("Sonido de derrota")]
    public AudioClip defeatSFX;

    // ===================================
    // VOLÚMENES
    // ===================================
    [Header("Volúmenes")]
    [Range(0f, 1f)]
    [Tooltip("Volumen de la música")]
    public float musicVolume = 0.5f;
    
    [Range(0f, 1f)]
    [Tooltip("Volumen global de efectos de sonido")]
    public float sfxVolume = 1.0f;

    // Multiplicadores individuales de SFX
    [Header("Multiplicadores de SFX")]
    [Range(0f, 1f)] public float pickupVolumeMultiplier = 0.7f;
    [Range(0f, 1f)] public float dropVolumeMultiplier = 0.6f;
    [Range(0f, 1f)] public float correctTrashVolumeMultiplier = 0.8f;
    [Range(0f, 1f)] public float incorrectTrashVolumeMultiplier = 0.7f;
    [Range(0f, 1f)] public float absorbVolumeMultiplier = 0.5f;

    // ===================================
    // PERSISTENCIA
    // ===================================
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string MUSIC_MUTED_KEY = "MusicMuted";

    // ===================================
    // INICIALIZACIÓN
    // ===================================
    void Awake()
    {
        // Implementación de Singleton estricta
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                Debug.LogWarning("[AUDIO] ⛔ Instancia duplicada de AudioManager destruida.");
            }
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Inicializar AudioSources si faltan
        InitializeAudioSources();

        // Cargar configuración guardada
        LoadSavedSettings();

        Debug.Log("[AUDIO] ✅ AudioManager inicializado y persistente.");
    }

    void Start()
    {
        // Iniciar música de fondo
        PlayBackgroundMusic();
        
        // Suscribirse a eventos
        SubscribeToEvents();
    }

    void OnDestroy()
    {
        // Desuscribirse de eventos
        UnsubscribeFromEvents();
    }

    // ===================================
    // SUSCRIPCIÓN A EVENTOS
    // ===================================
    private void SubscribeToEvents()
    {
        GameEvents.OnTrashPickedUp += PlayPickupSFX;
        GameEvents.OnTrashSorted += OnTrashSorted;
        GameEvents.OnGameOver += OnGameOver;
    }

    private void UnsubscribeFromEvents()
    {
        GameEvents.OnTrashPickedUp -= PlayPickupSFX;
        GameEvents.OnTrashSorted -= OnTrashSorted;
        GameEvents.OnGameOver -= OnGameOver;
    }

    // ===================================
    // INICIALIZACIÓN PRIVADA
    // ===================================
    private void InitializeAudioSources()
    {
        // Inicializar Music Source
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            Debug.Log("[AUDIO] 🎵 Music Source creado automáticamente.");
        }

        // Inicializar SFX Source
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            Debug.Log("[AUDIO] 🔊 SFX Source creado automáticamente.");
        }
    }

    private void LoadSavedSettings()
    {
        // Cargar volúmenes guardados
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.5f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1.0f);
        
        bool isMusicMuted = PlayerPrefs.GetInt(MUSIC_MUTED_KEY, 0) == 1;

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
            musicSource.mute = isMusicMuted;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }

        Debug.Log($"[AUDIO] 📊 Configuración cargada - Música: {musicVolume:F2}, SFX: {sfxVolume:F2}, Muted: {isMusicMuted}");
    }

    // ===================================
    // CONTROL DE MÚSICA
    // ===================================
    public void PlayBackgroundMusic()
    {
        if (musicSource == null || backgroundMusic == null)
        {
            Debug.LogWarning("[AUDIO] ⚠️ No se puede reproducir música. Source o Clip nulo.");
            return;
        }

        if (musicSource.isPlaying && musicSource.clip == backgroundMusic)
        {
            return; // Ya está sonando
        }

        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume;
        musicSource.Play();
        
        Debug.Log("[AUDIO] 🎵 Música de fondo iniciada.");
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("[AUDIO] 🎵 Música detenida.");
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.Save();
        
        Debug.Log($"[AUDIO] 🎵 Volumen de música: {musicVolume:F2}");
    }

    public void ToggleMusic(bool enabled)
    {
        if (musicSource != null)
        {
            musicSource.mute = !enabled;
            PlayerPrefs.SetInt(MUSIC_MUTED_KEY, enabled ? 0 : 1);
            PlayerPrefs.Save();
            
            Debug.Log($"[AUDIO] 🎵 Música: {(enabled ? "ON" : "OFF")}");
        }
    }

    // ===================================
    // SFX - MÉTODOS PÚBLICOS
    // ===================================
    public void PlayPickupSFX()
    {
        PlaySFX(pickupSFX, pickupVolumeMultiplier, "Recogida");
    }

    public void PlayDropSFX()
    {
        PlaySFX(dropSFX, dropVolumeMultiplier, "Soltar");
    }

    public void PlayCorrectTrashSFX()
    {
        PlaySFX(correctTrashSFX, correctTrashVolumeMultiplier, "Clasificación Correcta");
    }

    public void PlayIncorrectTrashSFX()
    {
        PlaySFX(incorrectTrashSFX, incorrectTrashVolumeMultiplier, "Clasificación Incorrecta");
    }

    public void PlayAbsorbSFX()
    {
        PlaySFX(trashAbsorbSFX, absorbVolumeMultiplier, "Absorción");
    }

    public void PlayButtonClickSFX()
    {
        PlaySFX(buttonClickSFX, 1.0f, "Click");
    }

    public void PlayVictorySFX()
    {
        PlaySFX(victorySFX, 1.0f, "Victoria");
    }

    public void PlayDefeatSFX()
    {
        PlaySFX(defeatSFX, 1.0f, "Derrota");
    }

    // ===================================
    // SFX - MÉTODO PRIVADO GENÉRICO
    // ===================================
    private void PlaySFX(AudioClip clip, float volumeMultiplier, string sfxName)
    {
        if (sfxSource == null)
        {
            Debug.LogError($"[AUDIO-SFX] ❌ FALLO: sfxSource es NULO.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning($"[AUDIO-SFX] ⚠️ SFX '{sfxName}' no asignado.");
            return;
        }

        float finalVolume = sfxVolume * volumeMultiplier;
        sfxSource.PlayOneShot(clip, finalVolume);
        
        Debug.Log($"[AUDIO-SFX] ✅ {sfxName} (Vol: {finalVolume:F2})");
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
        
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
        
        Debug.Log($"[AUDIO] 🔊 Volumen de SFX: {sfxVolume:F2}");
    }

    // ===================================
    // CALLBACKS DE EVENTOS
    // ===================================
    private void OnTrashSorted(bool isCorrect, TrashCan.TrashType binType)
    {
        if (isCorrect)
        {
            PlayCorrectTrashSFX();
        }
        else
        {
            PlayIncorrectTrashSFX();
        }
    }

    private void OnGameOver(bool won)
    {
        if (won)
        {
            PlayVictorySFX();
        }
        else
        {
            PlayDefeatSFX();
        }
    }

    // ===================================
    // DEBUG
    // ===================================
    public void DebugAudioStatus()
    {
        Debug.Log($"\n=== AUDIO DEBUG STATUS ===");
        Debug.Log($"Music Clip: {(musicSource?.clip != null ? musicSource.clip.name : "Ninguno")}");
        Debug.Log($"Is Playing: {musicSource?.isPlaying ?? false}");
        Debug.Log($"Music Volume: {musicVolume:F2} | SFX Volume: {sfxVolume:F2}");
        Debug.Log($"Music Muted: {musicSource?.mute ?? false}");
        Debug.Log($"==========================\n");
    }
}
