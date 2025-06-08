using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State machine managing the different phone UI states.
/// </summary>
public class PhoneUIStateMachine : StateMachine<PhoneUIStateMachine.PhoneStates>
{
    /// <summary>
    /// Enumeration of all possible phone UI states.
    /// </summary>
    public enum PhoneStates
    {
        MainMenu,   // Main menu screen of the phone UI
        Delivery,   // Delivery app screen
        Notes,      // Notes app screen
        Map,        // Map app screen
        Camera,     // Camera app screen
        Gallery,    // Gallery app screen
        Caller,     // Call app screen
    }
}
