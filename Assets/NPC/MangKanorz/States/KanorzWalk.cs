using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanorzWalk : MangKanorzState
{
    public override EKanorzState StateType => EKanorzState.Walk;

    public override void Enter(Dictionary<string, object> data = null)
    {
        npc.DoAnimate("Walk", 0.15f);
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
        npc.LookAt(npc.GetPlayer().transform.position);

        if (DialogueManager.Instance.IsDialogueActive)
        {
            return;
        }

        if (npc.questItem.IsFinished)
        {
            stateMachine.TransitionTo(EKanorzState.Idle);
            return;
        }

        if (npc.tooFar && npc.accepted && !npc.questItem.IsFinished)
        {
            npc.Interact();
        }

        if (Vector3.Distance(npc.transform.position, npc.GetPlayer().transform.position) < 2)
        {
            stateMachine.TransitionTo(EKanorzState.Idle);
        }
    }
}
