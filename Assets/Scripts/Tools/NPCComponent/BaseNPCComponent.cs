using System;
using UnityEngine;

/// <summary>
/// Abstract base class for components that belong to a BaseNPC.
/// Provides lifecycle methods and owner assignment.
/// </summary>
public abstract class BaseNPCComponent : MonoBehaviour
{
    // Reference to the owning BaseNPC
    protected BaseNPC Owner { get; private set; }

    // Event triggered when this component finishes its task or action
    public event Action<BaseNPCComponent> OnComponentFinished;

    // If true, component actions loop and never signal finished
    public bool loop = false;

    /// <summary>
    /// Initialization logic for the component, to be implemented by subclasses.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Activates or starts the component's behavior, to be implemented by subclasses.
    /// </summary>
    public abstract void Activate();

    /// <summary>
    /// Call to signal that this component has finished its work,
    /// unless looping is enabled.
    /// </summary>
    protected void FinishComponent()
    {
        if (loop) return;

        OnComponentFinished?.Invoke(this);
    }

    /// <summary>
    /// Assigns the owner NPC of this component.
    /// </summary>
    /// <param name="owner">The BaseNPC owning this component.</param>
    public void SetOwner(BaseNPC owner)
    {
        Owner = owner;
    }
}
