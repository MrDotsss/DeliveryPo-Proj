using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoEyesAttack : NoEyesState
{
    public override NoEyesStateMachine.States StateType => NoEyesStateMachine.States.Attack;

    public Light redLight;
    public override void Enter(Dictionary<string, object> data = null)
    {
        npc.DoAttack();
        redLight.enabled = true;
    }

    public override void Exit()
    {

    }

    public override void PhysicsUpdate()
    {
        npc.SetVelocity(0, 0, 0);
    }

    public override void UpdateState()
    {

    }
}
