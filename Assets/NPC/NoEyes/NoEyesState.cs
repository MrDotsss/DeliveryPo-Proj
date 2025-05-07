using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NoEyesState : BaseState<NoEyesStateMachine.States>
{
    protected NoEyes npc;

    public override void Initialize(StateMachine<NoEyesStateMachine.States> stateMachine)
    {
        base.Initialize(stateMachine);

        npc = stateMachine.GetComponent<NoEyes>();
    }
}
