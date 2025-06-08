using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : PlayerState
{
    public override PlayerStateMachine.PlayerStates StateType => PlayerStateMachine.PlayerStates.Idle;

    public override void EnterState(Dictionary<string, object> data = null)
    {

    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        player.ApplyVelocity(Vector3.zero, 0, player.friction);
        player.Move();

        if (!player.IsOnFloor)
        {
            stateMachine.TransitionTo(PlayerStateMachine.PlayerStates.Air);
        }
        else if (InputManager.Instance.Move.magnitude != 0)
        {
            stateMachine.TransitionTo(PlayerStateMachine.PlayerStates.Walk);
        }
        else if (InputManager.Instance.Crouch)
        {
            stateMachine.TransitionTo(PlayerStateMachine.PlayerStates.Crouch);
        }
        else if (InputManager.Instance.Jump)
        {
            stateMachine.TransitionTo(PlayerStateMachine.PlayerStates.Air, new Dictionary<string, object> { { "jump", player.jumpStrength } });
        }
    }
}
