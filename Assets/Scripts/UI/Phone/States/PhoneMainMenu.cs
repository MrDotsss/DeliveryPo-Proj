using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneMainMenu : PhoneUIState
{
    public override PhoneUIStateMachine.PhoneStates StateType => PhoneUIStateMachine.PhoneStates.MainMenu;

    public override void EnterState(Dictionary<string, object> data = null)
    {
        gameObject.SetActive(true);
    }

    public override void ExitState()
    {
        gameObject.SetActive(false);
    }

    public override void UpdateState()
    {
        
    }
}
