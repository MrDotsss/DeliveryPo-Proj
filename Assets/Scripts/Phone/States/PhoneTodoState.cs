using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhoneTodoState : PhoneState
{
    public override EPhoneStates StateType => EPhoneStates.Todo;

    [SerializeField] private RectTransform mainList;
    [SerializeField] private GameObject questTextPrefab;

    private List<QuestItem> todoQuests = new List<QuestItem>();

    public override void Enter(Dictionary<string, object> data = null)
    {
        phone.todoApp.gameObject.SetActive(true);

        InitializeQuests();
        PopulateQuests();
    }

    public override void Exit()
    {
        phone.previousScreen = StateType;
        phone.todoApp.gameObject.SetActive(false);
    }

    public override void PhysicsUpdate()
    {

    }

    public override void UpdateState()
    {
        if (phone.currentPressed != "todo" || phone.currentPressed == "back")
        {
            stateMachine.TransitionTo(phone.GetState());
        }
    }

    private void InitializeQuests()
    {
        List<QuestItem> activeQuests = QuestManager.Instance.GetActiveQuests();

        foreach (QuestItem quest in activeQuests)
        {
            if (quest.data.questType == QuestType.Main || quest.data.questType == QuestType.Favor && !todoQuests.Contains(quest))
            {
                if (quest.IsFinished)
                {
                    todoQuests.Add(quest);
                }
                else
                {
                    todoQuests.Insert(0, quest);
                }

            }
        }
    }

    private void PopulateQuests()
    {
        foreach (Transform child in mainList)
        {
            if (child.GetComponent<TextMeshProUGUI>() == null)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (QuestItem quest in todoQuests)
        {

            QuestText questText = Instantiate(questTextPrefab, mainList).GetComponent<QuestText>();
            questText.questData = quest;

        }
    }
}
