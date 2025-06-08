using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base abstract class for states used with the generic StateMachine.
/// Each state must define its StateType (an enum value) and implement state lifecycle methods.
/// </summary>
/// <typeparam name="EState">Enum type representing the state identifier.</typeparam>
public abstract class BaseState<EState> : MonoBehaviour where EState : System.Enum
{
    // The enum value representing this state's type
    public abstract EState StateType { get; }

    // Reference to the parent state machine controlling this state
    protected StateMachine<EState> stateMachine;

    /// <summary>
    /// Called by the StateMachine when initializing this state.
    /// Provides the reference to the parent StateMachine.
    /// </summary>
    /// <param name="stateMachine">The StateMachine this state belongs to.</param>
    public virtual void Initialize(StateMachine<EState> stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    /// <summary>
    /// Called when the state becomes active.
    /// Optional data dictionary can be passed to initialize or configure the state.
    /// </summary>
    /// <param name="data">Optional data passed to the state when entering.</param>
    public abstract void EnterState(Dictionary<string, object> data = null);

    /// <summary>
    /// Called once per frame while this state is active.
    /// Place per-frame logic here.
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// Called at fixed intervals (FixedUpdate) while this state is active.
    /// Useful for physics-related updates.
    /// </summary>
    public virtual void PhysicsUpdateState()
    {
        // Optional override in derived classes
    }

    /// <summary>
    /// Called when exiting this state.
    /// Place cleanup or transition logic here.
    /// </summary>
    public abstract void ExitState();
}
