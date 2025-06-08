using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using UnityEngine;

public class ShrubQuestComponent : BaseQuestComponent
{
    public ItemData ballData;
    public int ballCount = 0;

    public override void Activate()
    {
        UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Dialogue);
        DialogueManager.Instance.StartDialogue(dialogueAsset, Owner);

        CheckQuest();
    }

    protected override void SyncDialogueVars(Story story, BaseNPC npc)
    {
        base.SyncDialogueVars(story, npc);

        DialogueManager.Instance.SetCustomVariable("ballCount", ballCount);
    }

    protected override void CheckQuest()
    {
        if (quest == null) return;

        List<InventoryItem> items = InventoryManager.Instance.GetAllInventoryList().ToList();

        foreach (InventoryItem item in items)
        {
            if(ballCount != 3 && item.data == ballData)
            {
                InventoryManager.Instance.DropItem(item);
                ballCount++;
                DialogueManager.Instance.SetCustomVariable("ballCount", ballCount);
            }
        }

        if (ballCount == 3)
        {
            QuestManager.Instance.FinishQuest(quest);
            DialogueManager.Instance.SetCustomVariable("finished", quest.finished);
            loop = false;
            FinishComponent();
        }
    }
}
