using Ink.Runtime;
using UnityEngine;

/// <summary>
/// BaseQuestComponent provides a foundation for NPC quest components,
/// managing quest data, dialogue syncing, and quest acceptance.
/// Inherits from BaseNPCComponent and is abstract.
/// </summary>
public abstract class BaseQuestComponent : BaseNPCComponent
{
    public TextAsset dialogueAsset;    // Dialogue asset related to the quest
    [Space]
    public QuestData data;             // Quest data reference
    protected Quest quest;             // Active quest instance

    /// <summary>
    /// Initializes the quest component by retrieving the quest,
    /// removing from queue if finished, and subscribing to dialogue events.
    /// </summary>
    public override void Initialize()
    {
        quest = QuestManager.Instance.GetQuest(data);

        if (quest != null && quest.finished)
        {
            Owner.RemoveFromQueue(this);
        }

        DialogueManager.Instance.OnDialogueStarted += SyncDialogueVars;
        DialogueManager.Instance.OnDialogueContinue += CheckDialogue;
    }

    /// <summary>
    /// Syncs quest-related custom variables to the dialogue system,
    /// such as whether the quest is finished or accepted.
    /// Only syncs if the dialogue is for this component's owner NPC.
    /// </summary>
    /// <param name="story">The Ink story instance.</param>
    /// <param name="npc">The NPC involved in the dialogue.</param>
    protected virtual void SyncDialogueVars(Story story, BaseNPC npc)
    {
        if (npc != Owner) return;

        if (quest != null)
        {
            DialogueManager.Instance.SetCustomVariable("finished", quest.finished);
            DialogueManager.Instance.SetCustomVariable("accepted", quest.accepted);
        }
    }

    /// <summary>
    /// Checks dialogue progression for quest acceptance,
    /// adds the quest if tagged with "quest", marks accepted,
    /// saves checkpoint, and updates dialogue variables.
    /// </summary>
    /// <param name="story">The Ink story instance.</param>
    /// <param name="npc">The NPC involved in the dialogue.</param>
    protected virtual void CheckDialogue(Story story, BaseNPC npc)
    {
        if (npc != Owner) return;

        if (story.currentTags.Contains("quest"))
        {
            quest = QuestManager.Instance.AddQuest(data);
            quest.accepted = true;
            SaveNLoadManager.Instance.MarkCheckpoint();
            DialogueManager.Instance.SetCustomVariable("accepted", quest.accepted);
        }
    }

    /// <summary>
    /// Abstract method to check quest progress or completion,
    /// to be implemented by subclasses.
    /// </summary>
    protected abstract void CheckQuest();
}
