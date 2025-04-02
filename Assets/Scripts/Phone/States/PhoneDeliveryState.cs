using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneDeliveryState : PhoneState
{
    public override EPhoneStates StateType => EPhoneStates.Delivery;

    [SerializeField] private RectTransform deliveryList;
    [SerializeField] private GameObject questTextPrefab;

    private List<QuestItem> deliveryQuests = new List<QuestItem>();

    public override void Enter(Dictionary<string, object> data = null)
    {
        phone.deliveryApp.gameObject.SetActive(true);

        InitializeQuests();
        PopulateQuests();
    }

    public override void Exit()
    {
        deliveryQuests.Clear();

        phone.previousScreen = StateType;
        phone.deliveryApp.gameObject.SetActive(false);
    }

    public override void PhysicsUpdate()
    {

    }

    public override void UpdateState()
    {
        if (phone.currentPressed != "delivery" || phone.currentPressed == "back")
        {
            stateMachine.TransitionTo(phone.GetState());
        }
    }

    private void InitializeQuests()
    {
        List<QuestItem> activeQuests = QuestManager.Instance.GetActiveQuests();

        foreach (QuestItem quest in activeQuests)
        {
            if (quest.data.questType == QuestType.Delivery && !deliveryQuests.Contains(quest))
            {
                if (quest.IsFinished)
                {
                    deliveryQuests.Add(quest);
                }
                else
                {
                    deliveryQuests.Insert(0, quest);
                }

            }
        }
    }

    private void PopulateQuests()
    {
        foreach (Transform child in deliveryList) Destroy(child.gameObject);

        foreach (QuestItem quest in deliveryQuests)
        {
            QuestText questText = Instantiate(questTextPrefab, deliveryList).GetComponent<QuestText>();
            questText.questData = quest;
        }
    }
}
