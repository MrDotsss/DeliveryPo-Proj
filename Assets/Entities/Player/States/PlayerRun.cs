using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRun : PlayerState
{
    public override EPlayerStates StateType => EPlayerStates.Run;

    public override void Enter(Dictionary<string, object> data = null)
    {

    }

    public override void Exit()
    {

    }

    public override void PhysicsUpdate()
    {
        //if nasa slope tayo we move in a slope direction instead
        if (player.IsOnSlope)
        {
            player.DoMove(player.GetSlopeDirection(), player.runSpeed, player.acceleration);
        }
        else
        {
            player.DoMove(player.GetMoveDirection(), player.runSpeed, player.acceleration);
        }

        player.UseGravity(!player.IsOnSlope);
    }

    public override void UpdateState()
    {
        //for effects
        player.cam.BobCam(Vector2.one * 0.1f, 10f); //camera bobbing
        player.cam.TiltCam(player.GetInputDir().x * -3f); //set the tilt depends on the left and right inputs
        player.cam.ZoomCam(70);


        //if walang nadetect na ground mapupunta tayo sa air state without passing the data for jumping
        if (!player.IsOnFloor)
        {
            stateMachine.TransitionTo(EPlayerStates.Air);
        }

        if (player.input.actions["jump"].WasPressedThisFrame())
        {
            stateMachine.TransitionTo(EPlayerStates.Air,
                new Dictionary<string, object> { { "jump", player.jumpStrength } });
        }
        else if (player.input.actions["crouch"].IsPressed())
        {
            stateMachine.TransitionTo(EPlayerStates.Crouch);
        }
        else if (!player.input.actions["sprint"].IsPressed())
        {
            stateMachine.TransitionTo(EPlayerStates.Walk);
        }
        else if (player.GetInputDir().magnitude == 0)
        {
            stateMachine.TransitionTo(EPlayerStates.Idle);
        }
    }
}
