using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanorzTalk : MangKanorzState
{
    public override EKanorzState StateType => EKanorzState.Talk;

    public override void Enter(Dictionary<string, object> data = null)
    {
        npc.DoAnimate("Talk", 0.15f);
    }

    public override void Exit()
    {

    }

    public override void PhysicsUpdate()
    {
        if (npc.tooFar && !npc.questItem.IsFinished)
        {
            npc.DoMove(npc.speed, npc.acceleration);
        }
        else
        {
            npc.DoMove(0, npc.friction);
        }


    }

    public override void UpdateState()
    {
        npc.LookAt(npc.GetPlayer().transform.position);

        if (!DialogueManager.Instance.IsDialogueActive)
        {
            stateMachine.TransitionTo(EKanorzState.Idle);
        }
    }
}
