using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : BaseState<PlayerStateMachine.PlayerStates>
{
    protected Player player;

    public override void Initialize(StateMachine<PlayerStateMachine.PlayerStates> stateMachine)
    {
        base.Initialize(stateMachine);

        player = stateMachine.GetComponent<Player>();
    }
}
