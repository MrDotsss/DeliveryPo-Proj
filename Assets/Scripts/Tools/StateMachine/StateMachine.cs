using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public abstract class StateMachine<EStates> : MonoBehaviour where EStates : System.Enum
{
    public EStates initialState;
    public bool debugStates = false;

    private Dictionary<EStates, BaseState<EStates>> stateDictionary = new Dictionary<EStates, BaseState<EStates>>();
    public BaseState<EStates> currentState {  get; private set; }

 
    public void Start()
    {
        BaseState<EStates>[] states = GetComponentsInChildren<BaseState<EStates>>();

        foreach (BaseState<EStates> state in states)
        {
            if (!stateDictionary.ContainsKey(state.StateType))
            {
                stateDictionary[state.StateType] = state;
                state.Initialize(this);
            }
        }

        TransitionTo(initialState);
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    private void FixedUpdate()
    {
        currentState?.PhysicsUpdateState();
    }

    public void AddState(BaseState<EStates> newState)
    {
        if (!stateDictionary.ContainsKey(newState.StateType))
        {
            stateDictionary[newState.StateType] = newState;
            newState.Initialize(this);
        }
    }

    public void ToggleState(EStates state, bool enable)
    {
        if (stateDictionary.ContainsKey(state))
        {
            stateDictionary[state].enabled = enable;
        }
    }

    public void TransitionTo(EStates nextState, Dictionary<string, object> data = null)
    {
        if (!stateDictionary.ContainsKey(nextState))
        {
            Debug.LogError($"{nextState} does not exist in state dictionary for {gameObject.name}");
            return;
        }

        currentState?.ExitState();
        currentState = stateDictionary[nextState];
        currentState?.EnterState(data);

        if (debugStates) Debug.Log($"{gameObject.name} transitioned state to {currentState}");
    }
}
