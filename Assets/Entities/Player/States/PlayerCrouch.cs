using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouch : PlayerState
{
    public override EPlayerStates StateType => EPlayerStates.Crouch;

    public override void Enter(Dictionary<string, object> data = null)
    {

    }

    public override void Exit()
    {

    }

    public override void PhysicsUpdate()
    {
        if (player.IsOnSlope)
        {
            player.DoMove(player.GetSlopeDirection(), player.crouchSpeed, player.acceleration);
        }
        else
        {
            player.DoMove(player.GetMoveDirection(), player.crouchSpeed, player.acceleration);
        }

        player.UseGravity(!player.IsOnSlope);
    }

    public override void UpdateState()
    {
        if (player.GetInputDir().magnitude != 0)
        {
            player.cam.BobCam(Vector2.one * 0.08f, 10f); //camera bobbing
        }
        else
        {
            player.cam.BobCam(Vector2.zero, 8f);
        }

        player.cam.TiltCam(player.GetInputDir().x * -1.5f); //set the tilt depends on the left and right inputs
        player.cam.ZoomCam(50);

        if (!player.IsOnFloor)
        {
            stateMachine.TransitionTo(EPlayerStates.Air);
        }

        if (player.input.actions["crouch"].IsPressed())
        {
            player.DoCrouch(true);
        }
        else if (!player.input.actions["crouch"].IsPressed() && !player.IsOnCeiling)
        {
            player.DoCrouch(false);

            stateMachine.TransitionTo(EPlayerStates.Idle);
        }
    }
}
