using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRun : PlayerState
{
    public override PlayerStateMachine.PlayerStates StateType => PlayerStateMachine.PlayerStates.Run;

    public override void EnterState(Dictionary<string, object> data = null)
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        player.ApplyVelocity(player.GetMoveDirection(), player.runSpeed, player.acceleration);
        player.Move();

        if(!player.IsOnFloor)
        {
            stateMachine.TransitionTo(PlayerStateMachine.PlayerStates.Air);
        } else if (!InputManager.Instance.Sprint)
        {
            stateMachine.TransitionTo(PlayerStateMachine.PlayerStates.Walk);
        } else if (InputManager.Instance.Crouch)
        {
            stateMachine.TransitionTo(PlayerStateMachine.PlayerStates.Crouch);
        }
        else if (InputManager.Instance.Jump)
        {
            stateMachine.TransitionTo(PlayerStateMachine.PlayerStates.Air, new Dictionary<string, object> { { "jump", player.jumpStrength } });
        }
    }
}
