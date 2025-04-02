using UnityEngine;
using System;

public class GameStatusManager : MonoBehaviour
{
    public static GameStatusManager Instance { get; private set; }

    [Header("Game Progress")]
    public int currentDay = 1;
    public int packagesDelivered = 0;
    public int evidencesCollected = 0;

    [Header("Trust & Suspicion")]
    private float _playerTrust = 0;

    public bool IsPaused { get; private set; }

    public float PlayerTrust
    {
        get => _playerTrust;
        private set
        {
            _playerTrust = Mathf.Clamp(value, -1, 1);
            OnTrustLevelChanged?.Invoke(_playerTrust);
            Debug.Log("Player Trust: " + _playerTrust);
        }
    }

    public event Action<float> OnTrustLevelChanged; // Event for UI or reactions

    [Header("Player Status")]
    public PlayerStatus playerState = PlayerStatus.Normal;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update trust level (with clamping)
    public void DefineTrust(float amount)
    {
        PlayerTrust = amount; // Clamping now handled in setter
    }

    // Method to update game day
    public void NextDay()
    {
        currentDay++;
        Debug.Log("Day " + currentDay);
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1;
    }
}

// Player status enum
public enum PlayerStatus
{
    Normal,
    Suspicious,
    Hunted
}
