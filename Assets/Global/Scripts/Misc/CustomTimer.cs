using System;
using UnityEngine;

// so I made my own custom timer kasi
// yawq na sa confusing timer class
// this timer is modular at the design is very human.
// see PlayerIdle script for sample implementation

public class CustomTimer : MonoBehaviour
{

    public float duration = 1f;
    public bool autostart = false;
    
    // altough naka set sila as public di sila kita sa inspector
    //yea, elapsed time getter.
    public float ElapsedTime { private set; get; }

    //setter and getter para macheck kung tumatakbo pa ba ang oras
    public bool IsRunning { private set; get; }

    // Action class is not Unity feature it's a C# feature
    // basically pagkacall ng OnTimout.Invoke() lahat ng naka 'subscribe' sa event na yan ay sabay sabay na matatawag.
    // make sure to unsubscribe after using, or else...
    public event Action OnTimeout;

    private void Start()
    {
        //start na agad
        if (autostart) StartTimer();
    }

    //can be called outside this class kasi public
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
            //here is invoke yung tatawagin nga lahat ng kasapi.
            OnTimeout?.Invoke();
        }
    }
}
