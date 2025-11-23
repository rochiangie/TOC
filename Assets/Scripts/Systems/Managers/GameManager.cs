using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Settings")]
    public float levelTimeSeconds = 300f;

    public GameState CurrentState { get; private set; }
    private float timeRemaining;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartLevel();
    }

    private void StartLevel()
    {
        timeRemaining = levelTimeSeconds;
        SetState(GameState.Playing);
        GameEvents.OnLevelStart?.Invoke();
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                SetState(GameState.Lost);
            }
        }
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        GameEvents.OnGameStateChanged?.Invoke(newState);

        if (newState == GameState.Won)
        {
            GameEvents.OnGameOver?.Invoke(true);
            Debug.Log("Game Won!");
        }
        else if (newState == GameState.Lost)
        {
            GameEvents.OnGameOver?.Invoke(false);
            Debug.Log("Game Lost!");
        }
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }
}
