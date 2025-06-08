using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class ensures the GameObject it is attached to persists across scenes.
/// </summary>
public class ManagerInitiator : MonoBehaviour
{
    /// <summary>
    /// Called when the script instance is being loaded.
    /// Prevents this GameObject from being destroyed when loading a new scene.
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
