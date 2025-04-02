using System.Collections;
using Ink.Runtime;
using UnityEngine;

public class DeliveryComponent : MonoBehaviour, INPComponent
{
    [Header("Properties")]
    public Quest questData;
    [SerializeField] private TextAsset deliveryDialogue;
    [Space]
    [SerializeField] private CaptureBox captureBox;

    public BaseNPC Owner { get; private set; }

    public bool IsReceived { get; private set; }

    public QuestItem questItem { get; private set; }

    private void Start()
    {
        captureBox.gameObject.SetActive(false);

        questItem = QuestManager.Instance.RegisterQuest(questData);

        foreach (ItemData item in questData.requiredItems)
        {
            PlayerInventory.Instance.AddItem(item);
        }

        DialogueManager.Instance.OnDialogueContinue += OnDeliver;

        Owner = GetComponent<BaseNPC>();

        captureBox.OnCapture += ((b) =>
        {
            if (IsReceived && b == transform.name)
            {
                QuestManager.Instance.FinishQuest(questItem);
                Destroy(captureBox.gameObject);
            }
        });
    }

    public void Activate()
    {
        if (IsReceived)
        {
            if (!questItem.IsFinished)
            {
                DialogueManager.Instance.StartDialogue(DialogueManager.Instance.GetDefaultDialogue("PODFirst"), Owner);
            }
            return;
        }

        CheckCompletion();
    }

    public void CheckCompletion()
    {
        ItemData currentItem = PlayerInventory.Instance.equippedItem?.itemData;

        if (currentItem != null)
        {
            if (questData.requiredItems.Contains(currentItem))
            {
                DialogueManager.Instance.StartDialogue(deliveryDialogue, Owner);
            }
            else
            {
                DialogueManager.Instance.StartDialogue(DialogueManager.Instance.GetDefaultDialogue("WrongDelivery"), Owner);
            }
        }
        else
        {
            DialogueManager.Instance.StartDialogue(DialogueManager.Instance.GetDefaultDialogue("NothingToSay"), Owner);
        }
    }

    private void OnDeliver(Story story, BaseNPC npc)
    {
        if (npc.npcName == Owner.npcName && story.currentTags.Contains("deliver"))
        {
            foreach (ItemData item in questData.requiredItems)
            {
                PlayerInventory.Instance.DropItem(PlayerInventory.Instance.FindParcel(item));
            }

            IsReceived = true;
            captureBox.gameObject.SetActive(true);
            DialogueManager.Instance.OnDialogueContinue -= OnDeliver;
        }

    }
}
