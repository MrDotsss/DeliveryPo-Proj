using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

/// <summary>
/// DeliveryComponent handles NPC delivery quest interactions,
/// including quest initialization, item checking, dialogue,
/// and capture/photo events upon successful delivery.
/// Inherits from BaseNPCComponent.
/// </summary>
public class DeliveryComponent : BaseNPCComponent
{
    [Header("References")]
    public QuestData questData;             // Quest data defining delivery items and info
    [SerializeField] private TextAsset dialogueAsset;  // Dialogue asset for delivery interaction
    [SerializeField] private CaptureBox captureBox;    // CaptureBox to handle photo capture on delivery

    public bool IsReceived { get; private set; }      // Whether delivery is completed
    public Quest quest { get; private set; }          // Reference to the active quest instance

    /// <summary>
    /// Initializes the delivery quest: adds quest and items if not finished,
    /// sets up capture box and subscribes to dialogue continue event.
    /// </summary>
    public override void Initialize()
    {
        quest = QuestManager.Instance.AddQuest(questData);

        if (!quest.finished)
        {
            // Add quest items to inventory if quest unfinished
            foreach (ItemData item in questData.items)
            {
                InventoryManager.Instance.AddItem(item);
            }
        }
        else
        {
            // Mark as received if quest already finished and remove from owner's queue
            IsReceived = true;
            Owner.RemoveFromQueue(this);
        }

        DialogueManager.Instance.OnDialogueContinue += OnDeliver;

        if (captureBox == null)
        {
            Debug.LogError("No CaptureBox assigned.");
        }
        else
        {
            // Setup capture event and assign quest info for capture box
            captureBox.OnCapture += Captured;
            captureBox.gameObject.SetActive(false);

            captureBox.fileName = quest.data.questTitle;
            captureBox.description = $"{Owner.npcName}\n{quest.data.questDescription}";
        }
    }

    /// <summary>
    /// Activates the delivery component:
    /// - if already received, starts default dialogue or finishes component if quest done
    /// - if not received, checks completion status based on inventory
    /// </summary>
    public override void Activate()
    {
        if (IsReceived)
        {
            if (!quest.finished)
            {
                UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Dialogue);
                DialogueManager.Instance.StartDialogue(DialogueManager.Instance.GetDefault("PODFirst"));
            }
            else
            {
                loop = false;
                FinishComponent();
            }
            return;
        }
        else
        {
            CheckCompletion();
        }
    }

    /// <summary>
    /// Checks if the player has the required delivery item,
    /// and starts appropriate dialogue (delivery, wrong item, or no item).
    /// </summary>
    private void CheckCompletion()
    {
        InventoryItem currentItem = InventoryManager.Instance.CurrentItem;
        UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Dialogue);

        if (currentItem != null)
        {
            if (questData.items.Contains(currentItem.data))
            {
                DialogueManager.Instance.StartDialogue(dialogueAsset, Owner);
            }
            else
            {
                DialogueManager.Instance.StartDialogue(DialogueManager.Instance.GetDefault("WrongDelivery"));
            }
        }
        else
        {
            DialogueManager.Instance.StartDialogue(DialogueManager.Instance.GetDefault("NothingToSay"));
        }
    }

    /// <summary>
    /// Called during dialogue continuation; if delivery tag is found and NPC matches owner,
    /// drops delivery items from inventory, marks delivery as received,
    /// enables capture box and unsubscribes from event.
    /// </summary>
    private void OnDeliver(Story story, BaseNPC npc)
    {
        if (npc == null || IsReceived) return;

        if (npc.npcName == Owner.npcName && story.currentTags.Contains("deliver"))
        {
            foreach (ItemData item in questData.items)
            {
                InventoryManager.Instance.DropItem(
                    InventoryManager.Instance.FindInventory(item));
            }

            IsReceived = true;
            captureBox.gameObject.SetActive(true);
            DialogueManager.Instance.OnDialogueContinue -= OnDeliver;
        }
    }

    /// <summary>
    /// Called when capture box photo is taken; if the capture matches quest,
    /// finishes quest, unsubscribes from capture event, destroys capture box object,
    /// and finishes this component.
    /// </summary>
    private void Captured(string captureName)
    {
        if (IsReceived && captureName == captureBox.fileName)
        {
            QuestManager.Instance.FinishQuest(quest);
            captureBox.OnCapture -= Captured;
            Destroy(captureBox.gameObject);
            FinishComponent();
        }
    }
}
