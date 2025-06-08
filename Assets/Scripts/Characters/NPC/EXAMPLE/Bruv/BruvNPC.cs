using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BruvNPC is a specific implementation of BaseNPC that supports interaction.
/// Implements the IInteractable interface to define interaction behavior.
/// </summary>
public class BruvNPC : BaseNPC, IInteractable
{
    /// <summary>
    /// Provides interaction text when the player is near the NPC.
    /// Displays a prompt to press 'F' to talk.
    /// </summary>
    public string InteractionText => $"Press 'F' to Talk to {npcName}";

    /// <summary>
    /// Handles player interaction with the NPC.
    /// Triggers the NPC's component activation process.
    /// </summary>
    public void Interact()
    {
        ActivateComponent();
    }
}
