using System;
using UnityEngine;


public class CaptureBox : MonoBehaviour
{
    //will be called when captured
    public event Action<string> OnCapture;

    public string fileName = $"Photo_";
    public float captureDistance = 5f;

    public void Capture(string captureName)
    {
        OnCapture?.Invoke(captureName);
    }
}
