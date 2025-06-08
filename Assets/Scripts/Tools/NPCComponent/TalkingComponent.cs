using Ink.Runtime;
using UnityEngine;

/// <summary>
/// Component that handles NPC talking behavior using Ink dialogue assets.
/// Inherits from BaseNPCComponent.
/// </summary>
public class TalkingComponent : BaseNPCComponent
{
    [SerializeField] private TextAsset dialogueAsset;  // Ink dialogue asset for this NPC

    /// <summary>
    /// Checks if the dialogue asset is assigned; logs error if missing.
    /// </summary>
    public override void Initialize()
    {
        if (dialogueAsset == null)
        {
            Debug.LogError($"You forgot to add dialogue asset when activating from {Owner.npcName}");
        }
    }

    /// <summary>
    /// Starts the dialogue if not already in dialogue UI.
    /// Subscribes to dialogue end event.
    /// </summary>
    public override void Activate()
    {
        if (UIManager.Instance.CurrentPanel == UIManager.EUIPanels.Dialogue) return;

        if (dialogueAsset != null)
        {
            UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Dialogue);
            DialogueManager.Instance.StartDialogue(dialogueAsset, Owner);

            DialogueManager.Instance.OnDialogueEnded += DialogueComplete;
        }
        else
        {
            Debug.LogError($"You forgot to add dialogue asset when activating from {Owner.npcName}");
        }
    }

    /// <summary>
    /// Callback when dialogue ends. If this NPC owns the dialogue,
    /// marks component as finished.
    /// </summary>
    private void DialogueComplete(Story story, BaseNPC npc)
    {
        if (npc == null) return;

        if (npc == Owner)
        {
            FinishComponent();
        }
    }
}
