using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for all phone UI states.
/// Provides a reference to the parent PhoneUI and handles basic initialization.
/// </summary>
public abstract class PhoneUIState : BaseState<PhoneUIStateMachine.PhoneStates>
{
    // Reference to the PhoneUI component this state belongs to
    protected PhoneUI phone;

    /// <summary>
    /// Initializes the state by linking it to the parent PhoneUI component
    /// and disabling the GameObject by default.
    /// </summary>
    /// <param name="stateMachine">The state machine managing this state</param>
    public override void Initialize(StateMachine<PhoneUIStateMachine.PhoneStates> stateMachine)
    {
        base.Initialize(stateMachine);

        // Get the PhoneUI component from the parent GameObject
        phone = stateMachine.GetComponentInParent<PhoneUI>();

        // Disable this state's GameObject initially
        gameObject.SetActive(false);
    }
}
