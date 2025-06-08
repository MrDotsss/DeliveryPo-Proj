using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

public class DeliveryComponent : BaseNPCComponent
{
    [Header("References")]
    public QuestData questData;
    [SerializeField] private TextAsset dialogueAsset;
    [SerializeField] private CaptureBox captureBox;

    public bool IsReceived { get; private set; }
    public Quest quest { get; private set; }

    private void Start()
    {
        quest = QuestManager.Instance.AddQuest(questData);

        if (!quest.finished)
        {
            foreach (ItemData item in questData.items)
            {
                InventoryManager.Instance.AddItem(item);
            }
        }
        else
        {
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
            captureBox.OnCapture += Captured;
            captureBox.gameObject.SetActive(false);

            captureBox.fileName = quest.data.questTitle;
            captureBox.description = $"{Owner.npcName}\n{quest.data.questDescription}";
        }
    }

    public override void Activate()
    {
        if (IsReceived)
        {
            if (!quest.finished)
            {
                UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Dialogue);
                DialogueManager.Instance.StartDialogue(DialogueManager.Instance.GetDefault("PODFirst"));
            }

            return;
        }
        else
        {
            CheckCompletion();
        }
    }

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
