using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneHomeState : PhoneState
{
    public override EPhoneStates StateType => EPhoneStates.Home;

    public override void Enter(Dictionary<string, object> data = null)
    {
        phone.mainMenu.gameObject.SetActive(true);
    }

    public override void Exit()
    {
        phone.previousScreen = StateType;
        phone.mainMenu.gameObject.SetActive(false);
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void UpdateState()
    {
        if (phone.currentPressed != "home" && phone.currentPressed != "back")
        {
            stateMachine.TransitionTo(phone.GetState());
        }
    }
}
