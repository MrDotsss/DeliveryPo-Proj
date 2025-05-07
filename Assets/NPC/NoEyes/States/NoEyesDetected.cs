using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoEyesDetected : NoEyesState
{
    public override NoEyesStateMachine.States StateType => NoEyesStateMachine.States.Detected;

    private CustomTimer timer;

    public override void Enter(Dictionary<string, object> data = null)
    {
        npc.DoAnimate("Detected", 0.5f);

        timer = gameObject.AddComponent<CustomTimer>();
        timer.StartTimer(3f);
    }

    public override void Exit()
    {
        Destroy(timer);
    }

    public override void PhysicsUpdate()
    {
        npc.DoMove(0, npc.friction);
    }

    public override void UpdateState()
    {
        if (!npc.lineOfSight.CanSeeTarget())
        {
            stateMachine.TransitionTo(NoEyesStateMachine.States.Idle);
            return;
        }

        if (!timer.IsRunning)
        {
            stateMachine.TransitionTo(NoEyesStateMachine.States.Chase);
        }
    }
}
