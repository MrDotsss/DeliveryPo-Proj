using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base abstract state class specific for player states.
/// Inherits from the generic BaseState using PlayerStateMachine.PlayerStates enum.
/// Provides access to the Player component.
/// </summary>
public abstract class PlayerState : BaseState<PlayerStateMachine.PlayerStates>
{
    protected Player player; // Reference to the Player component

    /// <summary>
    /// Initializes the state with a reference to the Player component on the same GameObject as the state machine.
    /// </summary>
    /// <param name="stateMachine">The StateMachine managing this state</param>
    public override void Initialize(StateMachine<PlayerStateMachine.PlayerStates> stateMachine)
    {
        base.Initialize(stateMachine);

        player = stateMachine.GetComponent<Player>();
    }
}
