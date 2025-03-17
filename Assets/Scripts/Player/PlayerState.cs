using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : BaseState<EPlayerStates>
{
    protected Player player;

    public override void Initialize(StateMachine<EPlayerStates> stateMachine)
    {
        base.Initialize(stateMachine);

        player = stateMachine.GetComponent<Player>();
    }
}
