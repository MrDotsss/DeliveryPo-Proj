using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruvNPC : BaseNPC, IInteractable
{
    public string InteractionText => $"Press 'F' to Talk to {npcName}";

    public void Interact()
    {
        ActivateComponent();
    }
}
