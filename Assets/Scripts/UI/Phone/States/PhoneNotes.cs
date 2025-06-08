using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Notes UI state in the phone UI, displaying main and favor quests in tabs.
/// </summary>
public class PhoneNotes : PhoneUIState
{
    public override PhoneUIStateMachine.PhoneStates StateType => PhoneUIStateMachine.PhoneStates.Notes;

    public GameObject itemUIPrefab;

    public Button mainTabButton;
    public Button favorTabButton;

    public GameObject mainViewParent;
    public GameObject favorViewParent;

    public int tabIndex = 0;

    /// <summary>
    /// Called when entering the Notes state. Activates the UI and switches to the main tab by default.
    /// </summary>
    public override void EnterState(Dictionary<string, object> data = null)
    {
        gameObject.SetActive(true);

        SwitchTab(0);
    }

    /// <summary>
    /// Called when exiting the Notes state. Deactivates the UI.
    /// </summary>
    public override void ExitState()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called every frame while in the Notes state. (Currently unused)
    /// </summary>
    public override void UpdateState()
    {
    }

    /// <summary>
    /// Switches between the main and favor tabs, updating UI and button visuals.
    /// </summary>
    /// <param name="index">Tab index: 0 for main, 1 for favor</param>
    public void SwitchTab(int index)
    {
        tabIndex = index;

        bool isMain = tabIndex == 0;

        BuildUIList(
            isMain ? QuestManager.Instance.GetActiveQuest(QuestType.Main) : QuestManager.Instance.GetActiveQuest(QuestType.Favor),
            isMain ? mainViewParent : favorViewParent);

        mainViewParent.SetActive(isMain);
        favorViewParent.SetActive(!isMain);

        mainTabButton.image.color = isMain ? Color.red : Color.white;
        favorTabButton.image.color = isMain ? Color.white : Color.red;
    }

    /// <summary>
    /// Builds the quest UI list by instantiating UI elements for each quest and initializing them.
    /// </summary>
    /// <param name="items">List of quests to display</param>
    /// <param name="parent">Parent GameObject for the UI items</param>
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

    /// <summary>
    /// Clears the existing quest UI list by destroying all child UI elements under the given parent.
    /// </summary>
    /// <param name="parent">Parent GameObject containing quest UI items</param>
    private void ClearUIList(GameObject parent)
    {
        if (parent.transform.childCount <= 0) return;

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
    }
}
