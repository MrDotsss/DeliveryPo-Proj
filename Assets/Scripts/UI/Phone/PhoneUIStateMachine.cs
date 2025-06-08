using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneUIStateMachine : StateMachine<PhoneUIStateMachine.PhoneStates>
{
    public enum PhoneStates
    {
        MainMenu,
        Delivery,
        Notes,
        Map,
        Camera,
        Gallery,
        Caller,
    }
}
