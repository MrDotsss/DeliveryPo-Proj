using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PhoneState : BaseState<EPhoneStates>
{
    protected Phone phone;

    public override void Initialize(StateMachine<EPhoneStates> stateMachine)
    {
        base.Initialize(stateMachine);

        phone = stateMachine.GetComponent<Phone>();
    }
}
