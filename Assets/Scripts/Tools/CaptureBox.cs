using System;
using UnityEngine;

/// <summary>
/// Simple component that triggers a capture event with a filename when invoked.
/// Can be used to represent an object that can be "captured" (e.g., photo taken).
/// </summary>
public class CaptureBox : MonoBehaviour
{
    // Event fired when a capture occurs, passing the filename as a parameter
    public event Action<string> OnCapture;

    [Tooltip("Base filename used when capturing.")]
    public string fileName = $"Photo_";

    [TextArea(3, 5), Tooltip("Description or notes about this capture box.")]
    public string description;

    [Tooltip("Minimum distance required for a capture to be valid.")]
    public float minDistance = 3f;

    [Tooltip("Maximum distance allowed for a capture to be valid.")]
    public float maxDistance = 5f;

    /// <summary>
    /// Triggers the OnCapture event, passing the filename.
    /// Intended to be called when the capture action happens.
    /// </summary>
    public void Capture()
    {
        OnCapture?.Invoke(fileName);
    }
}
