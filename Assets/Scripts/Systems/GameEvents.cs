using System;
using UnityEngine;

public enum GameState { Playing, Won, Lost, Paused }

public static class GameEvents
{
    // Game State
    public static Action<GameState> OnGameStateChanged;
    public static Action OnLevelStart;
    public static Action<bool> OnGameOver; // true = win, false = loss

    // Trash & Inventory
    public static Action OnTrashCollected;
    public static Action OnBagFilled;
    public static Action OnBagDisposed;
    public static Action<int, int> OnTrashCountUpdated; // currentCollected, totalInLevel

    // Audio & UI
    public static Action<string> OnPlaySFX;
}
