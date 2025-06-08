using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// Generic finite state machine base class.
/// Manages states of type EStates (an Enum), storing and transitioning between BaseState<EStates> instances.
/// </summary>
/// <typeparam name="EStates">Enum type representing the possible states.</typeparam>
public abstract class StateMachine<EStates> : MonoBehaviour where EStates : System.Enum
{
    public EStates initialState;  // The state the machine starts in
    public bool debugStates = false; // Enable debug logging of state transitions

    // Dictionary mapping enum states to their corresponding BaseState instances
    private Dictionary<EStates, BaseState<EStates>> stateDictionary = new Dictionary<EStates, BaseState<EStates>>();

    // The current active state of the machine
    public BaseState<EStates> currentState { get; private set; }

    /// <summary>
    /// Called by Unity on Start.
    /// Finds all BaseState components in children, initializes them, and transitions to the initial state.
    /// </summary>
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

    /// <summary>
    /// Called every frame by Unity.
    /// Calls the UpdateState method on the current state.
    /// </summary>
    private void Update()
    {
        currentState?.UpdateState();
    }

    /// <summary>
    /// Called at fixed intervals by Unity.
    /// Calls the PhysicsUpdateState method on the current state.
    /// </summary>
    private void FixedUpdate()
    {
        currentState?.PhysicsUpdateState();
    }

    /// <summary>
    /// Adds a new state to the state dictionary and initializes it if not already present.
    /// </summary>
    /// <param name="newState">The new state to add.</param>
    public void AddState(BaseState<EStates> newState)
    {
        if (!stateDictionary.ContainsKey(newState.StateType))
        {
            stateDictionary[newState.StateType] = newState;
            newState.Initialize(this);
        }
    }

    /// <summary>
    /// Enables or disables a specific state component.
    /// </summary>
    /// <param name="state">The state to toggle.</param>
    /// <param name="enable">Enable or disable the state.</param>
    public void ToggleState(EStates state, bool enable)
    {
        if (stateDictionary.ContainsKey(state))
        {
            stateDictionary[state].enabled = enable;
        }
    }

    /// <summary>
    /// Transitions the state machine to the specified next state.
    /// Calls ExitState on the current state and EnterState on the new state.
    /// </summary>
    /// <param name="nextState">The state to transition to.</param>
    /// <param name="data">Optional data to pass to the new state's EnterState method.</param>
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
