using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for all UI panel states.
/// Each UI panel should inherit from this and implement its own behavior for entering,
/// updating, and exiting the panel.
/// </summary>
public abstract class UIState : MonoBehaviour
{
    /// <summary>
    /// The UI panel state enum this class represents.
    /// Must be overridden by derived classes.
    /// </summary>
    public abstract UIManager.EUIPanels State { get; }

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Registers this UI panel with the UIManager and disables it initially.
    /// </summary>
    protected virtual void Awake()
    {
        UIManager.Instance.RegisterUIPanel(this);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when this UI panel is entered.
    /// Override to implement initialization or setup logic.
    /// </summary>
    /// <param name="data">Optional data passed when entering the panel.</param>
    public abstract void Enter(object data = null);

    /// <summary>
    /// Called every frame while this UI panel is active.
    /// Override to implement update logic specific to this panel.
    /// </summary>
    public virtual void UpdateState()
    {
        // Optional override
    }

    /// <summary>
    /// Called when this UI panel is exited.
    /// Override to implement cleanup or teardown logic.
    /// </summary>
    public abstract void Exit();
}
