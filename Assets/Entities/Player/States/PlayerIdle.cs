using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//State script naming convention should be: Owner + StateName
//ex. PlayerIdle

//inheriting the PlayerState class
//then i'll implement abstract class sa error (cursor sa error, then crtl+ .)
//make sure na tanggalin yung throw new System.NotImplementedException(); sa lahat ng parts.
public class PlayerIdle : PlayerState
{
    //anong state ng script na ito from specified script of enums
    //ex. Idle or EPlayerStates.Idle
    public override EPlayerStates StateType => EPlayerStates.Idle;


    //for implementation ng timer we will use it as idle cooldown
    //create muna ng timer object
    private CustomTimer timer;


    public override void Enter(Dictionary<string, object> data = null)
    {
        //play animation here
        //set something

        //pagka start ng state na to iadd na natin yung timer component with this syntax:
        timer = gameObject.AddComponent<CustomTimer>();
        //before starting we need to subscribe sa event pag natapos na yung timer
        //+= syntax meaning idadagdag natin to sa mga nakasubscribe sa kanya
        //Timeout is nasa baba sariling method
        timer.OnTimeout += Timeout;

        //then if gusto natin istart agad we can:
        //we start the timer with half a second duration
        timer.StartTimer(0.5f);

        //check states object para makita yung bagong component na timer during play
    }

    public override void Exit()
    {
        // what to do when the state about to leave
        //unsubscribe sa timer event using '-='
        timer.OnTimeout -= Timeout;
        //destroying timer component !NOT OBJECT! kasi everytime na papasok sa state na to is gagawa ng bagong timer
        //modular it is
        Destroy(timer);
    }

    //optionally physics update pwede magcheck if no choice talaga
    //ex. IsOnSlope (soon)
    public override void PhysicsUpdate()
    {
        //calling the action from player
        //we make the do move (why move eh idle nga)
        //we move with ZERO speed, meaning di gagalaw but still applying the interpolation
        //meaning we ISMUTHLY stop by the friction (for acceleration refer to walk/run state)
        player.DoMove(Vector2.zero, 0, player.friction);
        player.UseGravity(!player.IsOnSlope);
    }

    //update state will check everything
    public override void UpdateState()
    {
        //for effects
        player.cam.BobCam(new Vector2(0.001f, 0.025f), 2); //reset camera bobbing
        player.cam.TiltCam(0); //reset tilt
        player.cam.ZoomCam(60); //reset fov or zoom

        //if walang nadetect na ground mapupunta tayo sa air state without passing the data for jumping
        if (!player.IsOnFloor)
        {
            //we transition and disregard the idle
            //since nakaset sa exit yung destroying and unsubscribing no need to stress this out
            stateMachine.TransitionTo(EPlayerStates.Air);
        }

        //input specific ay taas lagi ng if else heirarchy
        //using new input system
        //using this syntax:
        //WasPressedThisFrame = kakapress lang
        //IsPressed = holding down the button
        //WasReleasedThisFrame = kakarelease lang
        if (player.input.actions["jump"].WasPressedThisFrame())
        {
            //Air state can have jump functions since jump is not a state but an action
            //so ginawa ko is pinasa ko yung data by creating new dictionary with this syntax:
            //refer to air state for retreiving the data
            stateMachine.TransitionTo(EPlayerStates.Air,
                new Dictionary<string, object> { { "jump", player.jumpStrength } });
        }
        else if (player.input.actions["crouch"].IsPressed())
        {
            stateMachine.TransitionTo(EPlayerStates.Crouch);
        }
        //magnitude or added length ng vector is hindi zero
        //meaning pag pumindot na si player ng kahit anong input direction from "movement"
        else if (player.GetInputDir().magnitude != 0)
        {
            //transitioning with this syntax
            stateMachine.TransitionTo(EPlayerStates.Walk);
        }
    }

    private void Timeout()
    {
        //what to do after ng timer

    }
}
