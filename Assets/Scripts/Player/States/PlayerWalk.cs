using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalk : PlayerState
{
    public override EPlayerStates StateType => EPlayerStates.Walk;

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
            player.DoMove(player.GetSlopeDirection(), player.walkSpeed, player.acceleration);
        }
        else
        {
            player.DoMove(player.GetMoveDirection(), player.walkSpeed, player.acceleration);
        }
    }

    public override void UpdateState()
    {
        //for effects
        player.cam.BobCam(Vector2.one * 0.08f, 8f); //camera bobbing
        player.cam.TiltCam(player.GetInputDir().x * -3f); //set the tilt depends on the left and right inputs

        player.RateStamina((player.staminaRate / 2) * Time.deltaTime);

        //if walang nadetect na ground mapupunta tayo sa air state without passing the data for jumping
        if (!player.IsOnFloor)
        {
            stateMachine.TransitionTo(EPlayerStates.Air);
        }

        if (InputManager.Instance.input.actions["jump"].WasPressedThisFrame())
        {
            stateMachine.TransitionTo(EPlayerStates.Air,
                new Dictionary<string, object> { { "jump", player.jumpStrength } });
        }
        else if (InputManager.Instance.input.actions["crouch"].IsPressed())
        {
            stateMachine.TransitionTo(EPlayerStates.Crouch);
        }
        else if (InputManager.Instance.input.actions["sprint"].IsPressed() && player.currentStamina > player.maxStamina / 4)
        {
            stateMachine.TransitionTo(EPlayerStates.Run);
        }
        else if (player.GetInputDir().magnitude == 0)
        {
            stateMachine.TransitionTo(EPlayerStates.Idle);
        }
    }
}
