using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<EState> : MonoBehaviour where EState : System.Enum
{
    public abstract EState StateType { get; }

    protected StateMachine<EState> stateMachine;

    public virtual void Initialize(StateMachine<EState> stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void EnterState(Dictionary<string, object> data = null);
    public abstract void UpdateState();
    public virtual void PhysicsUpdateState()
    {

    }
    public abstract void ExitState();
}
