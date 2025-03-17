using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : PlayerState
{
    public override EPlayerStates StateType => EPlayerStates.Idle;

    public override void Enter(Dictionary<string, object> data = null)
    {

    }

    public override void Exit()
    {

    }

    public override void PhysicsUpdate()
    {
        player.DoMove(Vector3.zero, 0, player.friction);
    }

    public override void UpdateState()
    {
        //for effects
        player.cam.BobCam(new Vector2(0.001f, 0.025f), 4f); //reset camera bobbing
        player.cam.TiltCam(0); //reset tilt

        player.RateStamina(player.staminaRate * Time.deltaTime);

        if (!player.IsOnFloor)
        {
            stateMachine.TransitionTo(EPlayerStates.Air);
        }

        if (InputManager.Instance.input.actions["jump"].WasPressedThisFrame())
        {
            stateMachine.TransitionTo(EPlayerStates.Air, new Dictionary<string, object> { { "jump", player.jumpStrength } });
        } else if (InputManager.Instance.input.actions["crouch"].IsPressed())
        {
            stateMachine.TransitionTo(EPlayerStates.Crouch);
        } else if (player.GetInputDir().magnitude != 0)
        {
            stateMachine.TransitionTo(EPlayerStates.Walk);
        }
    }
}
