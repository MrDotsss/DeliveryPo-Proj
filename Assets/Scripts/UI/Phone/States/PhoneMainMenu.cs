using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the Main Menu state of the Phone UI.
/// Handles enabling and disabling the main menu panel.
/// </summary>
public class PhoneMainMenu : PhoneUIState
{
    public override PhoneUIStateMachine.PhoneStates StateType => PhoneUIStateMachine.PhoneStates.MainMenu;

    /// <summary>
    /// Called when entering the Main Menu state.
    /// Activates the main menu UI.
    /// </summary>
    public override void EnterState(Dictionary<string, object> data = null)
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Called when exiting the Main Menu state.
    /// Deactivates the main menu UI.
    /// </summary>
    public override void ExitState()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called every frame while in the Main Menu state.
    /// Currently no update logic is needed here.
    /// </summary>
    public override void UpdateState()
    {

    }
}
