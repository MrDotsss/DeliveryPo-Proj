using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoEyesStateMachine : StateMachine<NoEyesStateMachine.States>
{
    public enum States
    {
        Idle,
        Detected,
        Chase,
        Attack
    }
}
