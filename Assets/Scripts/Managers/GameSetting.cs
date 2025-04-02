using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    public static GameSetting Instance { get; private set; }

    [Header("Control")]
    public float mouseXSensitivity = 5f;
    public float mouseYSensitivity = 6f;
    [Header("Video")]
    public int maxFPS = 120;

    [Header("GamePlay")]
    public Color playerColor = Color.blue;
    public Color npcColor = Color.yellow;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = maxFPS;
    }
}
