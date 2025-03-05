using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//dito na yung state machine ng isang owner
//make sure na nasa labas sya for easy detection ng base states
//naming convention should be E-(name ng owner)-States
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
