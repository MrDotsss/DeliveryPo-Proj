using UnityEngine;
using System;

public enum GameState
{
    MainMenu,
    Loading,
    Playing,
    Paused,
    Cutscene,
    GameOver,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        DontDestroyOnLoad(this);
    }

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    public event Action<GameState> OnGameStateChanged;

    private int pauseRequestCount = 0; // Track multiple pause callers

    public void RequestPause()
    {
        pauseRequestCount++;
        UpdatePauseState();
    }

    public void RequestUnpause()
    {
        pauseRequestCount = Mathf.Max(0, pauseRequestCount - 1);
        UpdatePauseState();
    }

    private void UpdatePauseState()
    {
        bool shouldPause = pauseRequestCount > 0;
        if (shouldPause && CurrentState != GameState.Paused)
        {
            SetGameState(GameState.Paused);
        }
        else if (!shouldPause && CurrentState == GameState.Paused)
        {
            SetGameState(GameState.Playing);
        }
    }

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Paused:
                Time.timeScale = 0;
                break;
            case GameState.Playing:
                Time.timeScale = 1;
                break;
            case GameState.MainMenu:
                break;
            case GameState.Loading:
                break;
            case GameState.Cutscene:
                break;
            case GameState.GameOver:
                break;
            default:
                Time.timeScale = 0;
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }


    // Other game state methods, scene loading, etc.
}
