using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<EStates> : MonoBehaviour where EStates : System.Enum
{
    //sa interface ito automatically pipiliin yung pinakaunang state na iseset sa enum
    //ano yung unang state na gagawin ng isang entity
    public EStates initialState;

    //for debugging, show what is the current state
    public bool showDebugState = false;

    //dictionary ng lahat ng states na detected
    //yung key which is "EStates" ay may value ng script "BaseState<EStates>>
    private Dictionary<EStates, BaseState<EStates>> stateDictionary = new Dictionary<EStates, BaseState<EStates>>();

    //yung naghohold at nagrarun ng script na state.
    private BaseState<EStates> currentState;

    private void Start()
    {
        //detecting lahat ng may BaseState<EStates> type (since nga abstract sya so ket magkaiba name ng class same type naman)
        BaseState<EStates>[] states = gameObject.GetComponentsInChildren<BaseState<EStates>>();

        //iisa isahin yung list of states using "foreach"   
        foreach (BaseState<EStates> state in states)
        {
            //if wala pa sa dictionary yung ganireng state
            if (!stateDictionary.ContainsKey(state.StateType))
            {
                //idadagdag natin sa dictionary using that syntax:
                //state.StateType is yung getter na nasa BaseState class
                stateDictionary[state.StateType] = state;
                //yung virtual void na method na nasa BaseState class is nilagay sa parameter na "this"
                //meaning 'this' script not this GameObject (magkaiba eon)
                state.Initialize(this);
            }
        }

        //we will transition to the initial state after getting all states
        TransitionTo(initialState);
    }

    //calling the update method from Monobehavior
    private void Update()
    {
        //simulating update method using updateState
        currentState?.UpdateState();
    }

    //same here XD
    private void FixedUpdate()
    {
        currentState?.PhysicsUpdate();
    }

    //this is set to public since ito yung maaccess ng bawat states and to call for transitioning the state.
    //the parameters "nextState" pag nagtatype kayo mago-oto code si visual studio kapag tinype nyo lang yung specific state
    //example: *nitype Idle (suggestion ni visual studio = "EPlayerStates.Idle") for faster workflow
    //this is the power of 'Generics'
    //Dicionary naman is yung ipapasa data which is optional din same sa "BaseState" class na "enter" method
    public void TransitionTo(EStates nextState, Dictionary<string, object> data = null)
    {
        //if di nag eexist sa dictionary yung state na nilagay
        if (!stateDictionary.ContainsKey(nextState))
        {
            Debug.LogError($"Cannot find state: {nextState} in the machine.");
            return; //return means stop right here and wag na ituloy yung rest of the code.
        }

        //the transition begin
        //called once exit muna
        currentState?.Exit();
        //then lipat ng ibang script (state) using this syntax:
        currentState = stateDictionary[nextState];
        //called once ulit to enter that state passing the data
        currentState?.Enter(data);

        //for debugging nga
        if (showDebugState) Debug.Log($"Current State: {currentState.StateType}");
    }
}
