using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAir : PlayerState
{
    public override EPlayerStates StateType => EPlayerStates.Air;

    private CustomTimer timer;

    public override void Enter(Dictionary<string, object> data = null)
    {
        player.DefineVelocity(player.GetVelocity().x, 0, player.GetVelocity().z);

        //checking the data if may laman yung data
        if (data != null)
        {
            //checking if may jump nga na key
            if (data.ContainsKey("jump"))
            {
                //using (float) convertion since alam natin na float yung value ng data
                //will return error if di iconvert sa specific value
                //access the value using this syntax: data["jump"]
                player.RateStamina(-player.staminaRate);
                player.DoJump((float)data["jump"]);
            }
        }

        timer = gameObject.AddComponent<CustomTimer>();
        timer.StartTimer(0.15f);
    }

    public override void Exit()
    {
        Destroy(timer);
    }

    public override void PhysicsUpdate()
    {
        //meaning we ISMUTHLY move by the air friction (slow movement in the air)
        //using direction and maxspeed
        player.UseGravity(true);

        if (!player.canInput) return;

        player.DoMove(player.GetMoveDirection(), player.walkSpeed, player.airFriction);
    }

    public override void UpdateState()
    {
        player.ApplyMovement();

        if (!player.canInput) return;

        //for effects
        player.cam.BobCam(Vector2.zero, 8f); //camera bobbing
        player.cam.TiltCam(player.GetInputDir().x * -1.5f); //set the tilt depends on the left and right inputs

        //if nakadetect na ng floor we back to idle
        //and the state loop continues
        if (player.IsOnFloor && !timer.IsRunning)
        {
            stateMachine.TransitionTo(EPlayerStates.Idle);
        }

        //if yung y-velocity is negative which is falling
        //and nadetect na nasa ledge 
        //and we keep pressing moving forward (y axis in input positive)
        if (player.GetVelocity().y < 0 && player.IsOnLedge && player.GetInputDir().y > 0)
        {
            player.DoVault(player.jumpStrength * 1.3f);
        }

        player.cam.DoLook(player.cam.GetMouseDelta());
    }
}
