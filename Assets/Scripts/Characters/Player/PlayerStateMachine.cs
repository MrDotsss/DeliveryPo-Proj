using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player-specific state machine managing player states.
/// Inherits from generic StateMachine using PlayerStates enum.
/// </summary>
public class PlayerStateMachine : StateMachine<PlayerStateMachine.PlayerStates>
{
    /// <summary>
    /// Enum representing all possible player states.
    /// </summary>
    public enum PlayerStates
    {
        Idle,
        Walk,
        Run,
        Crouch,
        Air,
        Drive,
        Talk
    }
}
