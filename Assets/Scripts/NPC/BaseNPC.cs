using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNPC : MonoBehaviour
{
    [Header("References")]
    public TalkingNPC talkingComponent;

    [Header("Properties")]
    public string npcName = "unknown";
}
