using UnityEngine;
using System;

/// <summary>
/// Manages the overall game state and pause system.
/// Handles transitions between states like MainMenu, Playing, Paused, etc.
/// Implements a pause request counter to support multiple pause callers.
/// </summary>
public class GameManager : BaseManager<GameManager>
{
    /// <summary>
    /// Enumeration of possible game states.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Loading,
        Playing,
        Paused,
        Cutscene,
        GameOver,
    }

    /// <summary>
    /// The initial game state when the game starts.
    /// </summary>
    public GameState initialState = GameState.MainMenu;

    /// <summary>
    /// Current active game state.
    /// </summary>
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    /// <summary>
    /// Event fired when the game state changes.
    /// </summary>
    public event Action<GameState> OnGameStateChanged;

    // Tracks the number of active pause requests
    private int pauseRequestCount = 0;

    private Player player;

    public DayNightVolume dayNightVolume;

    /// <summary>
    /// Registers a request to pause the game.
    /// Multiple callers can request pause and the game remains paused until all unpause.
    /// </summary>
    public void RequestPause()
    {
        pauseRequestCount++;
        UpdatePauseState();
    }

    /// <summary>
    /// Registers a request to unpause the game.
    /// Decreases pause request count and updates pause state accordingly.
    /// </summary>
    public void RequestUnpause()
    {
        pauseRequestCount = Mathf.Max(0, pauseRequestCount - 1);
        UpdatePauseState();
    }

    /// <summary>
    /// Checks pause request count and updates the game state to Paused or Playing.
    /// </summary>
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

    /// <summary>
    /// Sets the current game state and applies any related side effects (e.g., time scale).
    /// Invokes the OnGameStateChanged event.
    /// </summary>
    /// <param name="newState">The new game state to switch to.</param>
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
                // Handle MainMenu-specific logic here
                break;
            case GameState.Loading:
                // Handle Loading-specific logic here
                break;
            case GameState.Cutscene:
                // Handle Cutscene-specific logic here
                break;
            case GameState.GameOver:
                // Handle GameOver-specific logic here
                break;
            default:
                Time.timeScale = 0;
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// Finds and returns the Player instance in the scene.
    /// Caches the reference for future calls.
    /// </summary>
    /// <returns>The Player component.</returns>
    public Player GetPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        return player;
    }
}
