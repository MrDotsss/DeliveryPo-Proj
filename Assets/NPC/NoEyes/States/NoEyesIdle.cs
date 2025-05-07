using System.Collections.Generic;
using UnityEngine;

public class NoEyesIdle : NoEyesState
{
    public override NoEyesStateMachine.States StateType => NoEyesStateMachine.States.Idle;

    public override void Enter(Dictionary<string, object> data = null)
    {
        npc.DoAnimate("Idle", 0.5f);
    }

    public override void Exit()
    {

    }

    public override void PhysicsUpdate()
    {
        npc.DoMove(0, npc.friction);
    }

    public override void UpdateState()
    {
        if (npc.lineOfSight.CanSeeTarget())
        {
            stateMachine.TransitionTo(NoEyesStateMachine.States.Detected);
        }
    }
}
