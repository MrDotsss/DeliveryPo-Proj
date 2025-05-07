using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine<EState> : MonoBehaviour where EState : System.Enum
{
    public EState initialState;
    public bool showDebugState = false;


    private Dictionary<EState, BaseState<EState>> stateDictionary = new Dictionary<EState, BaseState<EState>>();
    private BaseState<EState> currentState;

    private void Start()
    {
        BaseState<EState>[] states = GetComponentsInChildren<BaseState<EState>>();

        foreach (BaseState<EState> state in states)
        {
            if (!stateDictionary.ContainsKey(state.StateType))
            {
                stateDictionary[state.StateType] = state;
                state.Initialize(this);
            }
        }

        TransitionTo(initialState);
    }

    private void LateUpdate()
    {
        currentState?.LateUpdateState();
    }
    private void Update()
    {
        currentState?.UpdateState();
    }

    private void FixedUpdate()
    {
        currentState?.PhysicsUpdate();
    }

    public void TransitionTo(EState nextState, Dictionary<string, object> data = null)
    {
        if (!stateDictionary.ContainsKey(nextState))
        {
            Debug.LogError($"Trying to transition to non-existing state: {nextState}");
            return;
        }

        currentState?.Exit();
        currentState = stateDictionary[nextState];
        currentState?.Enter(data);

        if (showDebugState) Debug.Log($"{gameObject.name}: {nextState} state");
    }
}
