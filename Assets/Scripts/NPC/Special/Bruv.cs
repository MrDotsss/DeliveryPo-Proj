using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bruv : BaseNPC, IInteractable
{
    public string GetInteractionText => $"Talk to {npcName}";

    public void Interact()
    {
        talkingComponent.Activate();
    }
}
