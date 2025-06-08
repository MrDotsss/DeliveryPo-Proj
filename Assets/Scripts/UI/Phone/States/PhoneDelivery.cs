using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneDelivery : PhoneUIState
{
    public override PhoneUIStateMachine.PhoneStates StateType => PhoneUIStateMachine.PhoneStates.Delivery;

    public GameObject itemUIPrefab;
    public GameObject content;

    public override void EnterState(Dictionary<string, object> data = null)
    {
        gameObject.SetActive(true);

        BuildUIList();
    }
    public override void ExitState()
    {
        gameObject.SetActive(false);

    }

    public override void UpdateState()
    {

    }
    private void BuildUIList()
    {
        List<Quest> items = QuestManager.Instance.GetActiveQuest(QuestType.Delivery);

        ClearUIList();

        for (int i = 0; i < items.Count; i++)
        {
            QuestItemUI itemUI = Instantiate(itemUIPrefab, content.transform).GetComponent<QuestItemUI>();
            itemUI.quest = items[i];

            itemUI.Initialize();
        }
    }

    private void ClearUIList()
    {
        if (content.transform.childCount <= 0) return;

        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
    }
}
