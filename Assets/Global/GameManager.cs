using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is under ENV parent object

//global script na maghahandle ng transition ng scenes
//current status ng game
//at settings ng game
//for now rough palang ito
public class GameManager : MonoBehaviour
{
    //setting ng game frame rate
    //since sa fps kasi yung sensitivity is nakabase sa lakas ng framerate
    //so mas malakas na frame rate mas sabog si sensitivity
    //setting it to specific value, madali nalang maset ni player yung sensitivity at uniform for all players
    public int gameFrameRate = 120;

    void Start()
    {
        //120 fps as default
        Application.targetFrameRate = gameFrameRate;
    }

    
}
