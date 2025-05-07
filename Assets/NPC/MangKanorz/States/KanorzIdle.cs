using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanorzIdle : MangKanorzState
{
    public override EKanorzState StateType => EKanorzState.Idle;

    public override void Enter(Dictionary<string, object> data = null)
    {
        npc.DoAnimate("Idle");
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
        if (DialogueManager.Instance.IsDialogueActive)
        {
            return;
        }

        if (npc.accepted && !npc.questItem.IsFinished)
        {
            if (Vector3.Distance(npc.transform.position, npc.GetPlayer().transform.position) > 2)
            {
                stateMachine.TransitionTo(EKanorzState.Walk);
            }
        }
        else if (Vector3.Distance(npc.transform.position, npc.GetPlayer().transform.position) < 3)
        {
            npc.LookAt(npc.GetPlayer().transform.position);

        }
    }
}
