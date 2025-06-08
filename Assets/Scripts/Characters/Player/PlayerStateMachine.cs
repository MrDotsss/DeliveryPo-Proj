using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerStateMachine : StateMachine<PlayerStateMachine.PlayerStates>
{
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
