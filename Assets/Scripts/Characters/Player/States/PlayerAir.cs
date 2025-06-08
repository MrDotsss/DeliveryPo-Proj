using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAir : PlayerState
{
    public override PlayerStateMachine.PlayerStates StateType => PlayerStateMachine.PlayerStates.Air;

    public override void EnterState(Dictionary<string, object> data = null)
    {
        if(data != null && data.ContainsKey("jump"))
        {
            player.DoJump((float)data["jump"]);
        }
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        player.ApplyVelocity(player.GetMoveDirection(), player.walkSpeed, player.acceleration);
        player.ApplyGravity();
        player.Move();

        if (player.IsOnFloor)
        {
            stateMachine.TransitionTo(PlayerStateMachine.PlayerStates.Idle);
        }
    }
}
