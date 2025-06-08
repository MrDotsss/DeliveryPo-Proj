using Ink.Runtime;
using UnityEngine;

public class TalkingComponent : BaseNPCComponent
{
    [SerializeField] private TextAsset dialogueAsset;

    public override void Initialize()
    {
        if (dialogueAsset == null)
        {
            Debug.LogError($"You forgot to add dialogue asset when activating from {Owner.npcName}");
        }
    }

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

    private void DialogueComplete(Story story, BaseNPC npc)
    {
        if (npc == null) return;

        if (npc == Owner)
        {
            FinishComponent();
        }
    }
}
