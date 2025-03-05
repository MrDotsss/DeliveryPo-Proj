using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this specific base state is abstract padin
//para naman madetect si owner class ng bawat states

//instead of EStates, we can now apply the specified states ni owner which is "EPlayerStates"
public abstract class PlayerState : BaseState<EPlayerStates>
{
    //protected is can access sa kung sino nageenherit d2
    protected Player player;

    public override void Initialize(StateMachine<EPlayerStates> stateMachine)
    {
        base.Initialize(stateMachine);

        //getting the component of the owner whic is "Player"
        //ngayon pwede na gamitin ng lahat ng base states ang meron kay player
        player = stateMachine.GetComponent<Player>();
    }
}
