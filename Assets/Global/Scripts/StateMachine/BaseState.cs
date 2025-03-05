using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//abstact classes naman ay good for templating at automation
//dito inautomate na natin yung pagseset ng statemachine sa bawat state na script para di na kelangang mag get component ng marami
//para lang makita si state machine
//also para may maidagdag pang ibang logic additional sa logic na nasa abstract class

public abstract class BaseState<EStates> : MonoBehaviour where EStates : System.Enum
{
    //anong state itong script na ito
    public abstract EStates StateType { get; }

    //reference para matawag yung state machine class at mga methods and properties nito
    protected StateMachine<EStates> stateMachine;

    //optional abstract method na di kelangan i-override
    //basically isesend lang natin si state machine component dito para mahanp nya yung "Transition to"
    public virtual void Initialize(StateMachine<EStates> stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    //States
    //refer sa state machine class yung "TransitionTo" na method para mas maintindihan yung exit at enter.

    // enter state is basically a question of "anong gagawin ng state na to the time na maguumpisa ang state"
    //enter state is called once lang
    //sinet yung 'data' dictionary parameter as null kasi optional lang dapat sya 
    //since di lahat ng state ay need magconnect or magpasa ng data sa isa't isa
    public abstract void Enter(Dictionary<string, object> data = null);


    //update state is called in delta time
    //ito yung pinakamabilis magprocess at nakadepends lagi sa lakas ng hardware ni player
    //so maganda dito is for checking for inputs, setting values, at dito din yung if else for transitioning
    public abstract void UpdateState();

    //physics update naman same sa sinabi sa name, for "Physics"
    //dito na yung buong movement, nakadepende to sa frameRate na iseset natin sa GameManager script
    //para fair ang movement ket gaano pa kahina or kalakas ang hardware ni player same padin yung gameplay
    //maganda to purely for movement, physics based logics, and collisions.
    public abstract void PhysicsUpdate();


    // enter state is basically a question of "anong gagawin ng state na to the time na natapos state"
    //exit state is called once lang
    public abstract void Exit();

    //optional lang to at rarely ginagamit, nilagay ko for future purposes iwas malaking refactoring
    //handle specific timed animation codes
    public virtual void AnimationTrigger()
    {

    }
}
