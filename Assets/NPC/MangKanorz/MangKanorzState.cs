using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MangKanorzState : BaseState<EKanorzState>
{
    protected MangKanorz npc;

    public override void Initialize(StateMachine<EKanorzState> stateMachine)
    {
        base.Initialize(stateMachine);

        npc = stateMachine.GetComponent<MangKanorz>();
    }
}
