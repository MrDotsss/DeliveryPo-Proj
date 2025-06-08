using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouch : PlayerState
{
    public override PlayerStateMachine.PlayerStates StateType => PlayerStateMachine.PlayerStates.Crouch;

    public override void EnterState(Dictionary<string, object> data = null)
    {
        player.DoCrouch(true);
    }

    public override void ExitState()
    {
        player.DoCrouch(false);
    }

    public override void UpdateState()
    {
        if (InputManager.Instance.Move.magnitude != 0)
        {
            player.ApplyVelocity(player.GetMoveDirection(), player.crouchSpeed, player.acceleration);
        }
        else
        {
            player.ApplyVelocity(Vector3.zero, 0, player.friction);
        }

        player.Move();

        if (!player.IsOnFloor)
        {
            stateMachine.TransitionTo(PlayerStateMachine.PlayerStates.Air);
        }
        else if (!InputManager.Instance.Crouch && !player.IsOnCeiling)
        {
            stateMachine.TransitionTo(PlayerStateMachine.PlayerStates.Idle);
        }
        else if (InputManager.Instance.Jump)
        {
            stateMachine.TransitionTo(PlayerStateMachine.PlayerStates.Air, new Dictionary<string, object> { { "jump", player.jumpStrength } });
        }
    }
}
