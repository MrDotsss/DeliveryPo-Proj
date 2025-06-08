using UnityEngine;

public class ShrubNPC : BaseNPC, IInteractable
{
    public string InteractionText => $"Press 'F' to talk to {aliasName}";

    public void Interact()
    {
        ActivateComponent();
    }
}
