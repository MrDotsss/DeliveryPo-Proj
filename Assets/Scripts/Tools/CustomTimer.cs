using System;
using UnityEngine;

public class CustomTimer : MonoBehaviour
{
    public float duration = 1f;
    public bool autostart = false;
    public float ElapsedTime { private set; get; }
    public bool IsRunning { private set; get; }

    public event Action OnTimeout;

    private void Start()
    {
        if (autostart) StartTimer();
    }

    public void StartTimer(float newDuration = -1f)
    {
        if (newDuration > 0) duration = newDuration;
        ElapsedTime = 0f;
        IsRunning = true;
    }

    public void StopTimer()
    {
        IsRunning = false;
    }

    public void ResetTimer()
    {
        ElapsedTime = 0f;
        IsRunning = false;
    }

    private void Update()
    {
        if (!IsRunning) return;

        ElapsedTime += Time.deltaTime;

        if (ElapsedTime >= duration)
        {
            IsRunning = false;
            OnTimeout?.Invoke();
        }
    }
}
