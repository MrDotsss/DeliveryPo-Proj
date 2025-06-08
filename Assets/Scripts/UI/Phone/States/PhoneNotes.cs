using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PhoneNotes : PhoneUIState
{
    public override PhoneUIStateMachine.PhoneStates StateType => PhoneUIStateMachine.PhoneStates.Notes;

    public GameObject itemUIPrefab;

    public Button mainTabButton;
    public Button favorTabButton;

    public GameObject mainViewParent;
    public GameObject favorViewParent;

    public int tabIndex = 0;

    public override void EnterState(Dictionary<string, object> data = null)
    {
        gameObject.SetActive(true);

        SwitchTab(0);
    }
    public override void ExitState()
    {
        gameObject.SetActive(false);

    }

    public override void UpdateState()
    {

    }

    public void SwitchTab(int index)
    {
        tabIndex = index;

        bool isMain = tabIndex == 0;

        BuildUIList(isMain ? QuestManager.Instance.GetActiveQuest(QuestType.Main) : QuestManager.Instance.GetActiveQuest(QuestType.Favor),
            isMain ? mainViewParent : favorViewParent);

        mainViewParent.SetActive(isMain);
        favorViewParent.SetActive(!isMain);

        mainTabButton.image.color = isMain ? Color.red : Color.white;
        favorTabButton.image.color = isMain ? Color.white : Color.red;
    }

    private void BuildUIList(List<Quest> items, GameObject parent)
    {
        ClearUIList(parent);

        for (int i = 0; i < items.Count; i++)
        {
            QuestItemUI itemUI = Instantiate(itemUIPrefab, parent.transform).GetComponent<QuestItemUI>();
            itemUI.quest = items[i];

            itemUI.Initialize();
        }
    }

    private void ClearUIList(GameObject parent)
    {
        if (parent.transform.childCount <= 0) return;

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
    }
}
