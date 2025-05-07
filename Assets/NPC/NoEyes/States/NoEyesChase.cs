using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoEyesChase : NoEyesState
{
    public override NoEyesStateMachine.States StateType => NoEyesStateMachine.States.Chase;

    public override void Enter(Dictionary<string, object> data = null)
    {
        npc.DoAnimate("Chase", 0.8f);
    }

    public override void Exit()
    {

    }

    public override void PhysicsUpdate()
    {
        npc.DoMove(npc.speed, npc.acceleration);
    }

    public override void UpdateState()
    {
        npc.LookAt(npc.GetPlayer().transform.position, 64);

        if (Vector3.Distance(npc.transform.position, npc.GetPlayer().transform.position) < 2f)
        {
            stateMachine.TransitionTo(NoEyesStateMachine.States.Attack);
            return;
        }

        if (!npc.lineOfSight.IsInRange(npc.GetPlayer().transform))
        {
            stateMachine.TransitionTo(NoEyesStateMachine.States.Idle);
        }
    }
}
