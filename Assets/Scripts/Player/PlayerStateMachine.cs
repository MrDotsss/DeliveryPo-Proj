using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPlayerStates
{
    Idle,
    Walk,
    Run,
    Crouch,
    Air
}
public class PlayerStateMachine : StateMachine<EPlayerStates>
{
    
}
