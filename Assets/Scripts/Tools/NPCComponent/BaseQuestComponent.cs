using Ink.Runtime;
using UnityEngine;

public abstract class BaseQuestComponent : BaseNPCComponent
{
    public TextAsset dialogueAsset;
    [Space]
    public QuestData data;
    protected Quest quest;

    protected virtual void Start()
    {
        quest = QuestManager.Instance.GetQuest(data);

        if (quest != null && quest.finished)
        {
            Owner.RemoveFromQueue(this);
        }

        DialogueManager.Instance.OnDialogueStarted += SyncDialogueVars;
        DialogueManager.Instance.OnDialogueContinue += CheckDialogue;
    }

    protected virtual void SyncDialogueVars(Story story, BaseNPC npc)
    {
        if (npc != Owner) return;

        if (quest != null)
        {
            DialogueManager.Instance.SetCustomVariable("finished", quest.finished);
            DialogueManager.Instance.SetCustomVariable("accepted", quest.accepted);
        }
    }

    protected virtual void CheckDialogue(Story story, BaseNPC npc)
    {
        if (npc != Owner) return;

        if(story.currentTags.Contains("quest"))
        {
            quest = QuestManager.Instance.AddQuest(data);
            quest.accepted = true;
            SaveNLoadManager.Instance.MarkCheckpoint();
            DialogueManager.Instance.SetCustomVariable("accepted", quest.accepted);
        }
    }
    protected abstract void CheckQuest();
}
