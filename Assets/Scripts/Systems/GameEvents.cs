using System;
using UnityEngine;

public enum GameState { Playing, Won, Lost, Paused }

/// <summary>
/// Sistema de eventos global para desacoplar sistemas.
/// Incluye eventos originales + eventos nuevos del proyecto anterior.
/// </summary>
public static class GameEvents
{
    // ===================================
    // EVENTOS ORIGINALES (Compatibilidad)
    // ===================================
    
    public static Action<GameState> OnGameStateChanged;
    public static Action OnLevelStart;
    public static Action<bool> OnGameOver; // true = win, false = loss
    public static Action OnTrashCollected;
    public static Action OnBagFilled;
    public static Action OnBagDisposed;
    public static Action<int, int> OnTrashCountUpdated; // currentCollected, totalInLevel
    public static Action<string> OnPlaySFX;

    // ===================================
    // EVENTOS NUEVOS (Proyecto Anterior)
    // ===================================
    
    // Estado del Juego
    public static Action OnGamePaused;
    public static Action OnGameResumed;

    // Basura (Nuevos)
    public static Action OnTrashPickedUp;
    public static Action<TrashObject.TrashType> OnTrashDisposed;
    public static Action<bool, TrashCan.TrashType> OnTrashSorted; // (isCorrect, binType)
    
    // Puntuación
    public static Action<int> OnScoreChanged;
    public static Action<int> OnComboIncreased;
    public static Action OnComboReset;

    // Tiempo
    public static Action<float> OnTimeUpdate;
    public static Action OnTimeWarning;
    public static Action OnTimeUp;

    // UI
    public static Action<string, float> OnShowMessage; // (message, duration)
    public static Action<string> OnShowError;
    public static Action<string> OnShowSuccess;

    // ===================================
    // MÉTODOS DE INVOCACIÓN (Opcionales)
    // ===================================
    
    // Solo para los eventos nuevos que no tienen invocadores directos
    
    public static void TrashPickedUp()
    {
        OnTrashPickedUp?.Invoke();
    }
    
    public static void TrashDisposed(TrashObject.TrashType type)
    {
        OnTrashDisposed?.Invoke(type);
    }
    
    public static void TrashSorted(bool isCorrect, TrashCan.TrashType binType)
    {
        OnTrashSorted?.Invoke(isCorrect, binType);
    }
    
    public static void GamePaused()
    {
        OnGamePaused?.Invoke();
    }
    
    public static void GameResumed()
    {
        OnGameResumed?.Invoke();
    }
    
    public static void ScoreChanged(int newScore)
    {
        OnScoreChanged?.Invoke(newScore);
    }
    
    public static void ComboIncreased(int comboCount)
    {
        OnComboIncreased?.Invoke(comboCount);
    }
    
    public static void ComboReset()
    {
        OnComboReset?.Invoke();
    }
    
    public static void TimeUpdate(float timeRemaining)
    {
        OnTimeUpdate?.Invoke(timeRemaining);
    }
    
    public static void TimeWarning()
    {
        OnTimeWarning?.Invoke();
    }
    
    public static void TimeUp()
    {
        OnTimeUp?.Invoke();
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
