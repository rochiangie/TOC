using System;
using UnityEngine;

// Enum para estados del juego
public enum GameState { Playing, Won, Lost, Paused }

/// <summary>
/// Sistema de eventos global para desacoplar sistemas.
/// Basado en patrones del proyecto anterior de TalentoTech 3D.
/// </summary>
public static class GameEvents
{
    // ===================================
    // 1. EVENTOS DE ESTADO DEL JUEGO
    // ===================================
    
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnLevelStart;
    public static event Action<bool> OnGameOver; // true = win, false = loss
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;

    // ===================================
    // 2. EVENTOS DE BASURA
    // ===================================
    
    /// <summary>Disparado cuando el jugador recoge un objeto de basura</summary>
    public static event Action OnTrashPickedUp;
    
    /// <summary>Disparado cuando se tira basura en un basurero</summary>
    public static event Action<TrashObject.TrashType> OnTrashDisposed;
    
    /// <summary>Disparado cuando se clasifica basura (correcta o incorrectamente)</summary>
    public static event Action<bool, TrashCan.TrashType> OnTrashSorted; // (isCorrect, binType)
    
    /// <summary>Actualización del contador de basura</summary>
    public static event Action<int, int> OnTrashCountUpdated; // (currentCollected, totalInLevel)
    
    public static event Action OnBagFilled;
    public static event Action OnBagDisposed;

    // ===================================
    // 3. EVENTOS DE PUNTUACIÓN
    // ===================================
    
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnComboIncreased; // Combo de clasificación correcta
    public static event Action OnComboReset;

    // ===================================
    // 4. EVENTOS DE TIEMPO
    // ===================================
    
    public static event Action<float> OnTimeUpdate; // Tiempo restante
    public static event Action OnTimeWarning; // Cuando queda poco tiempo (ej: 30 segundos)
    public static event Action OnTimeUp; // Tiempo agotado

    // ===================================
    // 5. EVENTOS DE AUDIO Y UI
    // ===================================
    
    public static event Action<string> OnPlaySFX;
    public static event Action<string, float> OnShowMessage; // (message, duration)
    public static event Action<string> OnShowError;
    public static event Action<string> OnShowSuccess;

    // ===================================
    // MÉTODOS PÚBLICOS DE INVOCACIÓN
    // ===================================
    
    // --- Estado del Juego ---
    public static void GameStateChanged(GameState newState)
    {
        OnGameStateChanged?.Invoke(newState);
        Debug.Log($"[GAME EVENTS] 🎮 Estado del juego: {newState}");
    }
    
    public static void LevelStart()
    {
        OnLevelStart?.Invoke();
        Debug.Log("[GAME EVENTS] 🚀 Nivel iniciado");
    }
    
    public static void GameOver(bool won)
    {
        OnGameOver?.Invoke(won);
        Debug.Log($"[GAME EVENTS] {(won ? "✅ Victoria" : "❌ Derrota")}");
    }
    
    public static void GamePaused()
    {
        OnGamePaused?.Invoke();
        Debug.Log("[GAME EVENTS] ⏸️ Juego pausado");
    }
    
    public static void GameResumed()
    {
        OnGameResumed?.Invoke();
        Debug.Log("[GAME EVENTS] ▶️ Juego reanudado");
    }
    
    // --- Basura ---
    public static void TrashPickedUp()
    {
        OnTrashPickedUp?.Invoke();
        Debug.Log("[GAME EVENTS] 📦 Basura recogida");
    }
    
    public static void TrashDisposed(TrashObject.TrashType type)
    {
        OnTrashDisposed?.Invoke(type);
        Debug.Log($"[GAME EVENTS] 🗑️ Basura tirada: {type}");
    }
    
    public static void TrashSorted(bool isCorrect, TrashCan.TrashType binType)
    {
        OnTrashSorted?.Invoke(isCorrect, binType);
        Debug.Log($"[GAME EVENTS] {(isCorrect ? "✅" : "❌")} Clasificación en basurero {binType}");
    }
    
    public static void TrashCountUpdated(int current, int total)
    {
        OnTrashCountUpdated?.Invoke(current, total);
        Debug.Log($"[GAME EVENTS] 📊 Progreso: {current}/{total}");
    }
    
    public static void BagFilled()
    {
        OnBagFilled?.Invoke();
        Debug.Log("[GAME EVENTS] 💼 Bolsa llena");
    }
    
    public static void BagDisposed()
    {
        OnBagDisposed?.Invoke();
        Debug.Log("[GAME EVENTS] 💼 Bolsa vaciada");
    }
    
    // --- Puntuación ---
    public static void ScoreChanged(int newScore)
    {
        OnScoreChanged?.Invoke(newScore);
        Debug.Log($"[GAME EVENTS] 🏆 Puntuación: {newScore}");
    }
    
    public static void ComboIncreased(int comboCount)
    {
        OnComboIncreased?.Invoke(comboCount);
        Debug.Log($"[GAME EVENTS] 🔥 Combo: x{comboCount}");
    }
    
    public static void ComboReset()
    {
        OnComboReset?.Invoke();
        Debug.Log("[GAME EVENTS] 💔 Combo perdido");
    }
    
    // --- Tiempo ---
    public static void TimeUpdate(float timeRemaining)
    {
        OnTimeUpdate?.Invoke(timeRemaining);
    }
    
    public static void TimeWarning()
    {
        OnTimeWarning?.Invoke();
        Debug.Log("[GAME EVENTS] ⏰ ¡Advertencia de tiempo!");
    }
    
    public static void TimeUp()
    {
        OnTimeUp?.Invoke();
        Debug.Log("[GAME EVENTS] ⏰ ¡Tiempo agotado!");
    }
    
    // --- Audio y UI ---
    public static void PlaySFX(string sfxName)
    {
        OnPlaySFX?.Invoke(sfxName);
    }
    
    public static void ShowMessage(string message, float duration = 2f)
    {
        OnShowMessage?.Invoke(message, duration);
    }
    
    public static void ShowError(string message)
    {
        OnShowError?.Invoke(message);
    }
    
    public static void ShowSuccess(string message)
    {
        OnShowSuccess?.Invoke(message);
    }
}
