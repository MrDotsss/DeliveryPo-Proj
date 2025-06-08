using UnityEngine;

/// <summary>
/// Generic base manager class implementing a singleton pattern for MonoBehaviour-derived types.
/// Ensures only one instance of the manager exists at a time.
/// </summary>
/// <typeparam name="T">Type of the manager class inheriting from MonoBehaviour.</typeparam>
public abstract class BaseManager<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// Static singleton instance of the manager.
    /// </summary>
    public static T Instance { get; private set; }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Handles singleton initialization and ensures only one instance persists.
    /// </summary>
    protected virtual void Awake()
    {
        // If an instance already exists and it’s not this one, destroy this GameObject
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Assign this as the singleton instance
        Instance = this as T;

        Debug.Log($"{Instance.name} initialized");
    }
}
