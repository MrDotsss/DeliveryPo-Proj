using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PhoneUIState : BaseState<PhoneUIStateMachine.PhoneStates>
{
    protected PhoneUI phone;

    public override void Initialize(StateMachine<PhoneUIStateMachine.PhoneStates> stateMachine)
    {
        base.Initialize(stateMachine);

        phone = stateMachine.GetComponentInParent<PhoneUI>();

        gameObject.SetActive(false);
    }
}
