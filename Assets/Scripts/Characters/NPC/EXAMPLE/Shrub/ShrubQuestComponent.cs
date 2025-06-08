using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using UnityEngine;

/// <summary>
/// ShrubQuestComponent manages a specific quest where the NPC tracks the collection of items.
/// </summary>
public class ShrubQuestComponent : BaseQuestComponent
{
    public ItemData ballData; // The item data representing the required ball
    public int ballCount = 0; // Counter for the number of collected balls

    /// <summary>
    /// Activates the quest component by switching to the dialogue panel and starting the relevant dialogue.
    /// Then checks the quest status.
    /// </summary>
    public override void Activate()
    {
        UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Dialogue);
        DialogueManager.Instance.StartDialogue(dialogueAsset, Owner);

        CheckQuest();
    }

    /// <summary>
    /// Synchronizes dialogue variables with the story data.
    /// </summary>
    /// <param name="story">The current Ink story instance.</param>
    /// <param name="npc">The NPC associated with the dialogue.</param>
    protected override void SyncDialogueVars(Story story, BaseNPC npc)
    {
        base.SyncDialogueVars(story, npc);

        // Updates the dialogue system's ball count variable.
        DialogueManager.Instance.SetCustomVariable("ballCount", ballCount);
    }

    /// <summary>
    /// Checks the quest status and updates the inventory items accordingly.
    /// Removes the required item until the condition is met.
    /// </summary>
    protected override void CheckQuest()
    {
        if (quest == null) return; // Ensure the quest exists before proceeding.

        List<InventoryItem> items = InventoryManager.Instance.GetAllInventoryList().ToList(); // Retrieve all inventory items

        foreach (InventoryItem item in items)
        {
            // If the item matches the required type and the ball count is not complete, remove it from inventory.
            if (ballCount != 3 && item.data == ballData)
            {
                InventoryManager.Instance.DropItem(item);
                ballCount++;
                DialogueManager.Instance.SetCustomVariable("ballCount", ballCount);
            }
        }

        // If the required number of items has been collected, mark the quest as finished.
        if (ballCount == 3)
        {
            QuestManager.Instance.FinishQuest(quest);
            DialogueManager.Instance.SetCustomVariable("finished", quest.finished);
            loop = false;
            FinishComponent();
        }
    }
}
