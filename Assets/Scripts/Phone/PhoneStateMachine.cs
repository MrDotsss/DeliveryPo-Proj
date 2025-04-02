using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPhoneStates
{
    Home,
    Delivery,
    Todo,
    Camera,
    Gallery,
    Flashlight,
    Phone
}

public class PhoneStateMachine : StateMachine<EPhoneStates>
{
    
}
