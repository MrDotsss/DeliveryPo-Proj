using System;
using UnityEngine;

public class CaptureBox : MonoBehaviour
{
    //will be called when captured
    public event Action<string> OnCapture;

    public string fileName = $"Photo_";
    [TextArea(3, 5)] public string description; 
    public float minDistance = 3f;
    public float maxDistance = 5f;

    public void Capture()
    {
        OnCapture?.Invoke(fileName);
    }
}